
* Create Unity Project
  * Go Into Services Tab
    * Make sure project has a unique unity project id
	* Turn on Analytics (Optional)
	* Turn on Cloud Build (Optional)
	* Turn on Unity Ads (Optional)
	* Turn on Unity IAP (Optional)

* Add Packages to packages.json
  * "com.ably.ably-dotnet": "https://USERNAME:PASSWORD@github.com/LostSignal/com.ably.ably-dotnet.git",
  * "com.lostsignal.lostlibrary": "https://USERNAME:PASSWORD@github.com/LostSignal/com.lostsignal.lostlibrary.git#30b068fa2bbb63a4adf03f5cb551d343ce963da2",
  * "com.microsoft.playfab": "https://USERNAME:PASSWORD@github.com/LostSignal/com.microsoft.playfab.git",
  * "com.revenantx.litenetlib": "https://USERNAME:PASSWORD@github.com/LostSignal/com.revenantx.litenetlib.git",
  * "com.tiinoo.tiinoo": "https://USERNAME:PASSWORD@github.com/LostSignal/com.tiinoo.tiinoo.git",

* Install Addressables
  * Windows -> Asset Management -> Addressables -> Groups (should be install button there)

* Install Text Mesh Pro Essentials

* Go Into PackageManager (Eventually will have a tool for this)
  * Install Universal RP Package (Optional)
  * Install anything else you might need...

* Find Assets/Editor/com.lostsignal.lostlibrary/AppConfigs.asset
  * Fill out Root Settings
    * Bundle Id, General App Settings, Etc

* Create PlayFab Account (at least Dev)
  * Add Title Id and SecretKey to PlayFabSettings in your Dev AppConfig

* Create an Ably Account
  * Add Ably Server Key to PlayFabSettings in your Dev AppConfig
  * Add Ably Client Key to the Releases object.

* Create Azure Functions App in Azure
  * Add the following Settings in the Configuration Section of the Functions App
    * ABLY_SERVER_KEY
    * DEVELOPMENT
    * PF_TITLE_ID
    * PF_SECRET_KEY
  * Copy Site Url and Default Host Key into PlayFabSettings in your Dev AppConfig

* Find Assets/Editor/com.lostsignal.lostlibrary/AzureFunctionsGenerator.asset
  * Give project name and Generate the project
  * Open the project in Visual Studio (must be windows version) and make sure it compiles
  * Publish this project to your Azure Functions App in Azure
  * Select the AzureFunctionsGenerator asset and select the "Register Cloud Functions with PlayFab" button

* Find Packages/com.lostsignal.lostlibrary/Content/Scenes/Bootloader.unity and copy this into your project
  * Also add this in the first slot in the build settings
  * I'd recommend making your own copy or prefab varients of all the prefabs in this scene

* Add your Generic Startup Code class
  * Create empty object in Bootloader scene "Blah Manager"
  * Make a new Script called "BlahManager" and attach that script to the game object
  * Here is an example of a generic script
  ```csharp
  //-----------------------------------------------------------------------
  // <copyright file="HavenManager.cs" company="Lost Signal">
  //     Copyright (c) Lost Signal. All rights reserved.
  // </copyright>
  //-----------------------------------------------------------------------

  namespace Lost
  {
      using Lost.DissonanceIntegration;
      using Lost.Networking;
      using Lost.PlayFab;

      public class HavenManager : Manager<HavenManager>, IGameServerFactory, IGameClientFactory
      {
          public override void Initialize()
          {
              Bootloader.OnBoot += this.OnBoot;
              
              this.SetInstance(this);
          }
          
          private void OnBoot()
          {
              NetworkingManager.Instance.SetGameClientFactory(this);
              NetworkingManager.Instance.SetGameServerFactory(this);
              
              if (UnityEngine.Debug.isDebugBuild)
              {
                  // TODO [bgish]: Add debug buttons here
                  // DialogManager.GetDialog<DebugMenu>().AddItem("Reset Profile", this.ResetProfile);
              }
          }
          
          // This is optional (Only use if your game has a game server)
          GameClient IGameClientFactory.CreateGameClientAndConnect(string ip, int port)
          {
              UserInfo userInfo = new UserInfo();
              userInfo.UserId = PlayFabManager.Instance.User.PlayFabNumericId;
              userInfo.SetPlayFabId(PlayFabManager.Instance.User.PlayFabId);
              userInfo.SetDisplayName(PlayFabManager.Instance.User.DisplayName);
              userInfo.SetSessionTicket(PlayFabManager.Instance.Login.SessionTicket);
              userInfo.CustomData.Add("Platform", XRManager.Instance.CurrentDevice.name);
              
              var gameClient = new GameClient(new LiteNetLibClientTransport(), userInfo, NetworkingManager.PrintDebugOutput);
              gameClient.RegisterSubsystem<UnityGameClientSubsystem>();
              gameClient.RegisterSubsystem<DissonanceClientSubsystem>();
              gameClient.Connect(ip + ":" + port);
              
              return gameClient;
          }
          
          // This is optional (Only use if your game has a game server)
          GameServer IGameServerFactory.CreateGameServerAndStart(int port)
          {
              var gameServer = new GameServer(new LiteNetLibServerTransport());
              gameServer.RegisterSubsystem<UnityGameServerSubsystem>();
              gameServer.RegisterSubsystem<DissonanceServerSubsystem>();
              gameServer.RegisterSubsystem<ColorAssignerServerSubsystem>();
              gameServer.RegisterSubsystem<ValidatePlayFabSessionTicketSubsystem>();
              gameServer.Start(port);
              
              return gameServer;
          }
      }
  }
  ```

