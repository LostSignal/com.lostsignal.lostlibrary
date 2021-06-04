//-----------------------------------------------------------------------
// <copyright file="NetworkingManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Networking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using global::PlayFab.ClientModels;
    using Lost.CloudFunctions;
    using Lost.PlayFab;
    using UnityEngine;

    public interface IGameServerFactory
    {
        GameServer CreateGameServerAndStart(int port);
    }

    public interface IGameClientFactory
    {
        GameClient CreateGameClientAndConnect(string ip, int port);
    }

    public delegate void ConnectedUsersUpdatedDelegate();

    public sealed class NetworkingManager : Manager<NetworkingManager>
    {
        private static readonly string ValidMatchNameCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

#pragma warning disable 0649
        [Header("Dependencies")]
        [SerializeField] private PlayFabManager playfabManager;

        [Header("Mode")]
        [SerializeField] private NetworkingMode networkingMode;
        [SerializeField] private NetworkingMode editorNetworkingMode;

        [Header("Editor Server Info")]
        [SerializeField] private string editorServerIp = "127.0.0.1";
        [SerializeField] private int editorServerPort = 9999;

        [Header("LAN Server Info")]
        [SerializeField] private string lanServerIp = "127.0.0.1";
        [SerializeField] private int lanServerPort = 7777;

        [Header("Debug")]
        [SerializeField] private bool printDebugOutput;
#pragma warning restore 0649

        private bool originalRunInBackground;
        private bool isConnected;

        private IGameServerFactory gameServerFactory;
        private IGameClientFactory gameClientFactory;

        private GameServer gameServer;
        private GameClient gameClient;

        public enum NetworkingMode
        {
            RunClientAndServer,
            RunClientAndLANServer,
            RunClientAndCloudServer,
        }

        public NetworkingMode Mode
        {
            get
            {
                return Application.isEditor ? this.editorNetworkingMode : this.networkingMode;
            }

            set
            {
                if (Application.isEditor)
                {
                    this.editorNetworkingMode = value;
                }
                else
                {
                    this.networkingMode = value;
                }
            }
        }

        public event ConnectedUsersUpdatedDelegate OnConnectedUsersUpdated;

        public bool HasJoinedServer => this.gameClient?.HasJoinedServer == true;

        private ReadOnlyCollection<UserInfo> emptyConnectedUsersList = new ReadOnlyCollection<UserInfo>(new List<UserInfo>());

        public ReadOnlyCollection<UserInfo> ConnectedUsers => this.gameClient?.ConnectedUsers ?? this.emptyConnectedUsersList;

        public static bool PrintDebugOutput => IsInitialized ? Instance.printDebugOutput : false;

        public static string GenerateRandomRoomName()
        {
            System.Random random = new System.Random();

            return BetterStringBuilder.New()
                .Append(ValidMatchNameCharacters[random.Next(0, ValidMatchNameCharacters.Length)])
                .Append(ValidMatchNameCharacters[random.Next(0, ValidMatchNameCharacters.Length)])
                .Append(ValidMatchNameCharacters[random.Next(0, ValidMatchNameCharacters.Length)])
                .Append(ValidMatchNameCharacters[random.Next(0, ValidMatchNameCharacters.Length)])
                .ToString();
        }

        public override void Initialize()
        {
            this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return this.WaitForDependencies(this.playfabManager);

                this.SetInstance(this);
            }
        }

        public void SetGameServerFactory(IGameServerFactory gameServerFactory)
        {
            this.gameServerFactory = gameServerFactory;
        }

        public void SetGameClientFactory(IGameClientFactory gameClientFactory)
        {
            this.gameClientFactory = gameClientFactory;
        }

        public void SendClientMessage(Message message)
        {
            this.gameClient?.SendMessage(message);
        }

        public NetworkIdentity InstantiateNetworkIdentity(string resourceName, Vector3 position)
        {
            return this.InstantiateNetworkIdentity(resourceName, position, Quaternion.identity);
        }

        public NetworkIdentity InstantiateNetworkIdentity(string resourceName, Vector3 position, Quaternion rotation)
        {
            var subsystem = this.gameClient.GetSubsystem<UnityGameClientSubsystem>();
            return subsystem.CreateDynamicNetworkIdentity(resourceName, NetworkIdentity.NewId(), PlayFab.PlayFabManager.Instance.User.PlayFabNumericId, position, rotation);
        }

        public UserInfo GetUserInfo(long playerId)
        {
            if (this.gameClient?.UserInfo?.UserId == playerId)
            {
                return this.gameClient.UserInfo;
            }
            else if (this.gameClient?.ConnectedUsers?.Count > 0)
            {
                foreach (var user in this.gameClient.ConnectedUsers)
                {
                    if (user.UserId == playerId)
                    {
                        return user;
                    }
                }
            }

            return null;
        }

        //// OLD MATCHMAKING/LEGACY SERVER SYSTEM
        ////
        //// public UnityTask<bool> DoesRoomExist(GameServerInfo matchmakingInfo)
        //// {
        ////     return UnityTask<bool>.Run(Coroutine());
        ////
        ////     IEnumerator<bool> Coroutine()
        ////     {
        ////         if (this.Mode == NetworkingMode.RunClientAndServer)
        ////         {
        ////             yield return false;
        ////         }
        ////         else if (this.Mode == NetworkingMode.RunClientAndLANServer)
        ////         {
        ////             yield return false;
        ////         }
        ////         else if (this.Mode == NetworkingMode.RunClientAndCloudServer)
        ////         {
        ////             var matchmake = Lost.PlayFab.PlayFabManager.Instance.Do(this.GetMatchmakeRequest(matchmakingInfo, false));
        ////
        ////             // Wait for matchmake to finish
        ////             while (matchmake.IsDone == false)
        ////             {
        ////                 yield return false;
        ////             }
        ////
        ////             if (matchmake.HasError)
        ////             {
        ////                 yield break;
        ////             }
        ////             else if (matchmake.Value.Status == MatchmakeStatus.NoAvailableSlots)
        ////             {
        ////                 // Room doesn't exist
        ////                 yield return false;
        ////             }
        ////             else
        ////             {
        ////                 yield return true;
        ////             }
        ////         }
        ////         else
        ////         {
        ////             throw new NotImplementedException();
        ////         }
        ////     }
        //// }
        ////
        //// public UnityTask<bool> CreateOrJoinRoom(GameServerInfo matchmakingInfo)
        //// {
        ////     return UnityTask<bool>.Run(Coroutine());
        ////
        ////     IEnumerator<bool> Coroutine()
        ////     {
        ////         if (this.Mode == NetworkingMode.RunClientAndServer)
        ////         {
        ////             var startEditorServer = this.StartEditorLocalServer();
        ////
        ////             while (startEditorServer.IsDone == false)
        ////             {
        ////                 yield return false;
        ////             }
        ////
        ////             if (startEditorServer.HasError || this.gameServer.IsRunning == false)
        ////             {
        ////                 yield break;
        ////             }
        ////         }
        ////
        ////         string serverIp;
        ////         string ticket;
        ////         int port;
        ////
        ////         if (this.Mode == NetworkingMode.RunClientAndServer)
        ////         {
        ////             serverIp = this.editorServerIp;
        ////             port = this.editorServerPort;
        ////             ticket = null;
        ////         }
        ////         else if (this.Mode == NetworkingMode.RunClientAndLANServer)
        ////         {
        ////             serverIp = this.lanServerIp;
        ////             port = this.lanServerPort;
        ////             ticket = null;
        ////         }
        ////         else if (this.Mode == NetworkingMode.RunClientAndCloudServer)
        ////         {
        ////             var matchmake = Lost.PlayFab.PlayFabManager.Instance.Do(this.GetMatchmakeRequest(matchmakingInfo, true));
        ////
        ////             // Waiting for create to finish
        ////             while (matchmake.IsDone == false)
        ////             {
        ////                 yield return false;
        ////             }
        ////
        ////             if (matchmake.HasError)
        ////             {
        ////                 yield break;
        ////             }
        ////
        ////             serverIp = matchmake.Value.ServerPublicDNSName;
        ////             port = matchmake.Value.ServerPort.Value;
        ////             ticket = matchmake.Value.Ticket;
        ////         }
        ////         else
        ////         {
        ////             throw new NotImplementedException();
        ////         }
        ////
        ////         var connect = this.StartClient(serverIp, port);
        ////
        ////         while (connect.IsDone == false)
        ////         {
        ////             yield return false;
        ////         }
        ////
        ////         yield return connect.HasError == false && this.gameClient?.IsConnected == true;
        ////     }
        //// }

        public UnityTask<bool> EnterRoom(string roomId)
        {
            return UnityTask<bool>.Run(Coroutine());

            IEnumerator<bool> Coroutine()
            {
                if (this.Mode == NetworkingMode.RunClientAndServer)
                {
                    var startEditorServer = this.StartEditorLocalServer();

                    while (startEditorServer.IsDone == false)
                    {
                        yield return false;
                    }

                    if (startEditorServer.HasError || this.gameServer.IsRunning == false)
                    {
                        yield break;
                    }
                }

                string serverIp;
                int port;

                if (this.Mode == NetworkingMode.RunClientAndServer)
                {
                    serverIp = this.editorServerIp;
                    port = this.editorServerPort;
                }
                else if (this.Mode == NetworkingMode.RunClientAndLANServer)
                {
                    serverIp = this.lanServerIp;
                    port = this.lanServerPort;
                }
                else if (this.Mode == NetworkingMode.RunClientAndCloudServer)
                {
                    var enterRoom = CloudFunctionsManager.Instance.Rooms_EnterRoom(roomId);

                    // Waiting for enterRoom to finish
                    while (enterRoom.IsCompleted == false)
                    {
                        yield return false;
                    }

                    if (enterRoom.Result.Success == false)
                    {
                        Debug.LogError($"Failed to Enter Room {roomId}: " + enterRoom.Result.Exception);
                        yield break;
                    }

                    var roomServerInfo = enterRoom.Result.ResultObject;

                    serverIp = enterRoom.Result.ResultObject.FQDN;
                    port = enterRoom.Result.ResultObject.Ports.Where(x => x.Name == "game_port").FirstOrDefault().Num;

                    if (this.printDebugOutput)
                    {
                        Debug.Log($"Connecting to Sever {roomServerInfo.FQDN}, Port = {port}, Room Id = {roomServerInfo.RoomId}, Session Id = {roomServerInfo.SessionId}, Server Id = {roomServerInfo.ServerId}");
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                var connect = this.StartClient(serverIp, port);

                while (connect.IsDone == false)
                {
                    yield return false;
                }

                yield return connect.HasError == false && this.gameClient?.IsConnected == true;
            }
        }

        public void Shutdown()
        {
            this.ShutdownClientAndServer();
        }

        private void Awake()
        {
            this.originalRunInBackground = Application.runInBackground;
        }

        private void Update()
        {
            this.gameClient?.Update();
            this.gameServer?.Update();

            // Toggling run in background on/off
            if (this.isConnected == false && this.gameClient?.IsConnected == true)
            {
                Application.runInBackground = true;
                this.isConnected = true;
            }
            else if (this.isConnected && (this.gameClient == null || this.gameClient.IsConnected == false))
            {
                this.isConnected = false;
                Application.runInBackground = this.originalRunInBackground;
            }
        }

        private void OnApplicationQuit()
        {
            this.ShutdownClientAndServer();
        }

        private void OnDestroy()
        {
            this.Shutdown();
            this.OnConnectedUsersUpdated = null;
        }

        private void ShutdownClientAndServer()
        {
            this.ShutdownGameClient();
            this.ShutdownGameServer();
        }

        private void ShutdownGameServer()
        {
            if (this.gameServer != null)
            {
                this.gameServer.Shutdown();
                this.gameServer = null;
            }
        }

        private void ShutdownGameClient()
        {
            if (this.gameClient != null)
            {
                this.gameClient.ClientUserConnected -= this.OnClientUserConnected;
                this.gameClient.ClientUserInfoUpdated -= this.OnClientUserInfoUpdated;
                this.gameClient.ClientUserDisconnected -= this.OnClientUserDisconnected;
                this.gameClient.ClientConnectedToServer -= this.OnClientConnectedToServer;
                this.gameClient.ClientDisconnectedFromServer -= this.OnClientDisconnectedFromServer;
                this.gameClient.Shutdown();
                this.gameClient = null;
            }
        }

        private MatchmakeRequest GetMatchmakeRequest(GameServerInfo info, bool startNewIfNoneFound)
        {
            var request = new MatchmakeRequest
            {
                GameMode = info.GameMode,
                BuildVersion = info.BuildVersion,
                Region = info.Region,
                StartNewIfNoneFound = startNewIfNoneFound,
            };

            if (string.IsNullOrEmpty(info.RoomName) == false)
            {
                request.TagFilter = new CollectionFilter
                {
                    Includes = new List<Container_Dictionary_String_String>
                    {
                        new Container_Dictionary_String_String
                        {
                            Data = new Dictionary<string, string>
                            {
                                { "Room", info.RoomName.ToUpper() },
                            },
                        },
                    },
                };
            }

            return request;
        }

        private UnityTask<bool> StartEditorLocalServer()
        {
            return UnityTask<bool>.Run(Coroutine());

            IEnumerator<bool> Coroutine()
            {
                this.ShutdownGameServer();
                this.gameServer = this.gameServerFactory.CreateGameServerAndStart(this.editorServerPort);

                while (this.gameServer.IsStarting)
                {
                    yield return default(bool);
                }

                yield return this.gameServer.IsRunning;

                if (this.gameServer.IsRunning == false)
                {
                    this.ShutdownGameServer();
                }
            }
        }

        private UnityTask<bool> StartClient(string ip, int port)
        {
            return UnityTask<bool>.Run(Coroutine());

            IEnumerator<bool> Coroutine()
            {
                this.ShutdownGameClient();

                this.gameClient = this.gameClientFactory.CreateGameClientAndConnect(ip, port);
                this.gameClient.ClientUserConnected += this.OnClientUserConnected;
                this.gameClient.ClientUserInfoUpdated += this.OnClientUserInfoUpdated;
                this.gameClient.ClientUserDisconnected += this.OnClientUserDisconnected;
                this.gameClient.ClientConnectedToServer += this.OnClientConnectedToServer;
                this.gameClient.ClientDisconnectedFromServer += this.OnClientDisconnectedFromServer;

                while (this.gameClient.IsConnecting)
                {
                    yield return default(bool);
                }

                yield return this.gameClient.IsConnected;

                if (this.gameClient.IsConnected == false)
                {
                    this.ShutdownGameClient();
                }
            }
        }

        private void OnClientUserConnected(UserInfo userInfo, bool wasReconnect)
        {
            this.OnConnectedUsersUpdated?.Invoke();
        }

        private void OnClientUserInfoUpdated(UserInfo userInfo)
        {
            this.OnConnectedUsersUpdated?.Invoke();
        }

        private void OnClientUserDisconnected(UserInfo userInfo, bool wasConnectionLost)
        {
            this.OnConnectedUsersUpdated?.Invoke();
        }

        private void OnClientConnectedToServer()
        {
            this.OnConnectedUsersUpdated?.Invoke();
        }

        private void OnClientDisconnectedFromServer()
        {
            this.OnConnectedUsersUpdated?.Invoke();
        }

        public class GameServerInfo
        {
            public string GameMode { get; set; }

            public string BuildVersion { get; set; }

            public Region Region { get; set; }

            public string RoomName { get; set; }
        }
    }
}

#endif
