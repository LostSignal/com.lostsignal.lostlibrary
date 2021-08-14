
# To Do
* I have a lot of stub Build Configs that should be moved to Experimental
* Move Analyzers folder to Content
* Delete Resources folder and have a Content/Managers folder with all the managers
* Add Content/Bootloader folder where the bootloader prefab will live?

# Dependency Graph 
* Everything but Core will eventually have defines to disable them, and eventually I'll make "Manager" prefabs for each of these categories.
* Hierarchy
  * Core
    * Audio        - Simple Scriptable Object based Audio system
    * Networking   - 
      * Gameplay   - 
        * Haven    - An XR interaction framework built on top of Unity's XR Interaction Toolkit
    * PlayFab      - A PlayFab Wrapper with caching and dialogs for Login, Updating DisplayName, Etc
      * LBE        - A set of managers needed to make a LBE Game
    * Misc         - Random Code: Unity's Guid Based References, Lost Signals Interactive Plants and a English Word Dictionary
    * Experimental - Almost everything in here is trash and should be removed
	
# PlayFab
* Requires PlayFab Accout
* Requires Azure PubSub Account
* Requires Azure Functions Account
* Requires Cosmos DB Account
	
# LBE 
* Requires a Google Maps SDK Account

# Installing Lost Library Instructions	
* Add Addressables to project and create required data
* Add Lost Library and all dependencies (editing manifest.json)
* Edit App Configs
* Create Bootloader Scene (Add to Build Settings)
  * Drag in Bootloader Prefab
* Create Managers Scene   (Add to Addressables)
  * Drag in all Manager prefabs you whish to have in your project
