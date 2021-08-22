//-----------------------------------------------------------------------
// <copyright file="LBEManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_GOOGLE_MAPS_SDK

namespace Lost.LBE
{
    using Google.Common.Geometry;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class LBEManager : Manager<LBEManager>
    {
#pragma warning disable 0649
        [SerializeField] private List<GameObjectTypeMapping> gameObjectTypes;
        [SerializeField] private float loadRadiusInMeters = 300.0f;
        [SerializeField] private float reloadDistanceInMeters = 15.0f;
        [SerializeField] private bool disableCaching;
#pragma warning restore 0649

        private Dictionary<ulong, S2Cell> s2CellsCache = new Dictionary<ulong, S2Cell>();
        private Dictionary<string, GameObject> locationIdToGameObject = new Dictionary<string, GameObject>();
        private Dictionary<string, LBELocation> locationIdToLBELocation = new Dictionary<string, LBELocation>();

        private GPSLatLong lastUpdateLatLong;
        private bool isInitialized;

        public override void Initialize()
        {
            this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return CreatePool();
                yield return GPSPositionManager.WaitForInitialization();
                yield return GoogleMapsManager.WaitForInitialization();

                this.SetInstance(this);

                GPSPositionManager.Instance.OnGPSChanged += this.OnGPSChanged;

                if (GPSPositionManager.Instance.HasReceivedGPSData)
                {
                    this.OnGPSChanged(GPSPositionManager.Instance.CurrentLatLong);
                }
            }

            IEnumerator CreatePool()
            {
                // TODO [bgish]: Pool all the necessary game objects over multiple frames
                yield break;
            }
        }

        private async void OnGPSChanged(GPSLatLong latLong)
        {
            if (this.isInitialized == false || GPSUtil.DistanceInMeters(this.lastUpdateLatLong, latLong) > this.reloadDistanceInMeters)
            {
                this.isInitialized = true;
                await this.UpdateLocation(latLong);
            }
        }

        private async Task UpdateLocation(GPSLatLong currentLatLong)
        {
            this.lastUpdateLatLong = currentLatLong;
            this.PruneExpiredAndOutOfRangeLocations(currentLatLong);
            await this.FindAndAddAllNewLocations(currentLatLong);

            // Since we moved, lets update the locations for all the lbe locations
            foreach (var location in this.locationIdToGameObject)
            {
                var locationId = location.Key;
                var lbeLocation = this.locationIdToLBELocation[locationId];
                location.Value.transform.localPosition = GoogleMapsManager.Instance.GetPosition(lbeLocation.LatLong);
            }
        }

        private void PruneExpiredAndOutOfRangeLocations(GPSLatLong currentLatLong)
        {
            var cachedS2CellsToRemove = new HashSet<ulong>();
            var utcNow = DateTime.UtcNow;

            foreach (var cell in this.s2CellsCache.Values)
            {
                bool isEntireCellExpired = cell.ExpirationUtc < utcNow;

                if (isEntireCellExpired)
                {
                    cachedS2CellsToRemove.Add(cell.S2CellId);
                }

                // Removing Locations
                foreach (var location in cell.Locations)
                {
                    if (isEntireCellExpired || this.IsLocationInLoadRange(location, currentLatLong) == false)
                    {
                        this.RemoveLocationFromMap(location);
                    }
                }
            }

            foreach (var s2CellId in cachedS2CellsToRemove)
            {
                this.s2CellsCache.Remove(s2CellId);
            }
        }

        private async Task FindAndAddAllNewLocations(GPSLatLong currentLatLong)
        {
            foreach (var s2CellId in this.GetCurrentS2CellIds(currentLatLong))
            {
                var s2Cell = await this.GetS2Cell(s2CellId.Id);

                foreach (var location in s2Cell.Locations)
                {
                    if (this.IsLocationInLoadRange(location, currentLatLong))
                    {
                        this.AddLocationToMap(location);
                    }
                }
            }
        }

        private async Task<S2Cell> GetS2Cell(ulong s2CellId)
        {
            if (this.s2CellsCache.TryGetValue(s2CellId, out S2Cell s2Cell) == false)
            {
                var getS2CellResult = await PlayablesManager.Instance.GetS2Cell(new GetS2CellRequest
                {
                    CurrentLatLong = GPSManager.Instance.CurrentRawLatLong,
                    S2CellId = s2CellId,
                });

                s2Cell = new S2Cell
                {
                    S2CellId = s2CellId,
                    Locations = getS2CellResult.Locations,
                    ExpirationUtc = DateTime.UtcNow.AddSeconds(getS2CellResult.SecondsToLive),
                };

                if (this.disableCaching == false)
                {
                    this.s2CellsCache.Add(s2Cell.S2CellId, s2Cell);
                }
            }

            return s2Cell;
        }

