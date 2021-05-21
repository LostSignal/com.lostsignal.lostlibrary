

* Make a custom mesh importer that looks for sub meshes that have a name that ends in _collision_mesh
  and adds a CollisionMesh component to it.
  * _collision_box
  * _collision_sphere
  * etc...
  * Also make sure it doesn't have a mesh renderer

* New Project Setup Tool
  * [ ] Add .editorconfig
  * [ ] Add .p4ignore, ignore.conf, .gitignore or something else?
  * [ ] Create Game Server Project Generator
  * [ ] Create Azure Functions Project Generator
  * [ ] Use StyleCop?
    * **How do we even do this?  2020.2+ Only

* Remove Everything Ably (Going to use new Azure Web Pub/Sub instead)

* Finish AppConfig -> BuildConfig renaming (make sure it just works with older projects)
  * appconfig was renamed to bootconfig, make sure source control (and template files) ignore it
    * add bootconfig and releaseconfig while we're at it
    * Also Add the following to to the ignore files
      * "Assets/AddressableAssetsData/*"

* Add Visual Scripting to Default git, p4 and plastic ignore files
  * **/Assets/Unity.VisualScripting.Generated/*
  * **/Assets/Unity.VisualScripting.Generated
  * **/Assets/Unity.VisualScripting.Generated.meta

* Update Bootloader system to have new BootConfig
  * Should also contain current release info
  * Clean out Release/AppVersion code when figured out
  * Make sure it supports Reboot

* Add UpdateManager
  * Remove UnityTask
  * Remove CoroutineRunner

* Add com.unity.coding libary as a dependency of lost libary

