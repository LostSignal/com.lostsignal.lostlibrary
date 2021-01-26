
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

* Install Addressables
  * Windows -> Asset Management -> Addressables -> Groups (should be install button there)

* Find Assets/Editor/com.lostsignal.lostlibrary/AppConfigs.asset\
  * Fill out Root Settings
    * Bundle Id, General App Settings

* Create PlayFab Account (at least Dev)
  * Add Title Id and SecretKey to PlayFabSettings in your Dev AppConfig

* Create an Ably Account
  * Add Ably Server Key to PlayFabSettings in your Dev AppConfig
  * Add Ably Client Key to the Releases object.

* Create Azure Functions Project
  * Copy Site Url and Default Host Key into PlayFabSettings in AppConfigs
  * Add Keys to Functions App
    * ABLY_SERVER_KEY
    * DEVELOPMENT
    * PF_SECRET_KEY
    * PF_TITLE_ID

* Find Packages/com.lostsignal.lostlibrary/Content/Scenes/Bootloader.unity and copy this into your project
  * Also add this in the first slot in the build settings
  * I'd recommend making your own copy or prefab varients of all the prefabs in this scene

* Find Assets/Editor/com.lostsignal.lostlibrary/AzureFunctionsGenerator.asset
  * Give project name and Generate the project
  * Open the project in Visual Studio (must be windows version) and make sure it compiles
  * Upload this project to your Azure Functions App

* Find Assets/Editor/com.lostsignal.lostlibrary/GameServerGenerator.asset (OPTIONAL)
  * Generate your game server project and build it
  * Dissonance
    * If you use Dissonance, you must add The Dissonance plugin Runtime folder and LostIntegration folder
    * You must also add the NCRUNCH define
    * Also add ```server.RegisterSubsystem<DissonanceServerSubsystem>();``` to Program.cs template

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

* Create GameServer build
  * Open GameServer project
  * Do Folder Publish
  * Run "Package Server.bat" (Puts the Zip file on your Desktop)
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
