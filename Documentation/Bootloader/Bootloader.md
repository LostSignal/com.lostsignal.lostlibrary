
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
  
* ------------------------------------------------------------------------------------------------------------------

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





# Lost Library - Bootloader 
--------------------------------------------------------------------

* Can I remake the Dialog manager to not care what type of Dialog it is, that way it can work with UGui dialogs as well as UIElements dialogs

* Bootstraper
  * PlayFab
  * Lost Debug Menu
  * Tiinoo
  * Ads
  * Analytics
  * Logging
  * Limit Screen Size
  
--------------------------

* Lost Settings
  * bool Enforce Line Endings
  * Line Endings Dropdown
  * Override Template Files
    * MonoBehaviour
    * Playable Asset
    * Etc...
  * Perforce Info
    * Server
    * Workspace
    * Username
    * Password
  * Add .p4ignore / .gitignore / .collabignore
  * Add .editorconfig
  * Lost Defines

* Build Time Only
  * Uplaod Addressables to Azure
  * Perforce (Use Commit for Build Number)
  * Android Keystore
  * iOS Provisioning
  * Build in Strict Mode
  * Run Tests (fail build if fail?)
  * iOS (Disable Bitcode / Push Notifications)
  * Copy CloudBuild DLL to Streaming Assets
  * Is Developement Build
  * Upload to Google Play/Test Flight/Etc
  * Upload BootloaderInfo
    * Write Bootloader URL to resources
    * Uplaods current Code/Data veriosn as well as latest store urls and whether or not to force user to upgrade to latest store version.

* Dialogs that must be built into app
  * MessageBox
  * DebugMenu

* Anonymous Function (Send App Version and Data Version and Platform/Config)
  * How do we upload App/Data version so we know what to send when we call the anonymous funcion?
  * Returns Addressables Location
  * Returns PlayFab Title Id, CatalogVersion, Cloudscript Revision, etc.
  * Or Returns FORCE UPDATE with link to the store
  * Should Cache this in case we dont' have internet (but only if Allow Offline Play is checked)

* AppConfig
  * Bootloader URL


GET RIDE OF TITLE DATA EDITING STUFF, JUST HAVE A SCRIPTABLE OBJECT WITH IData INTERFACE

DEPENDS ON ADDRESSABLES AND PLAYFAB

Analytics
Logging
DebugMenu / Tiinoo
DialogManager?

Log Into PlayFab
Get Bootloader (Send Version, Get Back Bootloader)

  SpriteAtlasManger
  Initialize Ads
  Initialize IAP
  Initialize Addressables
  Initialize Push Notification
  Initialize Local Notifications
  Initialize OneSignal
  Get Custom Data
  Localization
  What's New
  CatalogVersion / CloudScript Revision / Timers
  
https://docs.unity3d.com/2018.3/Documentation/ScriptReference/SettingsProvider.html
Custom Settings Provider
JsonSerialization Util (all unity types and abilty to serailize AddressablesReference)


Bootloader (Pre Login)
  

Show Loading Background / Progress Bar
Log Into PlayFab (Ananymous / Signup / Twitch / Etc)
Send App Version and Get BootLoader



### Bootloader

GetBootloader(string bootloaderEndpoint)
  Calls an Anonymous Azure Function (passing in platform, config, app version and data version)
  Returns the bootloader json
  It should cache the last bootloader returned so you don't always have to have an internet connection
    * Maybe that's a configuration option

Has a background image and progress bar
Has a built in LocalizationTable for updating the progress bar text ("Downloading data blah or blah")

Bootloader has the Main Camera and I gameobject that stores the background image with progress bar
Once loading has finished it destorys that object and creates the dialogs needed and shows them?

How does forcing user to upgrade look like?


### Bootstrapper

Manditory
  PlayFabClientSDK
  PlayFabAllSDK
  PlayFabUtil class that registers running playfab aysnc tasks on background thread when in editor
  Addressables
  TextMesh Pro
  Unity Localization
  Json.Net

Optional
  UnityIAP (maybe?)
  Firebase
  com.unity.mobile.notifications
  Facebook SDK
  OneSignal
  SignalR

Initial Package install
  AppConfig System (Dev and Prod)
  PlayFabSDK(s)
  PlayFabServerlessUtilityCode (With Anonymous GetBootloader function)
  Localization Table (for handling PlayFab errors)
  Basic Audio System (with button clicks and dialogs showing/hiding noises, app background music)
  Addressable Asset Settings
  Bootloader scene
  Dialogs:
    MessageBox
    DebugMenu
    PurchaseItem
    StringInputBox
    SettingsMenu (with Audio On/Off, Language Switching)
    LogInDialog
    SignUpDialog
    WhatsNewDialog
    UpdateAppDialog
    PushNotificationsDialog (for showing notifications received when app was open)

