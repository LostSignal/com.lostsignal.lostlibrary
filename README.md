
* Dependency Graph (Everything but Core will eventually have defines to disable them, and eventually I'll make "Manager" prefabs for each of these categories)
  * Core
    * Audio     
    * Networking
      * Gameplay
        * Haven 
    * PlayFab
      * LBE       
    * Misc      
    * Experimental
	
* PlayFab
  * This category has a lot of dependencies
    * PlayFab Accout
	* Azure PubSub Account
	* Azure Functions Account
	* Cosmos DB Account
	
* LBE (Requires a Google Maps SDK Account)

* To Do
  * I have a lot of stub Build Configs that should be moved to Experimental
	
	
	
* -------------------------------------	
	
	
	
* Add Lost Library and all dependencies (editing manifest.json)
* Create Addressables
* Create App Configs
* Create Bootloader (Drag in Bootloader prefab)
  * Bootloader scene must be named correctly and in the Edtior Build Settings
  * Disable Ably if not using it, or add a key
* Create Main Scene (Add to addressables)
  * Drag this scene into the Bootloader Startup Scene
* Update all your Bootloader Dialogs to be on the correct Layer

