
### Build Validation Tool
----------------------------------
* Check out this validator https://github.com/DarrenTsung/DTValidator

* If there is a UI element who's z != 0, print warning

* Is OnValidate called at build time?  If so, if I throw a build exception will it fail the build?

* CalledByUnityEvent Attribute (See Experimental Folder in Lost Library)

* Validators!!!!
  * Can run on scene open
  * Print warnings/errors
  * Fail builds (must check every ScriptableObject, Prefab, Scene)
  * etc

* No two dialog with the same name (so Screen analytics work)
* Print warning if you have two dialog on the same layer?

* InitializeOnLoad grab every object in the scene and recursively look for these attributes

* Need to make my system that processes all scenes/prefabs and prints build warning/errors if things aren't working
  * RectTransforms scale are 1,1,1
  * Button has no Raycast Targets
  * [CalledByUnity] for UI
  * LazyAssets, Localization Ids, Settings Strings point to real data
  * All MonoBehaviorus with [Validate] methods get called and results are part of build results
  * Have menu items for "Validate Scene", "Validate Folder", "Validate All" so you can get the output without making build
    * Possibly register for Debug.LogError/LogWarning events and throw build exceptions if those things happen?
  * Presets References in folder meta files are valid
  * Validate On Play Mode Start????