        private IEnumerable<S2CellId> GetCurrentS2CellIds(GPSLatLong currentLatLong)
        {
            // Info on S2 Geometry - https://s2geometry.io/
            // Code taken from this video - https://www.youtube.com/watch?v=UZsf3bqmmKs  (6:26)
            // Min/Max Level and Max Cells taken from here - https://s2.sidewalklabs.com/regioncoverer/
            // Using Nuget S2Geometry vs 1.0.3 (https://www.nuget.org/packages/S2Geometry/)
            S2RegionCoverer rc = new S2RegionCoverer();
            rc.MaxCells = S2RegionCoverer.DefaultMaxCells;
            rc.MinLevel = rc.MaxLevel = 14;

            // var southwestLat = new GPSLatLong { Latitude = 47.013859, Longitude = -122.920682 };
            // var northeast = new GPSLatLong { Latitude = 47.033532, Longitude = -122.894687 };
            //
            // S2LatLng low = S2LatLng.FromDegrees(southwestLat.Latitude, southwestLat.Longitude);
            // S2LatLng high = S2LatLng.FromDegrees(northeast.Latitude, northeast.Longitude);
            //
            // S2LatLngRect latLngRect = new S2LatLngRect(low, high);
            // return rc.GetCovering(latLngRect);


            //// https://stackoverflow.com/questions/7477003/calculating-new-longitude-latitude-from-old-n-meters
            //// number of km per degree = ~111km (111.32 in google maps, but range varies
            //// between 110.567km at the equator and 111.699km at the poles)
            //// 1km in degree = 1 / 111.32km = 0.0089
            //// 1m in degree = 0.0089 / 1000 = 0.0000089
            double meters = this.loadRadiusInMeters;
            double coef = meters * 0.0000089;
            double new_lat = /*currentLatLong.Latitude +*/ coef;
            double new_long = /*currentLatLong.Longitude +*/ coef / Math.Cos(currentLatLong.Latitude * (Math.PI / 180.0));

            S2LatLng center = S2LatLng.FromDegrees(currentLatLong.Latitude, currentLatLong.Longitude);
            S2LatLng size = S2LatLng.FromDegrees(new_lat, new_long);

            return rc.GetCovering(S2LatLngRect.FromCenterSize(center, size));
        }

        private bool IsLocationInLoadRange(LBELocation location, GPSLatLong currentLatLong)
        {
            return GPSUtil.DistanceInMeters(location.LatLong, currentLatLong) < this.loadRadiusInMeters;
        }

        private void AddLocationToMap(LBELocation location)
        {
            if (this.locationIdToGameObject.ContainsKey(location.LocationId) == false)
            {
                var locationGameObject = this.CreateGameObject(location);

                if (locationGameObject != null)
                {
                    this.locationIdToGameObject.Add(location.LocationId, locationGameObject);
                    this.locationIdToLBELocation.Add(location.LocationId, location);
                }
            }
        }

        private void RemoveLocationFromMap(LBELocation location)
        {
            if (this.locationIdToGameObject.TryGetValue(location.LocationId, out GameObject locationGameObject))
            {
                this.DestoryGameObject(locationGameObject);
                this.locationIdToGameObject.Remove(location.LocationId);
                this.locationIdToLBELocation.Remove(location.LocationId);
            }
        }

        private void DestoryGameObject(GameObject gameObject)
        {
            // TODO [bgish]: Disable and Pool it instead
            GameObject.Destroy(gameObject);
        }

        private GameObject CreateGameObject(LBELocation location)
        {
            if (this.gameObjectTypes != null)
            {
                for (int i = 0; i < this.gameObjectTypes.Count; i++)
                {
                    if (this.gameObjectTypes[i].GameObjectType == location.TypeId)
                    {
                        // TODO [bgish]: Get from a Pool instead of instantiating it (Unity now has a pooling API)
                        var newGameObject = GameObject.Instantiate(this.gameObjectTypes[i].Prefab);
                        newGameObject.transform.Reset();
                        newGameObject.transform.position = GoogleMapsManager.Instance.GetPosition(location.LatLong);

                        return newGameObject;
                    }
                }
            }

            return null;
        }

        private class S2Cell
        {
            public ulong S2CellId { get; set; }

            public List<LBELocation> Locations { get; set; }

            public DateTime ExpirationUtc { get; set; }
        }

        [Serializable]
        private class GameObjectTypeMapping
        {
#pragma warning disable 0649
            [SerializeField] private string gameObjectType;
            [SerializeField] private GameObject prefab;
#pragma warning restore 0649

            public string GameObjectType => this.gameObjectType;

            public GameObject Prefab => this.prefab;
        }
    }
}

#endif
