//-----------------------------------------------------------------------
// <copyright file="GoogleMapsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_GOOGLE_MAPS_SDK

namespace Lost.LBE
{ 
    using Google.Maps;
    using Google.Maps.Coord;
    using Google.Maps.Feature.Style.Settings;
    using UnityEngine;

    //// Use this video for GPS info - https://www.youtube.com/watch?v=MMrZfjlOcTU (2ish min in)
    ////   Check out BaseMapLoader.cs for how to load maps.
    ////   DynamicMapsUpdater handles the map updating as you move.
    ////   MapsService.Coords.FromVector3ToLatLong
    ////   MapsService.Coords.FromLatLongToVector3

    [RequireComponent(typeof(MapsService))]
    public class GoogleMapsManager : MonoBehaviour 
    {
        #pragma warning disable 0649
        [Header("Stylings")]
        [SerializeField] private RegionStyleSettings regionStyleSettings;
        [SerializeField] private SegmentStyleSettings roadStyleSettings;
        [SerializeField] private AreaWaterStyleSettings waterStyleSettings;
        [SerializeField] private ExtrudedStructureStyleSettings extrudedStructureStyleSettings;
        [SerializeField] private ModeledStructureStyleSettings modeledStructureStyleSettings;

        [Header("Loading")]
        [SerializeField] private float loadRange = 2000;

        [Header("Debug Initial Starting Location")]
        [SerializeField] private LatLng latLong = new LatLng(40.6892199, -74.044601);

        [HideInInspector]
        [SerializeField] private MapsService mapsService;
        #pragma warning restore 0649

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.mapsService);
        }

        private void Awake()
        {
            this.OnValidate();
        }

        private void Start()
        {
            // Set real-world location to load
            this.mapsService.InitFloatingOrigin(this.latLong);

            // Configure Map Styling
            var options = new GameObjectOptions();
            options.RegionStyle = this.regionStyleSettings.Apply(options.RegionStyle);
            options.SegmentStyle = this.roadStyleSettings.Apply(options.SegmentStyle);
            options.AreaWaterStyle = this.waterStyleSettings.Apply(options.AreaWaterStyle);
            options.ExtrudedStructureStyle = this.extrudedStructureStyleSettings.Apply(options.ExtrudedStructureStyle);
            options.ModeledStructureStyle = this.modeledStructureStyleSettings.Apply(options.ModeledStructureStyle);

            // Load map with default options
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one * this.loadRange);
            this.mapsService.LoadMap(bounds, options);
        }
    }
}

#endif
