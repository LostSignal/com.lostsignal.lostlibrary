//-----------------------------------------------------------------------
// <copyright file="PlayablesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_GOOGLE_MAPS_SDK

namespace Lost.LBE
{
    using Google.Common.Geometry;
    using UnityEngine;

    ////
    //// To Do:
    ////  * Actually call out to the playable service and get some results
    ////  * Spawn prefabs based on locations returned from playables service
    ////  * Update this class to do some caching and pagination?
    ////
    public class PlayablesManager : Manager<PlayablesManager>
    {
        #pragma warning disable 0649
        [SerializeField] private string apiKey;
        #pragma warning restore 0649

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        private void Start()
        {
            this.PrintS2CellIds();
        }

        private void PrintS2CellIds()
        {
            // Info on S2 Geometry - https://s2geometry.io/
            // Code taken from this video - https://www.youtube.com/watch?v=UZsf3bqmmKs  (6:26)
            // Min/Max Level and Max Cells taken from here - https://s2.sidewalklabs.com/regioncoverer/
            // Using Nuget S2Geometry vs 1.0.3 (https://www.nuget.org/packages/S2Geometry/)
            S2RegionCoverer rc = new S2RegionCoverer();
            rc.MaxCells = S2RegionCoverer.DefaultMaxCells;
            rc.MinLevel = rc.MaxLevel = 14;
        
            var southwestLat = new GPSLatLong { Latitude = 47.013859, Longitude = -122.920682 };
            var northeast = new GPSLatLong { Latitude = 47.033532, Longitude = -122.894687 };

            S2LatLng low = S2LatLng.FromDegrees(southwestLat.Latitude, southwestLat.Longitude);
            S2LatLng high = S2LatLng.FromDegrees(northeast.Latitude, northeast.Longitude);

            S2LatLngRect latLngRect = new S2LatLngRect(low, high);

            S2CellUnion cellUnion = rc.GetCovering(latLngRect);

            foreach (var cellId in cellUnion)
            {
                Debug.Log(this.GetRequestBody(cellId.Id));
                Debug.Log(cellId.ToToken());
                break;
            }
        }

        private string GetRequestUrl() => "https://playablelocations.googleapis.com/v3:samplePlayableLocations?key=" + this.apiKey;

        private string GetRequestBody(ulong s2CellId)
        {
            string requestString = 
@"{
    ""area_filter"":{
        ""s2_cell_id"": {s2CellId}
    },
    ""criteria"":[
        {
            ""gameObjectType"": 1,
            ""filter"": {
                ""maxLocationCount"": 4,
                ""spacing"": {
                    ""minSpacingMeters"": 500
                }
            }
        },
        {
            ""gameObjectType"": 2,
            ""filter"": {
                ""maxLocationCount"": 4,
                ""spacing"": {
                    ""minSpacingMeters"": 400
                }
            }
        },
        {
            ""gameObjectType"": 3,
            ""filter"": {
                ""maxLocationCount"": 50,
                ""spacing"": {
                    ""minSpacingMeters"": 200
                }
            }
        }
    ]
}";

            return requestString.Replace("{s2CellId}", s2CellId.ToString());
        }
    }
}

#endif
