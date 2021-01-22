
* Create PlayFab Account (at least Dev)
* Create an Ably Account

* Create Azure Functions Project
  * Add Keys
    * ABLY_SERVER_KEY
    * DEVELOPMENT
    * PF_SECRET_KEY
    * PF_TITLE_ID

* Create Unity Project
  * Turn on Analytics (make sure has it's own project cloud id)
  * Turn on Cloud Build
  * Optional
    * Install Universal RP

* Add Packages to packages.json
  * "com.ably.ably-dotnet": "https://USERNAME:PASSWORD@github.com/LostSignal/com.ably.ably-dotnet.git",
  * "com.lostsignal.lostlibrary": "https://USERNAME:PASSWORD@github.com/LostSignal/com.lostsignal.lostlibrary.git#30b068fa2bbb63a4adf03f5cb551d343ce963da2",
  * "com.microsoft.playfab": "https://USERNAME:PASSWORD@github.com/LostSignal/com.microsoft.playfab.git",
  * "com.revenantx.litenetlib": "https://USERNAME:PASSWORD@github.com/LostSignal/com.revenantx.litenetlib.git",
  * "com.tiinoo.tiinoo": "https://USERNAME:PASSWORD@github.com/LostSignal/com.tiinoo.tiinoo.git",

* Optional Packages to Add
  * "com.placeholder-software.dissonance": "https://USERNAME:PASSWORD@github.com/LostSignal/com.placeholder-software.dissonance.git",
  * "com.vuplex.vuplex": "https://USERNAME:PASSWORD@github.com/LostSignal/com.vuplex.vuplex.git",
  * "com.becausewhynot.webrtc": "https://USERNAME:PASSWORD@github.com/LostSignal/com.becausewhynot.webrtc.git",

* Install Addressables
  * Windows -> Asset Management -> Addressables -> Groups (should be install button there)

* Find Packages/com.lostsignal.lostlibrary/Content/Scenes/Bootloader.unity and copy this into your project
  * Also add this in the first slot in the build settings
  * I'd recommend making your own copy or prefab varients of all the prefabs in this scene
  * I'll try to automate this in the future

* Find Assets/Editor/com.lostsignal.lostlibrary/AzureFunctionsGenerator.asset
  * Give project name and build the project
  * Upload this project to Azure

  
* Find Assets/Editor/com.lostsignal.lostlibrary/GameServerGenerator.asset (OPTIONAL)
  * Generate your game server project and build it
    * Make sure rename the project and fill out all the Custom Replace Variables
  * Dissonance
    * If you use Dissonance, you must add The Dissonance plugin Runtime folder and LostIntegration folder
    * You must also add the NCRUNCH define
    * Also add ```server.RegisterSubsystem<DissonanceServerSubsystem>();``` to Program.cs template

* Find Assets/Editor/com.lostsignal.lostlibrary/AppConfigs.asset (OPTIONAL)
  * Fill out Root Settings
    * Bundle Id, General App Settings
  * Fill out Dev Settings
    * PlayFab   

* Open Bootloader scene
  * Set the Startup scene on the Bootloader Prefab

* Create GameServer build
  * Open GameServer project
  * Do Folder Publish
  * Run "Package Server.bat"
  * Rename Zip to "0.1.0.zip" and upload to PlayFab Multiplayer
    * Add "game_port", UDP
    * Add command line "C:\Assets\GameServer.exe GSDK=1 PF_TITLE_ID=XXXX PF_SECRET_KEY=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
  * Add Title Data "GameServerInfo"
  ```json
  {
    "BuildName": "0.1.0",
    "BuildId": "693b9fed-5a97-4162-8a52-789b140aed5c",  // This guid is the build id after uploading your build zip
    "Regions": [
      "EastUs"
    ]
  }
  ```

* Azure Functions 
  * Generate project
  * Upload Functions to PlayFab
  * Publish project to Azure
