
* Create PlayFab Account (at least Dev)
* Create an Ably Account 
* Create New Unity Project (make sure you have a valid cloud based project)
* Turn on Analytics
* Turn on Cloud Build
* Install Universal RP

* Add Packages to packages.json
  * "com.ably.ably-dotnet": "file:../../../Packages/com.ably.ably-dotnet",
  * "com.lostsignal.lostlibrary": "file:../../../Packages/com.lostsignal.lostlibrary",
  * "com.microsoft.playfab": "file:../../../Packages/com.microsoft.playfab",
  * "com.revenantx.litenetlib": "file:../../../Packages/com.revenantx.litenetlib",
  * "com.tiinoo.tiinoo": "file:../../../Packages/com.tiinoo.tiinoo",

* Optional Packages to Add
  * "com.vuplex.vuplex": "file:../../../Packages/com.vuplex.vuplex",
  * "com.becausewhynot.webrtc": "file:../../../Packages/com.becausewhynot.webrtc",

* Install Addressables
  * Windows -> Asset Management -> Addressables -> Groups (should be install button there)

* Find Packages/com.lostsignal.lostlibrary/Content/Scenes/Bootloader.unity and copy this into your project
  * Also add this in the first slot in the build settings
  * I'd recommend making your own copy or prefab varients of all the prefabs in this scene
  * I'll try to automate this in the future
  
* Find Packages/com.lostsignal.lostlibrary/Content/ScriptableObjects/AzureFunctionsGenerator.asset 
  * Copy this asset into the Assets/Editor/com.lostsignal.lostlibrary folder
  * I'll try to automate this in the future

* Find Packages/com.lostsignal.lostlibrary/Content/ScriptableObjects/GameServerGenerator.asset 
  * Copy this asset into the Assets/Editor/com.lostsignal.lostlibrary folder
  * I'll try to automate this in the future
  * Generate your game server project and build it
    * Make sure rename the project and fill out all the Custom Replace Variables
  * Dissonance
    * If you use Dissonance, you must add The Dissonance plugin Runtime folder and LostIntegration folder
    * You must also add the NCRUNCH define
    * Also add ```server.RegisterSubsystem<DissonanceServerSubsystem>();``` to Program.cs template

* Lost Library
  * Lost Library will auto generate App Configs in the Assets/Editor/com.lostsignal.lostlibrary folder.
    * There are 3 AppConfigs Dev, Live and Root, you'll need to make sure to fill everything out in these
      * Root.asset
  	    * Bundle Id
  	    * General App Settings
  	    * Perforce Settings
  	    * Android Keystore Settings
  	  * Live
  	    * PlayFab Account Settings
  	  * Dev
  	    * PlayFab Account Settings (**MUST SET THESE**)
  * Lost Library will also auto generate a Releases object in the Assets/Editor/com.lostsignal.lostlibrary folder.
    * The Bootloader's ReleasesManager loads this data to figure out how to download a bunch of initial data
    * You must set the Ably Key

* Open Bootloader scene
  * Set the Startup scene (make sure that scene is in the Addressables system)