### The Following Steps are only required if you game requires a multiplayer server

* Find Assets/Editor/com.lostsignal.lostlibrary/GameServerGenerator.asset 
  * Generate your game server project and make sure it builds
  * If not using Dissonance
    * You can remove a directory from the the Code Folders section
	* You can remove the NCRUNCH define
	* You'll need to remove the ```server.RegisterSubsystem<DissonanceServerSubsystem>();``` from Program.cs template
  * Create GameServer build in PlayFab Multiplayer Settings
	  * Open GameServer project
	  * Do Folder Publish
	  * Run "Package Server.bat" (Creates a Server.zip file in the same folder as the bat file)
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
















# -------------------------------------- OLD NOTES NEEDS CLEANUP --------------------------------------


### New Unity Project Setup
----------------------------------
* Visual StyleCop
* Visual Studio Spell Checker
* Warning as Errors
* Figure out DEFINES (APP_DEV, APP_LIVE, APP_QA, Etc)
 
* P4 Setup
  * Project Editor settings are set to "Force Text", and "Visible Meta Files"
  * P4Connect Setup (or Native P4 Setup)
  * Setup detection of multiple files with same name (just different casing)
  * Setup workspace that is just the Unity folder (for Unity Cloud Build)
     or 
  * Copy "C:\Google Drive\lost signal\Perforce, Git, iOS, Android\Perforce\PerforcePlugin\PerforcePlugin.exe" 
    into C:\Program Files\Unity 5.3.0f4\Editor\Data\Tools\VersionControl
  * Project Editor settings are set to "Force Text", and "Perforce", and fill in appropiate p4 connection data
 
* Unity Cloud Build
  * Create UnityCloudBuild class (append CL to version, warnings as errors?)
  * Create Keystore for Android
  * Create Provisioning files for iOS
  * Setup Unity Services Project and turn on Cloud Build




* New Project Setup
  * Asks for Dev/Live playfab ids - Saved to AppConfigs
  * Asks for Releases/AssetBundle Url/Bucket Info - Saved to AppConfigs
  * Asks for Dev/Live CosmosDB and Redis Connection Strings - Saved to Internal Title Data
  * Copies Bootloader scene adds to project (sets as first scene in build)
  * Creates Dev/Live AppConfig files
  * Adds Releases Scriptable Object
  * Adds Functions Project Generator Scriptable Object
  * Adds Lost Server Project Generator Scriptable Object
  

### Create New Project and Getting App Setup working
------------------------------------------------------------------

  * PackageManager
    * Addressables
	  * Advertisement
	  * Analytics Library
	  * Asset Bundle Browser
	  * In App Purchasing
	  * Lightweight RP
	  * Memory Profiler
	  * TextMesh Pro
	  * Unity User Reporting
	  * Vector Grphics
  
  * Add Tiinoo To Project
  * Add PlayFab + PlayFabEditorExtensions
  
  * Go To Services Tab
    * Enable Ads, Go To Unity Web Dashboard, Click on App and go to Settings and write down ios and android ids
	  * Enable In-App Purchasing
  
  * Add Lost Library
    * Create Root/Dev/Live Configs
    * Add DebugMenu to Resources
    * Add To Root
      * BundleId
  	  * General App Settings
  	  * Override Template Files
  	  * Perforce
  	  * P4 Ignore
  	  * Lost Defines (USING_PLAYFAB_SDK, USING_UNITY_ADDRESSABLES, USING_UNTIY_ADS, USING_UNITY_ANALYTICS, USING_UNITY_PURCHASING)
  	  * LostAds
  	  * LostAnalytics
  	  * LostLogging
    * Add to Dev
      * Development Build (Set True)
  	  * LostDebugMenu
  	  * Tiinoo
    * Add to Live
      * Development Build (Set False)

  * Create IdioWordsApp class

    ```csharp
    using System.Collections;
    using Lost;
    using UnityEngine;

    public class IdioWordsApp : SingletonGameObject<IdioWordsApp>
    {
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeApp()
        {
            IdioWordsApp.Initialize();
        }

        public void Quit()
        {
            Platform.QuitApplication();
        }

        protected override void Awake()
        {
            base.Awake();

            this.StartCoroutine(this.InitializeAppCoroutine());
        }

        private IEnumerator InitializeAppCoroutine()
        {
            // Initialization logic goes here
        }
    }
    ```

### Logining In
--------------------

  * UI
    * Add DebugMenu, ConnectToServer, MessageBox, PurchaseItem, SpinnerBox, StringInputBox to Resources
	
  * PlayFab
    * Add PlayFab AppConfig to Dev and Live
    * Create AppBackend class that inherits from PlayFabBackend


### Cloud Build Setup
----------------------------
* Make sure to setup AppConfigs.SetDevConfig or else a lot of very crutial code won't be fired


