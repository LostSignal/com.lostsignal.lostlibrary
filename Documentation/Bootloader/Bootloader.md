
* Responsible for booting up all Managers
* There is a BootConfig scriptable object that lives in Resources 
  * You can move it whereever you like as long as it lives in a root level of a Resources folder with the same name
* Different Types of Boots
  * None
  * Resource Prefab
  * Scene Name
  * Addressables Prefab
  * Addressables Scene
  
* Bootload is always run at startup (unless you're running from the editor and your scene is in the IgnoreBootloader list in the bootconfig).
  
* -------------------

* BootConfig Class (Stored in Streaming Assets)
  * Boot Type?  (Resources Prefab, Addressables Prefab, Scene Name or Addressables Scene)
    * string BootPrefabResourceName;
    * LazyAsset BootPrefab;
    * string SceneName;
    * LazyScene BootScene;
  * bool DontShowLoadingInEditor
  * LazyScene BootFinishedScene
  * Has CurrentState Enum and Progress Float so Bootloader UI can function
  
* Bootloader Static Class
  * Load BootConfg from Resources
    * Load Boot Scene (if present and not already loaded)
  * Get Current Release (from resources or playfab)
    * Force App Update?
  * Fire Event "InitializeManagers"
    * Mangers can call ```Booloader.GetManagerConfig<T>()``` or ```Bootloader.CurrentRelease```
  * **Manager live in boot scene if specifed, else they live in DontDestoryOnLoad
  * Fire Event "BootComplete"
  * Manager all have a OnInitialize function though

* Boot Finished Scene
  * ManagerConfigs (List of ManagerConfig)
  * Load BootConfig from Streaming Assets
  * Load Boot Scene if not already
  * GetRuntimeConfig (Streaming Assets or PlayFab
  
* Bootloader Scene
  * Loading Screen
  * Loads BootConfig
    * GetRuntimeConfig (Streaming Assets or PlayFab)
      * Send up App Version, Data Version, Platform, Config Name
      * Gets back New Data Version, AssetBundle URL, ```List<RuntimeConfig> Configs```
    * Sets Addressabels Url
    * Cache's result so we don't call this every time (Editor only?)
    * Force App Update? Needs Dailog Manager
  * Performing App Update
    * Loads any "Always Loaded" scenes
    * Loads app Startup Scene
  * Initializing Mangers
    * Fires Event InitializeManagers (All manager register for this)
  * Event OnBootComplete (Once all managers have completed loading)
