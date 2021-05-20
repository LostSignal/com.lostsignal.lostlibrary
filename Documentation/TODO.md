
* Add UpdateManager
* Remove UnityTask
* Remove CoroutineRunner

* LostPlayerPrefs needs a refactor
  * DeviceStorage
  * PlayerStorage
  * GameStorage (with CreateSnapshot() and Rollback() for checkpoint system)


* Turn Gameplay/Expereince/Experience.cs into "Level.cs"?
  * Basically a Level should be a scriptable object with a Main scene and chunck scenes.
  * Maybe even have a list of "Startup" scenes that should also be loaded
  * This way when you load the level, we know exactly how much to load
  * The LevelManager should handle all of these
  * Objects should also be able to tell the LevelManager that they are "Busy" doing work and the level manager won't fade up till that work is done.


* **PLAYFAB
  * Look at the WIP Folder for any CloudFunctions related code and move it into Cloud Funtions or delete it
  * Make PlayFab an optional dependency (so wrap all playfab code around USING_PLAYFAB)
  * Update all PlayFab Managers to use Async instead of UnityTask
  * Try to get PlayFab code (specially the caching) working in an outside C# project

* Make sure all cool Lost UGUI scripts have filters to Lost folder with AddComponentMenu

* Create Gameplay Events
  * On PlayerData Loaded
  * On PlayerData Before Save
  * On PlayerData After Save
  * On GameData Loaded
  * On GameData Before Save
  * On GameData After Save
  * On Checkpoint Snapshot
  * On Checkpoint Restore
  * On Player Death

* Remove WeakReference out of Experimental, shoudl be replaced with GuidBasedReferences

* appconfig was renamed to bootconfig, make sure source control (and template files) ignore it
  * add bootconfig and releaseconfig while we're at it

  
  