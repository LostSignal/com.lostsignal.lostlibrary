//-----------------------------------------------------------------------
// <copyright file="PlayablesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_GOOGLE_MAPS_SDK

namespace Lost.LBE
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Lost.Networking;
    using Newtonsoft.Json;
    using UnityEngine;

    ////
    //// NOTE [bgish]: This Manager is temporary and will eventually move to be purely CloudFunctions.
    ////
    public class PlayablesManager : Manager<PlayablesManager>
    {
        private const int MaxRetryCount = 3;

#pragma warning disable 0649
        [SerializeField] private string apiKey;
#pragma warning restore 0649

        private Dictionary<ulong, byte[]> fakeRedisCache = new Dictionary<ulong, byte[]>();

        public async Task<GetS2CellResult> GetS2Cell(GetS2CellRequest request, int tryCount = 0)
        {
            var cachedResult = this.GetFromRedisCache(request.S2CellId);

            if (cachedResult != null)
            {
                cachedResult.CalculateSecondsToLive();
                return cachedResult;
            }

            var databaseResult = this.GetFromDatabase(request.S2CellId);

            if (databaseResult == null || databaseResult.IsExpired())
            {
                var newResult = await this.GetFromPlayablesApi(request.S2CellId);

                try
                {
                    if (this.SaveToDatabase(databaseResult))
                    {
                        // We successfully updated the database!
                        // Potentially we will have database entries for each location.  If that's the case, then do the following
                        //   * Delete all the old database locations from databaseResult if it's not null
                        //   * Add all the locations from result

                        databaseResult = newResult;
                    }
                    else
                    {
                        // There was an etag mismatch because someone beat us to the punch, so lets get
                        databaseResult = this.GetFromDatabase(request.S2CellId);
                    }
                }
                catch
                {
                    if (tryCount < MaxRetryCount)
                    {
                        return await this.GetS2Cell(request, tryCount++);
                    }
                    else
                    {
                        return new GetS2CellResult { ResultCode = GetS2CellResult.GetS2CellResultCode.DatabaseError };
                    }
                }
            }

            this.SaveToRedisCache(databaseResult);

            if (databaseResult == null)
            {
                Debug.LogError("Null databaseResult!  No clue how this could happen.");
            }

            databaseResult.CalculateSecondsToLive();
            return databaseResult;
        }

        public Task<VisitLocationResult> VisitLocation(VisitLocationRequest request)
        {
            //// * Need a Visit Api? (Pokestops)
            //// * Player Data stores a Dictionary of LBEID to DateTime
            ////   * VisitRequest LBEID
            ////     * Loads visit history, if that id dne or DateTime.UtcNow > stored DateTime
            ////       * Remove any old entries from teh dictionary
            ////       * Add the LBEID with DateTime.UTCNow
            ////       * Save Dictionary to DB
            ////       * Calculate and Send Goodies (Maybe these should be PlayFab Drop tables?)
            ////     * Else
            ////       * return Too Soon!
            ////   * VisitResponse List of Goodies
            ////   * PlayerData has

            return null;
        }

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        private async Task<GetS2CellResult> GetFromPlayablesApi(ulong s2CellId)
        {
            var resultJson = await this.SendPlayablesRequest(s2CellId);
            var playablesResponse = JsonUtil.Deserialize<PlayablesResponse>(resultJson);

            var s2CellResult = new GetS2CellResult
            {
                S2CellId = s2CellId,
                Locations = new List<LBELocation>(),
                ExpirationUtc = DateTime.UtcNow.Add(ParseTTL(playablesResponse.TTL)),
            };

            foreach (var gameObjectType in playablesResponse.LocationsPerGameObjectType)
            {
                var typeId = gameObjectType.Key.ToString();

                var locationCount = gameObjectType.Value?.Locations?.Count;

                if (locationCount == null || locationCount == 0)
                {
                    continue;
                }

                foreach (var location in gameObjectType.Value.Locations)
                {
                    s2CellResult.Locations.Add(new LBELocation
                    {
                        LocationId = location.Name,
                        TypeId = typeId,
                        LatLong = new GPSLatLong
                        {
                            Latitude = location.CenterPoint != null ? location.CenterPoint.Latitude : location.SnappedPoint.Latitude,
                            Longitude = location.CenterPoint != null ? location.CenterPoint.Longitude : location.SnappedPoint.Longitude,
                        },
                    });
                }
            }

            return s2CellResult;
        }

        private async Task<string> SendPlayablesRequest(ulong s2CellId)
        {
            var uri = "https://playablelocations.googleapis.com/v3:samplePlayableLocations?key=" + this.apiKey;
            var body = this.GetPlayablesRequestTemplate().Replace("{s2CellId}", s2CellId.ToString());
            var response = await HttpUtil.SendJsonPost(uri, body);

            Debug.Log("Response = " + response);
            Debug.Log("Status Code = " + response?.StatusCode);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
            {
                Debug.Log("Success!");
            }

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private string GetPlayablesRequestTemplate()
        {
            return
@"{
    ""area_filter"":{
        ""s2_cell_id"": {s2CellId}
    },
    ""criteria"":[
        {
            ""gameObjectType"": 1,
            ""filter"": {
                ""maxLocationCount"": 5,
                ""spacing"": {
                    ""minSpacingMeters"": 300
                }
            }
        },
        {
            ""gameObjectType"": 2,
            ""filter"": {
                ""maxLocationCount"": 20,
                ""spacing"": {
                    ""minSpacingMeters"": 150
                }
            }
        },
        {
            ""gameObjectType"": 3,
            ""filter"": {
                ""maxLocationCount"": 50,
                ""spacing"": {
                    ""minSpacingMeters"": 50
                }
            }
        }
    ]
}";
        }

        private GetS2CellResult GetFromRedisCache(ulong s2CellId)
        {
            // TODO [bgish]: Actually call redis instead of my fake in memory dictionary
            if (this.fakeRedisCache.TryGetValue(s2CellId, out byte[] data))
            {
                return DeserializeS2CellResult(data);
            }

            return null;
        }

        private void SaveToRedisCache(GetS2CellResult s2CellResult)
        {
            if (s2CellResult == null)
            {
                return;
            }

            // TODO [bgish]: Actaully make call to redis and set the keys TTL instead of using my fake redis dictionary
            this.fakeRedisCache.AddOrOverwrite(s2CellResult.S2CellId, SerializeGetS2CellResult(s2CellResult));
        }

        private GetS2CellResult GetFromDatabase(ulong s2CellId)
        {
            // TODO [bgish]: Implement
            return null;
        }

        private bool SaveToDatabase(GetS2CellResult result)
        {
            //// TODO [bgish]: This needs to save to CosmosDB, and needs to return true if
            ////               sucessful, and false if there is an etag mismatch.  My assumption
            ////               is that etag mismatches happen if two people try to insert something
            ////               new at the exact same time.  Shoudl throw an excpetion is there was
            ////               a databse write error so we can re-try.

            return true;
        }

        private static byte[] SerializeGetS2CellResult(GetS2CellResult result)
        {
            NetworkWriter writer = new NetworkWriter();

            writer.WritePackedUInt32((uint)result.ResultCode);
            writer.Write(result.S2CellId);
            writer.Write(result.ExpirationUtc.ToFileTimeUtc());

            if (result.Locations != null)
            {
                writer.WritePackedUInt32((uint)result.Locations.Count);

                for (int i = 0; i < result.Locations.Count; i++)
                {
                    writer.Write(result.Locations[i].LocationId);
                    writer.Write(result.Locations[i].TypeId);
                    writer.Write(result.Locations[i].LatLong.Latitude);
                    writer.Write(result.Locations[i].LatLong.Longitude);
                }
            }
            else
            {
                writer.WritePackedUInt32(0);
            }

            return writer.ToArray();
        }

        private static GetS2CellResult DeserializeS2CellResult(byte[] data)
        {
            GetS2CellResult result = new GetS2CellResult();
            NetworkReader reader = new NetworkReader(data);

            result.ResultCode = (GetS2CellResult.GetS2CellResultCode)reader.ReadPackedUInt32();
            result.S2CellId = reader.ReadUInt64();
            result.ExpirationUtc = DateTime.FromFileTimeUtc(reader.ReadInt64());

            int locationCount = (int)reader.ReadPackedUInt32();
            result.Locations = new List<LBELocation>(locationCount);

            for (int i = 0; i < locationCount; i++)
            {
                result.Locations.Add(new LBELocation
                {
                    LocationId = reader.ReadString(),
                    TypeId = reader.ReadString(),
                    LatLong = new GPSLatLong
                    {
                        Latitude = reader.ReadDouble(),
                        Longitude = reader.ReadDouble(),
                    },
                });
            }

            return result;
        }

        private static TimeSpan ParseTTL(string ttl)
        {
            if (string.IsNullOrEmpty(ttl) == false)
            {
                char durationType = ttl[ttl.Length - 1];
                string duration = ttl.Substring(0, ttl.Length - 1);

                // Seconds
                if ((durationType == 's' || durationType == 'S') && long.TryParse(duration, out long seconds))
                {
                    return TimeSpan.FromSeconds(seconds);
                }
            }

            Debug.LogError($"Unable to Parse TTL {ttl}");
            return TimeSpan.FromDays(1);
        }

        [Serializable]
        public class PlayablesResponse
        {
            [JsonProperty("locationsPerGameObjectType")]
            public Dictionary<string, LocationCollection> LocationsPerGameObjectType { get; set; }

            [JsonProperty("ttl")]
            public string TTL { get; set; }

            public class LocationCollection
            {
                [JsonProperty("locations")]
                public List<Location> Locations { get; set; }
            }

            public class Location
            {
                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("placeId")]
                public string PlaceId { get; set; }

                [JsonProperty("centerPoint")]
                public CenterPoint CenterPoint { get; set; }

                [JsonProperty("snappedPoint")]
                public CenterPoint SnappedPoint { get; set; }

                [JsonProperty("plus_code")]
                public string PlusCode { get; set; }
            }

            public class CenterPoint
            {
                [JsonProperty("latitude")]
                public double Latitude { get; set; }

                [JsonProperty("longitude")]
                public double Longitude { get; set; }
            }
        }
    }
}

#endif