// Definitely need Json.Net custom converters for Color, Vector1/2/3/4, etc
//   * Also all LazyAsset or AssetReference types

// Need a better way of turning on defines
//  * They should be menu items instead (Lost/Defines/Using Firebase/Facebook/Etc)

// CAN WE SAVE THE BOOTSTRAPPER IN PLAYFAB!?!?!?!

// WHAT DO WE DO ABOUT FORCING APP

// HOW DO WE HANDLE (IN EDITOR) INCREMENTING APP AND DATA VERSIONS AND MAKING SURE OLD VERIONS GET THEIR IsVersionOutOfDate flag set

App Version
Data Version

App Store Manager
  bool IsVersionOutOfData;  // Same as ForceUserToUpdate
  Links for every Unity IAP Store Type
    * iOS, Mac, Google Play, Universal Windows Platform, Amazon, 
    * Samsung Galaxy Apps, Tizen, CloudMoolah, Xiaomi Mi Game Pay

Notification Manager (USING_SIGNALR)
  string ConnectionString;
  SetTopics(list<sting>);
  TopicReceivedEvent

CustomData
  Key -> Value
  Key -> Value
  Key -> Value
  Key -> Value
  Key -> Value

Addressables Manager
  String Addressables Location
  List<ILazy> PreLoadAtStartup

LocalizationManager
   string defaultLanguage;
   List<string> languages;
   LazyLocalizationTable

Whats New Manager (Should this just live as a PlayFab scriptable object?)
   List of Versions and Features
   If user has never seen a new version, then show the popup and make sure they dont' see it again

PlayFab
    CloudScript Revision
    Catalog Version
    List<TimerDefinitions> Timers;  // Timer (Start Value, Min, Max, Hours Till Next Incremeant) - Uploaded to title data on build
    TitleData [ScriptableObject] [Type] [Compress] [Key] [Version] [Cache At Startup]  (On Build, these should upload to playfab title data)
    TitleData [ScriptableObject] [Type] [Compress] [Key] [Version] [Cache At Startup]  (On Build, these should upload to playfab title data)
    TitleData [ScriptableObject] [Type] [Compress] [Key] [Version] [Cache At Startup]  (On Build, these should upload to playfab title data)
    TitleData [ScriptableObject] [Type] [Compress] [Key] [Version] [Cache At Startup]  (On Build, these should upload to playfab title data)
    
    [LoginMethod]
    
    List<string> TitleDataToLoadAtStartup;
    List<string> UserDataToLoadAtStartup;
    
    // IAP Catalog - Uploaded to PlayFab on Build?

    GetTimer(ID)
    GetTitleData<Type>();
    GetTitleData(string titleDataKey);  // When reads string from playfab, will detect if it's serialized or not and do the thing

Unity IAP
    bool InitializeStoreAtStartup (Requires IAP Catalog Defined in PlayFab)
    [Shows error if Unity IAP is not installed]

Android Push Notifications [Enabled]
  string Firebase Key
  bool RegisterOnStartup
  bool RegisterWithPlayFab
  [Shows error if USING_FIREBASE define is not enabled]

iOS Push Notifications [Enabled]
  bool RegisterOnStartup
  bool RegisterWithPlayFab

Local Notifications Manager [Enabled]
  Abstraction for making local Notifications
  [Show error if unity push notifications plugin is not installed]

Push Notifications Manager
  bool ShowPopupWhenAppIsRunning;
  Event for catching them

OneSignal Manager
  string project ID
  [Shows error if USING_ONESIGNAL define is not enabled]

Tiinoo [Enabled]
  Settings for how to Bring it up
  bool DevelopmentBuildsOnly
  [Shows error if USING_TIINOO define is not enabled]

Debug Menu [Enabled]
  Settings for how to Bring it up
  bool DevelopmentBuildsOnly
  [Shows error if USING_UNITY_ADS define is not enabled]

Ads Manager
  Apple App Store ID
  Google Play Store ID

Analytics Manager
  Register PlayFab Provider
  Register Unity Provider
  [Shows error if USING_UNITY_ANALYTICS define is not enabled]

Dialog Manager
  // Maybe specify Dialog Types (Overlay)
  // Also plane distance and layering
  // Can use the order here to set OrderInLayer
  [LazyDialog] [Type] [Name] [Pre-load At Startup]
  [LazyDialog] [Type] [Name] [Pre-load At Startup]
  [LazyDialog] [Type] [Name] [Pre-load At Startup]
  [LazyDialog] [Type] [Name] [Pre-load At Startup]

SpriteAtlasManager [Enabled]
  List of atlases to load at Startup
  List of all atlases that should be loaded through Addressables

Logging Manager (Maybe this should stay as an AppConfig setting)
  Forward as Analytics

