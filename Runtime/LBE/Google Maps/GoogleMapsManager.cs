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
    using Google.Maps.Event;
    using Google.Maps.Feature.Style.Settings;
    using UnityEngine;

    [RequireComponent(typeof(MapsService))]
    public class GoogleMapsManager : Manager<GoogleMapsManager>
    {
        #pragma warning disable 0649
        [Header("Stylings")]
        [SerializeField] private RegionStyleSettings regionStyleSettings;
        [SerializeField] private SegmentStyleSettings roadStyleSettings;
        [SerializeField] private AreaWaterStyleSettings waterStyleSettings;
        [SerializeField] private ExtrudedStructureStyleSettings extrudedStructureStyleSettings;
        [SerializeField] private ModeledStructureStyleSettings modeledStructureStyleSettings;

        [Header("Loading")]
        [SerializeField] private float loadRadiusInMeters = 300.0f;
        [SerializeField] private float reloadDistanceInMeters = 100.0f;

        [Header("Debug")]
        [SerializeField] private bool printDebugOutput;

        [HideInInspector]
        [SerializeField] private MapsService mapsService;
        #pragma warning restore 0649

        private GameObjectOptions gameObjectOptions;
        private bool isMapServiceLoaded;
        private bool isInitialized;

        // Used for loading
        private GPSLatLong currentLatLong;
        private GPSLatLong lastLoadLatLng;

        public bool IsMapLoaded => this.isMapServiceLoaded;

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        public void UnloadMap()
        {
            if (Platform.IsApplicationQuitting)
            {
                return;
            }

            if (this.mapsService.GameObjectManager != null)
            {
                this.mapsService.GameObjectManager.DestroyAll();
            }

            foreach (Transform child in this.mapsService.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            this.isMapServiceLoaded = false;
        }

        public void ReloadMap()
        {
            if (this.isInitialized == false)
            {
                Debug.LogError($"{nameof(GoogleMapsManager)} failed to ReloapMap, google maps must be initialized before you can reload it.");
                return;
            }

            this.UnloadMap();
            this.LoadMap();
        }

        public Vector3 GetPosition(GPSLatLong latLong)
        {
            return this.mapsService.Projection.FromLatLngToVector3(new LatLng(latLong.Latitude, latLong.Longitude));
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.mapsService);
        }

        protected override void Awake()
        {
            base.Awake();
            this.OnValidate();
        }

        private void Start()
        {
            // NOTE [bgish]: This must happen in Start, if we received GPS data in the Awake MapService won't be initialized and will throw errors
            GPSPositionManager.OnInitialized += () =>
            {
                GPSPositionManager.Instance.OnGPSChanged += this.OnGPSReceived;
            };
        }

        private void OnGPSReceived(GPSLatLong latLong)
        {
            if (this.isInitialized == false)
            {
                this.isInitialized = true;
                this.InitializeMap(latLong);
                return;
            }

            // Keeping tracking of the current LatLong
            this.currentLatLong = latLong;

            // Tell the map service of our new location
            if (this.isMapServiceLoaded)
            {
                this.mapsService.MoveFloatingOrigin(new LatLng(this.currentLatLong.Latitude, this.currentLatLong.Longitude));

                // Checking out if we need to reload maps content
                var lastLoadDistance = GPSUtil.DistanceInMeters(this.lastLoadLatLng, this.currentLatLong);

                if (lastLoadDistance > this.reloadDistanceInMeters)
                {
                    if (this.printDebugOutput)
                    {
                        Debug.Log("GoogleMapsManager Reloading Map");
                    }

                    this.LoadMap();
                }
            }
        }

        private void InitializeMap(GPSLatLong latLong)
        {
            if (this.printDebugOutput)
            {
                Debug.Log($"GoogleMapsManager Initializing Map To ({latLong})");
            }

            this.currentLatLong = latLong;

            // Registering for error handling (Taken from BaseMapLoader.cs)
            this.mapsService.Events.MapEvents.LoadError.AddListener(args =>
            {
                switch (args.DetailedErrorCode)
                {
                    case MapLoadErrorArgs.DetailedErrorEnum.NetworkError:
                        {
                            // Handle errors caused by a lack of internet connectivity (or other network problems).
                            if (Application.internetReachability == NetworkReachability.NotReachable)
                            {
                                Debug.LogError("The Maps SDK for Unity must have internet access in order to run.");
                            }
                            else
                            {
                                Debug.LogErrorFormat(
                                    "The Maps SDK for Unity was not able to get a HTTP response after " +
                                    "{0} attempts.\nThis suggests an issue with the network, or with the " +
                                    "online Semantic Tile API, or that the request exceeded its deadline  " +
                                    "(consider using MapLoadErrorArgs.TimeoutSeconds).\n{1}",
                                    args.Attempts,
                                    string.IsNullOrEmpty(args.Message)
                                    ? string.Concat("Specific error message received: ", args.Message)
                                    : "No error message received.");
                            }

                            return;
                        }

                    case MapLoadErrorArgs.DetailedErrorEnum.UnsupportedClientVersion:
                        {
                            Debug.LogError(
                                "The specific version of the Maps SDK for Unity being used is no longer " +
                                "supported (possibly in combination with the specific API key used).");

                            return;
                        }
                }

                // For all other types of errors, just show the given error message, as this should describe
                // the specific nature of the problem.
                Debug.LogError(args.Message);

                // Note that the Maps SDK for Unity will automatically retry failed attempts, unless
                // args.Retry is specifically set to false during this callback.
            });

            // Set real-world location to load
            this.mapsService.InitFloatingOrigin(new LatLng(latLong.Latitude, latLong.Longitude));

            // Configure Map Styling
            this.gameObjectOptions = this.GetMapStyle();

            this.LoadMap();
        }

        private void LoadMap()
        {
            this.lastLoadLatLng = this.currentLatLong;

            // Load map with default options
            this.mapsService
                .MakeMapLoadRegion()
                .AddCircle(Vector3.zero, this.loadRadiusInMeters)
                .Load(this.gameObjectOptions);

            // Adding flags so we know when the map is loading, and has finished being loaded
            this.mapsService.Events.MapEvents.Loaded.AddListener(this.MapLoadFinished);

            // If the serverice is already loaded, then probably have things to unload
            if (this.isMapServiceLoaded)
            {
                this.ExecuteDelayed(1.0f, this.UnloadOutside);
            }
        }

        private void MapLoadFinished(MapLoadedArgs args)
        {
            if (this.printDebugOutput)
            {
                Debug.Log($"GoogleMapsManager MapLoaded Complete");
            }

            this.isMapServiceLoaded = true;
            this.mapsService.Events.MapEvents.Loaded.RemoveListener(this.MapLoadFinished);
        }

        private void UnloadOutside()
        {
            this.mapsService
                .MakeMapLoadRegion()
                .AddCircle(Vector3.zero, this.loadRadiusInMeters)
                .UnloadOutside();
        }

        private GameObjectOptions GetMapStyle()
        {
            var options = new GameObjectOptions();
            options.RegionStyle = this.regionStyleSettings.Apply(options.RegionStyle);
            options.SegmentStyle = this.roadStyleSettings.Apply(options.SegmentStyle);
            options.AreaWaterStyle = this.waterStyleSettings.Apply(options.AreaWaterStyle);
            options.ExtrudedStructureStyle = this.extrudedStructureStyleSettings.Apply(options.ExtrudedStructureStyle);
            options.ModeledStructureStyle = this.modeledStructureStyleSettings.Apply(options.ModeledStructureStyle);

            return options;
        }
    }
}

#endif
