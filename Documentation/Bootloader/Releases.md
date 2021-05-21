

**HOW DO WE HANDLE DATA VERSION!?!?!?!?
  * Every time we do a build, we append the Config/Changelist to the current release??
  * If there is a newer version, but my config/changlist is in a previous version, then
    we look for the "ForceUpdate" flag to see if we should force an update, or just load
    the old data?
    * Does every config/changlist entry have a different AssetBundle Url?  Or do we just
      assume it exists.  If so, how do older version use the new stuff?  Does it look for
      newer Configs/Changelist within the release and use those?
    * So whenever there is an Android build, it uploads asset bundles to a changelist number
      and overwrites the Android colum?  So 
    * So Releases have a Dictionary of Platform -> AssetBundle Changelist URL
      * So if make data only build, that gets overwritten
    * How do we update this if there are 2 android builds happening at the exact same time?
    * How does this work for local builds?
    * What's New stuff should be apart of releases, so perhaps it would make sense to mark 
      a point release as a Data Only build.  That way we can still show What's new info on 
      data only.
    * Should we have a Releases.json for every build type?  Dev/Live
      * https://lostsignalreleases.z22.web.core.windows.net/{machine_name}/{platform}/{config}/releases.json
        * for local builds, machine_name is the machine name, but for cloud builds it's "cloud_build"
        * Do we need to specify config?  
          * My guess is yes, because Dev could be behind Live
          * Do we need to have a "Code Version" that is baked into C#?
    * **How do we handle having a having people force update before it's live on the app store?
      * Do you make the new version, wait for it to appear in the store, then say ForceUpdate?
      * What does that look like?  Part of the build process, or do this from the editor?
      * Put a timestamp on the ForceUpdate?


**Bootstrapper should call ReleaseManger.GetCurrentRelease() when the app regains focus, and if it has
changed, then reboot the app.

* Releases System with Title Data Versioning!
  * Login -> Get Realease -> GetPlayerCombinedInfo
  * Releases object with Updates, Title Data Versioning, Config Variables
  * Upload on Build Relase Info and Asset Bundles on build
  * Azure Function for getting Current Release
  * Make a new PlayFabStartupCaching manager?
    * After Login/GetReleases it does the GetPlayerCombinedInfo

* Releases
  * List of Addressable Assets to load at startup
  * List of Title Data Keys to load at startup
  * Store URL
  * Force Update Logic
  * Addressables location
  * Data Version
  * App Version
  * SpriteAtlas References should probably live in releases as well, and SpriteAtlasLoadingManager should rely on ReleaseManager

* --------------------------------------------

* AppConfig
  * Needs Releases Config for storing blob storage stuff?
  * Definitely needs PlayFab Config 
  * **Also Releases has Asset Bundle Uploading?

* Releases
  * Azure Blob Credentials (or in AppConfig?)
  * Website URL (or in AppConfig?)
  * bool Upload AssetBundles (or in AppConfig?)
  * Upload()
    * Serializes List of Releases release to Json and puts in blob storage
    * Goes through all Title Data objects and uploads to PlayFab
    * if (UploadAssetBundles), build player content and upload
  * List of Releases
    * Release
      * Store URL
        * Android
          * Google Play => Blah
          * Samsung => Blah
        * iOS
          * Apple => Blah

      * PlayFab
        * List of PlayFab Title Data Objects
        * PlayFab Catalog Object (gets uploaded on build?)
        * MatchMaking Configs
          * "Default" => GameMode, BuildVersion, Regions, StartNewIfNoneFound
        * Anonymous Cloud Functions
          * Dev Config
            * RequestLogin
            * Login
          * Live Config
            * RequestLogin
            * Login


### What's New System
--------------------------
* AppVersion
   * int CloudScriptRevision;
   * string CatalogVersion;
   * bool DataOnlyBuild;
   * LocString Description;
   * LocString[] BulletPoints;
   * bool IsLive;
   * bool ForceToUpdate;
   * Add a Coming soon field?
   * AssetBundle Version?!?!?!?!?
   
* What's New System (apart of AppSettings?)
* Make a what's new Dialog (it can probably take care determining if it has shown that info yet)
