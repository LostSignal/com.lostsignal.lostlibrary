


* Create Ably Account?
* Create PlayFab Account?
* Add 

* ------------------------

* Where to input PlayFab Title Id?

* -------------------------

* Add Lost Library and all dependencies (editing manifest.json)
* Create Addressables
* Create App Configs
* Create Bootloader (Drag in Bootloader prefab)
  * Bootloader scene must be named correctly and in the Edtior Build Settings
  * Disable Ably if not using it, or add a key
* Create Main Scene (Add to addressables)
  * Drag this scene into the Bootloader Startup Scene
* Update all your Bootloader Dialogs to be on the correct Layer