* Make a SafeAreaManager (will pre-built in safe area's for most popular iPhones)

* LostPlayerPrefs needs a refactor
  * DeviceStorage
    * App Open Count
    * Player Custom Ids
  * PlayerStorage
    * Stats
    * Achievements
  * GameStorage 
    * Flag System
    * Checkpoint info?  Saving and restoring to older version?
    * CreateSnapshot()
    * Rollback() (used by checkpoint system)
  * Add Reset File options for each type
  * Can DeviceStorage be reused for other systems like Analytics/Logging/PerfRecording Data, so we can save all that before sending up to the cloud?

* FlagManager System (how will it work with the Player/Game Storage System?)

* **CLEAN UP HAVEN CODE (remove all of the unity example projects code too)
  * Update XR Dialog Follow Code to:
    * https://forum.unity.com/threads/quaternion-smoothdamp.793533/
    * https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b

* Turn Gameplay/Expereince/Experience.cs into "Level.cs"?
  * Basically a Level should be a scriptable object with a Main scene and chunck scenes.
  * Maybe even have a list of "Startup" scenes that should also be loaded
  * This way when you load the level, we know exactly how much to load
  * The LevelManager should handle all of these
  * Objects should also be able to tell the LevelManager that they are "Busy" doing work and the level manager won't fade up till that work is done.

* **PLAYFAB
  * Make PlayFab an optional dependency (so wrap all playfab code around USING_PLAYFAB)
  * Look at the WIP Folder for any CloudFunctions related code and move it into Cloud Funtions or delete it
  * Update all PlayFab Managers to use Async instead of UnityTask
  * Try to get PlayFab code (specially the caching) working in an outside C# project

* Make sure all cool Lost UGUI scripts have filters to Lost folder with AddComponentMenu

* Clean up any Behaviour code that I think shouldn't exist anymore (Maybe add ```[AddComponent("")]```)

* Add ScriptableObject Event System?
  * https://github.com/roboryantron/Unite2017
  * Events
    * On PlayerData Loaded
    * On PlayerData Before Save
    * On PlayerData After Save
    * On GameData Loaded
    * On GameData Before Save
    * On GameData After Save
    * On Checkpoint Snapshot
    * On Checkpoint Restore
    * On Player Death
    * On Object Pooled
  
  * Make sure Bolt Graphs can listen for these

* Remove WeakReference out of Experimental, shoudl be replaced with GuidBasedReferences

* Add Searchable Enum Property Drawer
  * https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SearchableEnum/Code/Editor/SearchableEnumDrawer.cs
  * https://github.com/fishtopher/UnityDrawers
  

* DissonanceManager should print error if "Microphone Usage Description" is empty on iOS

* LiveSwitchManger should print error if "Camera Usage Description" is empty on iOS

* Local Notifications and Push Notification managers
  * Local Notifications Manager (based on Unity's wrapper)
  * Lost.PlayFab should look for a PushNotifications manager if USING_LOST_PUSH_NOTIFICATIONS is on
  * Register for is ready and send up RegisterForIOSPushNotification/RegisterForAndroidPushNotification


* SpriteAtlasLoadingManger custom editor has been broken
* UnityPurchasingManager needs to have a "Is Enable" settings, and print errors if not properly installed
* Add XRManagerSettings (use this instead of IsXRApp)
* Make Settings for PlayFabAnalyticsProvider
* Is Lost Library Codebase completely static instance free?  Can I safely reboot an app without messing this up?
* What about Pooler system.  Should probably move that to Releases
  * Also, how good am I about using the Pooling system in the app?
* Get 2FA Login working (will require some anonymous functions)
  * Will need to define these in Releases?
* Update Reference Tool to figure out the exact location of the bad reference
* **MAKE SURE TextObjectEditor SETS IT DIRTY WHEN IT'S SUPPOSE TO
* Make sure BuildConfigs, GameServerGenerator, AzureFunctionsGenerator and BootConfig are 
  all autogenerated and registered with EditorBuildSettings
    * EditorBuildSettings.AddConfigObject(Namespace, editorAppConfig, true);

* Pooling System needs to use the new Manager System
  * Delete SingletonUtil.cs after the move is complete
  * Need to detect if a pooled object is being destroyed?
  * If it's not being destroyed during app shutdown then print an error (Platform.IsShuttingDown)?
  * Would it make more sense to not put a Pooled component on the object and instead keep a HashSet of Instance Ids?


* **MAKE LAYOUT GROUP DISABLER COMPONENT (MAYBE ALL DIALOGS HAVE THIS AS A REQUIRED COMPONENT?)
  * This scans your dialog for all layout groups and waits x seconds before disabling them after a show

* GameObjectState Component (Move, Scale, Rotate, Tint), make networkable
    * Animator ones should have a flag, disable Animator when finished (defaulted to true)
    * Add Showable exist in the project?
      * Will it use ObjectState component with Show/Hide under the hood?

* Scan all code bad line endings and print warnings if find files with bad line endings
  or messed up line endings "\r\r\n"

* Can I make a tool to populating the Game window resolutions?


  * Find All Invalid Raycast Targets
  * Generate Default Best Practices Presets
  * Remove Empty Directories
  * Fix Line Endings
  * Validate Scene
  * Validate Game



### CachedUrlImage Utility
---------------------------------
* Create a CachedUrlImage class.  Takes a URL, has a loading animation, and
  caches the result for later. It will most likely be used for profile pictures.


  * URL To Image Util (Does this actually work?)
  ```
  var url = "http://URL to image...";
  var tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
  var www = new WWW(url);
  www.LoadImageIntoTexure(tex);
  spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height, new Vector2(0.5f, 0.5f), 100.0f);
  ```
  * Texture2D.Compress - https://docs.unity3d.com/ScriptReference/Texture2D.Compress.html
  * https://docs.unity3d.com/ScriptReference/Texture2D.LoadRawTextureData.html



### AppSettings Improvements
-----------------------------------
* Make static App Class that tracks login count and last login version
  * Make RateMe Dialog
  * Make What's New

* Would it be worth it to investigate some sort of visual scripting that's downloaded on startup?
  * One use case would be CheckIfShouldShowRateMe().  We may want to change that logic after ship,
    maybe it's when you enter a new PlayFab segment, maybe it's when you just opened your first
    briefcase?
  * It could tap into the analyics system to figure this out.  "OpenBriefcase" event, happened within
    1 minute of "Visit MainMenu" event.

* Have runtime App class that loads the config.json and has useful things like:
  * First time running app
  * App.ShowRateMe();  (Makes sure to check PlayerPrefs for "Never Show This Again")
  * App.ShowWhatsNew(); (Only show's it if FirstTimeRunningApp == false, and FirstTimeRunningThisVersion == true)
  * App.NumberOfTimesOpened;

* Update to use new Editor Config stuff

* Look at Unity source and figure out how to read/write to the ProjectSettings folder
  * Would be nice to make sure that SourceControlProvider works with this too
* If I store AppSettings in ProjectSettings (as json), can I load it in "InitializeOnLoad"?
* Unity Cloud build should just work with this BuildConfiguration object
* Add CommandLine argument for -BuildConfig=Dev
* Should be able to switch configurations from the file menu
  * Create a IAssetBundleSource class
    * Have a default StreamingAssetsSource
    * Have a default S3Source
    * Have a default AzureBlobSource
    * Have a default PlayFabSource
    * MoveAssetBunde(string absoluteFilePath, string assetPath)
    * GetAssetBundle(string assetPath)
    * Definitely make sure my S3 uploader jar only has packages it needs (should be much much smaller)
* Does AppSettings show warnings if meta files aren't visible and assets aren't forced to text?
* Maybe have AppSettings.Orientation set the differnet modes for you?
* Add Cache Server setting to App class.  Can i specify a different IP for every version of Unity that has loaded the project?
* Make Disable BitCode code only run on iOS (no point in showing it on Android)
  * Move it to Platform class?
* bool AppSettings.IsFirstTimeRunningApp
* bool AppSettings.StartupCount
* bool AppSettings.isApplicationQutting?
* Print Warnings if "Force Text" or "Visible Meta Files" is off
* Add P4 Server Url and set for user (Not sure how to do this yet)
* Put Bundle Identifier in Project Settings
* Fix for AppSettings being cached in editor?  Portrait doesn't update correctly because of it
  * Possibly use AssetDatabase.LoadAssetAtPath instead of Resources.Load for accessing AppSettings in the editor.
  * EditorUtility.SetDirty(globals);
  * AssetDatabase.StopAssetEditing();
  * AssetDatabase.SaveAssets();
* Add Development Mode to AppSettings
  * EditorUserBuildSettings.development = true or false
* Print Warning if "Force Text Serialization" is off
* Print Warning if "Visible Meta Files" is off
* Set the editor playmode tint for you!
* Detect AndroidManifest to make sure bundle identifier is set corrctly

* If user runs editor and there is no .p4config file, then prompt them to create one
  * A sample P4CONFIG file might contain the following lines:
    * P4CLIENT=joes_client
    * P4USER=joe
    * P4PORT=ssl:ida:3548
    * P4IGNORE=.p4ignore
  * If this file exists, then make sure to set above P4 editor settings to those values

* Ability to specify Cache Server (Based on Unity Version) - Have it set at startup
  * EditorPrefs.SetBool("CacheServerEnabled", true);
  * EditorPrefs.SetString("CacheServerIPAddress", IPaddr + ":" + portNum);
  * If (!UnityEditorInternal.InternalEditorUtility.CanConnectToCacheServer())

* AssetBundles
  * In the AssetBundle section of AppSettings.  There should be a sectoin of 
    all the asset bundles in the project with check marks next to them.  All 
    the ones you check will be put in the StreamingAssets durring the build 
    process.




### Global PostProcessor
------------------------------
* Editor
  * Presets
    * Defaults
      * Mesh.preset, CanvasRender.preset, TexturePreset.preset, etc
    * Sprite.preset
    * UISprite.preset

* Create Best Practices Default Presets

* Lost -> Presets -> Generate Default Best Practices Presets
  * CanvasRenderer (Cull Transparent Mesh = true)
  * Image (Raycast Target = false)
  * Text (Raycast Target = false)
  * TextMeshPro_UGUI (Raycast Target = false)
  * See below for more

* Asset Import Hierachy System
  * Make it a folder editor (stored in folder meta file)
  * Make sure same tech works for build configuration system)
  * Rig -> Optimize Game Object (true)
  * Particle System -> withChildren (false)
    * If you do have multiple particle system children, make component 
      that will cache them on Awake and then call Start/Stop on the list

* Would Presets help with this?
* Directory Hierarchy (with inheritence?)
  * Scriptable object at the root of the project or store info in directory meta file
* Setting asset bundle names and packing tags (using string format "{ParentDir}\Blah"
  * FileName, DirName, ParentDirName, ParentParentDirName
* Default models to not have animators
* Default models read/write to false
* Default rigs to Optimize Animation (which doesn't expose bones)
* Default optimizing
* Make asset pre-processor to make sure read/write is disabled for textures
  * also make sure texture are compressed
* If it's a mesh, and it doesn't have a MeshCollider, then turn off read/write for those too
* ParticleSystems have a "withChildren" attribute that defaults to true, should be false
  * Look up why ```https://www.youtube.com/watch?v=_wxitgdx-UI``` 4:00ish minutes in
* Meshes should turn off Blend shaps if don't use them (faster import time)
* Optimize Game Objects (True)
* Things to Edit
  * Turn off Rig if not needed
  * Turn off Animator if not needed
  * Animation Comppression
  * Model Compression
  * Texture Compression
  * Optimize Rig
  * Etc


