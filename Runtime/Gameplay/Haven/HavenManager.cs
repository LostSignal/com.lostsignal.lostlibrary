//-----------------------------------------------------------------------
// <copyright file="HavenManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Haven
{
    using Lost;

    public class HavenManager : Manager<HavenManager>
    {
        public override void Initialize()
        {
            this.SetInstance(this);
        }
    }
}

/*
    * HavenManager.StartExperience
      * Load Scenes
      * Create Server
      * Create Client
      * Make sure all network behaviours are still working
      * Make sure ownership switching is working
      * Make sure you can create Network objects and have them replicate
      * Need concept of requesting ownership (Need RequestOwnership message)
*/


/*

NetworkingManager
  bool RunServerLocally
  void RegisterServerSubsystems<T>();
  void RegisterClientSubsystems<T>();

  void CreateServer(int port);
  UnityTask CreateClient(string connectionString, int retryCount);

  UnityTask StartPrivateExperience
  UnityTask JoinPrivateExperienceWithPopup
  UnityTask JoinPrivateExperience


HavenManager

  Load Experience()
    Go to Loading Level
    Download scenes
    Load Scenes
    UnityTask NetworkManager.InitializeServer(ServerSubsystems)  (skip if not running locally)
    MatchMaking...  (skip if running locally)
    UnityTask NetworkManager.InitializeClient(ClientSubsystems)
      Connects to Server
      Registers this Client with all Static objects
      Sends NetworkIdentityRequestUpdate for every static object
      Wait for either NetworkIdentityUpdate or NetworkIdentityDestoryed
      Wait for Dynamic NetworkIdentites to be created

GameServer server = new GameServer(7777);
server.AddSubsytem<UnityGameServerSubsystem>();
server.AddSubsytem<HavenGameServerSubsystem>();
server.Start();

GameClient client = new GameClient();
client.AddSubsytem<UnityGameClientSubsystem>();
client.AddSubsytem<HavenGameClientSubsystem>();
client.Start();

bool IsServerRunning { get; }
bool IsClientRunning { get; }
UnityTask NetworkingManager.StartServer(List<IGameServerSubsystem> subsystems);
UnityTask NetworkingManager.StartClient(List<IGameClientSubsystem> subsystems);
void StopServer();
void StopClient();
void StopAll();
void Update(); // Calls Update() on client and/or server

*/
