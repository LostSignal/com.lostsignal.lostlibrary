#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="GameServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;

    public class GameServer
    {
        private IServerTransportLayer transportLayer;
        private MessageCollection messageCollection;

        // Used for serializing messages to a byte buffer
        private NetworkWriter messageWriter = new NetworkWriter();

        // User management
        private Dictionary<long, UserInfo> connectionIdToUserInfoMap = new Dictionary<long, UserInfo>();
        private List<UserInfo> users = new List<UserInfo>();
        private HashSet<long> knownUserIds = new HashSet<long>();
        private ReadOnlyCollection<UserInfo> readonlyUsersList;

        // Subsystem Tracking
        private List<IGameServerSubsystem> subsystems = new List<IGameServerSubsystem>();

        private ConcurrentList<UserInfo> failedJoinConnectionIds = new ConcurrentList<UserInfo>(20);
        private List<UserInfo> failed = new List<UserInfo>();

        private ConcurrentList<UserInfo> successfulJoinUserInfos = new ConcurrentList<UserInfo>(20);
        private List<UserInfo> successes = new List<UserInfo>();

        // tracking data
        private ServerStats stats = new ServerStats();

        public delegate void ServerUserConnectedDelegate(UserInfo userInfo, bool isReconnect);

        public delegate void ServerUserInfoUpdatedDelegate(UserInfo userInfo);

        public delegate void ServerUserDisconnectedDelegate(UserInfo userInfo, bool wasConnectionLost);

        public delegate void ServerReceivedMessageDelegate(UserInfo userInfo, Message message, bool reliable);

        public delegate void ServerStartedDelegate();

        public delegate void ServerUpdatedDelegate();

        public delegate void ServerShutdownDelegate();

        public ServerUserConnectedDelegate ServerUserConnected;
        public ServerUserInfoUpdatedDelegate ServerUserInfoUpdated;
        public ServerUserDisconnectedDelegate ServerUserDisconnected;
        public ServerReceivedMessageDelegate ServerReceivedMessage;
        public ServerStartedDelegate ServerStarted;
        public ServerStartedDelegate ServerUpdated;
        public ServerShutdownDelegate ServerShutdown;

        public GameServer(IServerTransportLayer transportLayer)
        {
            this.transportLayer = transportLayer;
            this.messageCollection = new MessageCollection();

            // client to server
            this.messageCollection.RegisterMessage<JoinServerRequestMessage>();
            this.messageCollection.RegisterMessage<UpdateUserInfoMessage>();

            // server to client
            this.messageCollection.RegisterMessage<JoinServerResponseMessage>();
            this.messageCollection.RegisterMessage<UserDisconnectedMessage>();
            this.messageCollection.RegisterMessage<UserInfoMessage>();
        }

        public IServerStats Stats
        {
            get { return this.stats; }
        }

        public bool IsStarting
        {
            get { return this.transportLayer.IsStarting; }
        }

        public bool IsRunning
        {
            get { return this.transportLayer.IsRunning; }
        }

        public ReadOnlyCollection<UserInfo> ConnectedUsers
        {
            get
            {
                if (this.readonlyUsersList == null)
                {
                    this.readonlyUsersList = new ReadOnlyCollection<UserInfo>(this.users);
                }

                return this.readonlyUsersList;
            }
        }

        public void RegisterSubsystem<T>()
            where T : IGameServerSubsystem, new()
        {
            var subsystem = new T();
            subsystem.Initialize(this);
            this.subsystems.Add(subsystem);
        }

        public void RegisterMessage<T>()
            where T : Message, new()
        {
            this.messageCollection.RegisterMessage<T>();
        }

        public bool Start(int port)
        {
            if (this.transportLayer.IsRunning == false)
            {
                this.stats.StartTime = DateTime.UtcNow;
                this.transportLayer.Start(port);

                if (this.transportLayer.IsRunning)
                {
                    bool success = this.StartSubsystems().Result;

                    if (success)
                    {
                        this.ServerStarted?.Invoke();
                        return true;
                    }
                    else
                    {
                        this.transportLayer.Shutdown();
                        return false;
                    }
                }
            }

            return true;
        }

        public virtual void Shutdown()
        {
            if (this.transportLayer.IsRunning)
            {
                this.stats.ShutdownTime = DateTime.UtcNow;
                this.StopSubsystems().Wait();

                this.transportLayer.Shutdown();

                this.ServerShutdown?.Invoke();
            }
        }

        public void SendMessageToUser(UserInfo userInfo, Message message, bool reliable = true)
        {
            this.SendMessageToConnection(userInfo.ConnectionId, message, reliable);
        }

        public void SendMessageToConnection(long connectionId, Message message, bool reliable = true)
        {
            byte[] data;
            uint length;
            this.GetDataFromMessage(message, out data, out length);

            this.SendData(connectionId, data, length, reliable);
        }

        public void SendMessageToAll(Message message, bool reliable = true)
        {
            byte[] data;
            uint length;
            this.GetDataFromMessage(message, out data, out length);

            for (int i = 0; i < this.users.Count; i++)
            {
                this.SendData(this.users[i].ConnectionId, data, length, reliable);
            }
        }

        public void SendMessageToAllExcept(UserInfo userInfo, Message message, bool reliable = true)
        {
            byte[] data;
            uint length;
            this.GetDataFromMessage(message, out data, out length);

            for (int i = 0; i < this.users.Count; i++)
            {
                if (this.users[i].UserId == userInfo.UserId)
                {
                    continue;
                }

                this.SendData(this.users[i].ConnectionId, data, length, reliable);
            }
        }

        public void AddCustomDataToUser(UserInfo userInfo, string key, string value)
        {
            if (this.users == null)
            {
                return;
            }

            foreach (var user in this.users)
            {
                if (user.UserId == userInfo.UserId)
                {
                    if (user.CustomData.ContainsKey(key))
                    {
                        user.CustomData[key] = value;
                    }
                    else
                    {
                        user.CustomData.Add(key, value);
                    }

                    // Sending the update off to all the clients
                    Debug.Log($"Updateing UserData {user.UserId}");
                    var userInfoMessage = (UserInfoMessage)this.messageCollection.GetMessage(UserInfoMessage.Id);
                    userInfoMessage.UserInfo = user;
                    this.SendMessageToAll(userInfoMessage);
                    this.messageCollection.RecycleMessage(userInfoMessage);

                    break;
                }
            }
        }

        public virtual void Update()
        {
            this.transportLayer?.Update();

            ServerEvent serverEvent;

            while (this.transportLayer.TryDequeueServerEvent(out serverEvent))
            {
                switch (serverEvent.EventType)
                {
                    case ServerEventType.ConnectionOpened:
                        this.ConnectionOpened(serverEvent.ConnectionId);
                        break;

                    case ServerEventType.ConnectionClosed:
                        this.ConnectionClosed(serverEvent.ConnectionId);
                        break;

                    case ServerEventType.ConnectionLost:
                        this.ConnectionLost(serverEvent.ConnectionId);
                        break;

                    case ServerEventType.ReceivedData:
                        this.ReceivedData(serverEvent.ConnectionId, serverEvent.Data, serverEvent.Reliable);
                        break;

                    default:
                        Debug.LogErrorFormat("GameServer: Found unknown ServerEvent type {0}", serverEvent.EventType);
                        break;
                }
            }

            // Sending out all our failed join messages
            this.failedJoinConnectionIds.GetItems(this.failed);

            if (this.failed.Count > 0)
            {
                foreach (var fail in failed)
                {
                    this.SendJoinServerResponse(fail.ConnectionId, fail.UserId, false);
                }

                this.failedJoinConnectionIds.RemoveItems(this.failed);
                this.failed.Clear();
            }

            // Sending out all our success join messages
            this.successfulJoinUserInfos.GetItems(this.successes);

            if (this.successes.Count > 0)
            {
                foreach (var success in this.successes)
                {
                    this.SendJoinServerResponse(success.ConnectionId, success.UserId, true);
                    this.AddOrUpdateUserInfo(success, true);
                }

                this.successfulJoinUserInfos.RemoveItems(this.successes);
                this.successes.Clear();
            }

            try
            {
                this.ServerUpdated?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Encountered an exception updating the server: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        private void ConnectionOpened(long connectionId)
        {
            // Do nothing about this connection till we get a RequestJoinServerMessage
        }

        private void ConnectionClosed(long connectionId)
        {
            this.CloseConnection(connectionId, false);
        }

        private void ConnectionLost(long connectionId)
        {
            this.stats.NumberOfConnectionDrops++;
            this.CloseConnection(connectionId, true);
        }

        private void ReceivedData(long connectionId, byte[] data, bool reliable)
        {
            Message message = this.messageCollection.GetMessage(data);

            this.connectionIdToUserInfoMap.TryGetValue(connectionId, out UserInfo userInfo);

            switch (message.GetId())
            {
                case JoinServerRequestMessage.Id:
                {
                    var requestJoinServer = (JoinServerRequestMessage)message;

                    // Checking if this connection has already registered a user info, if so, they've already joined
                    if (userInfo != null)
                    {
                        Debug.LogErrorFormat("GameServer: Got a JoinServerRequestMessage from ConnectionId {0} who has already joined.", userInfo.ConnectionId);
                        this.SendJoinServerResponse(connectionId, userInfo.UserId, true);
                        break;
                    }

                    if (requestJoinServer.UserInfo != null)
                    {
                        Debug.LogFormat("GameServer: Received JoinServerRequestMessage from UserId {0}", requestJoinServer.UserInfo.UserId);
                        requestJoinServer.UserInfo.ConnectionId = connectionId;
                        var requestJoinTask = this.RequestJoinServer(new UserInfo(requestJoinServer.UserInfo));
                    }
                    else
                    {
                        Debug.LogError("GameServer: JoinServerRequestMessage had a null UserInfo object.");
                        this.SendJoinServerResponse(connectionId, -1, false);
                    }

                    break;
                }

                case UpdateUserInfoMessage.Id:
                {
                    if (userInfo == null)
                    {
                        Debug.LogError("GameServer: Unregistered user tried to send a messages.");
                        break;
                    }

                    var updateUserInfoMessage = (UpdateUserInfoMessage)message;

                    if (updateUserInfoMessage.UserInfo != null)
                    {
                        updateUserInfoMessage.UserInfo.ConnectionId = connectionId;
                    }
                    else
                    {
                        Debug.LogError("GameServer: UpdateUserInfoMessage had a null UserInfo object.");
                        break;
                    }

                    Debug.LogFormat("GameServer: Received UpdateUserInfoMessage from UserId {0}", updateUserInfoMessage.UserInfo.UserId);

                    // checking is someone is trying to hack by changing their userId to something else
                    if (userInfo.UserId != updateUserInfoMessage.UserInfo.UserId)
                    {
                        Debug.LogErrorFormat("GameServer: User {0} is trying to change thier Id to {1}", userInfo.UserId, updateUserInfoMessage.UserInfo.UserId);
                    }
                    else
                    {
                        this.AddOrUpdateUserInfo(updateUserInfoMessage.UserInfo);
                    }

                    break;
                }

                default:
                {
                    if (userInfo == null)
                    {
                        Debug.LogError("GameServer: Unregistered user tried to send a messages.");
                    }
                    else
                    {
                        this.ServerReceivedMessage?.Invoke(userInfo, message, reliable);
                    }

                    break;
                }
            }

            this.messageCollection.RecycleMessage(message);
        }

        private void AddOrUpdateUserInfo(UserInfo userInfoUpdate, bool sendToAll = false)
        {
            UserInfoMessage userInfoMessage = null;
            UserInfo existingUserInfo = null;
            UserInfo newUserInfo = null;

            for (int i = 0; i < this.users.Count; i++)
            {
                if (this.users[i].UserId == userInfoUpdate.UserId)
                {
                    existingUserInfo = this.users[i];
                }
            }

            // checking if this is just a user info updated
            if (existingUserInfo != null)
            {
                existingUserInfo.CopyFrom(userInfoUpdate);
                this.ServerUserInfoUpdated?.Invoke(existingUserInfo);
            }
            else
            {
                // if we got here, then this is a newly connected user
                newUserInfo = new UserInfo();
                newUserInfo.CopyFrom(userInfoUpdate);

                // making sure this user is now in the list and map
                this.users.Add(newUserInfo);

                bool isReconnect = this.knownUserIds.Contains(newUserInfo.UserId);

                this.knownUserIds.Add(newUserInfo.UserId);
                this.stats.NumberOfReconnects += (uint)(isReconnect ? 1 : 0);

                // Telling base class and event that someone has connected
                this.ServerUserConnected?.Invoke(newUserInfo, isReconnect);

                // Since this user is new, send them all the known user infos to them
                userInfoMessage = (UserInfoMessage)this.messageCollection.GetMessage(UserInfoMessage.Id);

                for (int i = 0; i < this.users.Count; i++)
                {
                    userInfoMessage.UserInfo = this.users[i];
                    this.SendMessageToUser(newUserInfo, userInfoMessage);
                }

                this.messageCollection.RecycleMessage(userInfoMessage);
            }

            UserInfo user = newUserInfo ?? existingUserInfo;

            // telling all other users that someone's info has been updated
            userInfoMessage = (UserInfoMessage)this.messageCollection.GetMessage(UserInfoMessage.Id);
            userInfoMessage.UserInfo = user;

            if (sendToAll)
            {
                this.SendMessageToAll(userInfoMessage);
            }
            else
            {
                this.SendMessageToAllExcept(user, userInfoMessage);
            }

            this.messageCollection.RecycleMessage(userInfoMessage);

            // Making sure the user's connectionId is in the map
            if (this.connectionIdToUserInfoMap.ContainsKey(user.ConnectionId) == false)
            {
                this.connectionIdToUserInfoMap.Add(user.ConnectionId, user);
            }

            // Updating max connected stat
            if (this.users.Count > this.stats.MaxConnectedUsers)
            {
                this.stats.MaxConnectedUsers = (uint)this.users.Count;
            }
        }

        private void CloseConnection(long connectionId, bool connectionLost)
        {
            UserInfo userInfo;
            if (this.connectionIdToUserInfoMap.TryGetValue(connectionId, out userInfo) == false)
            {
                return;
            }

            if (userInfo.ConnectionId != connectionId)
            {
                Debug.LogErrorFormat("Trying to close old connection {0}.  User {1} already has connectionId {2}", connectionId, userInfo.UserId, userInfo.ConnectionId);
                return;
            }

            // Removing this connection from all our maps and lists
            this.connectionIdToUserInfoMap.Remove(userInfo.ConnectionId);

            int usersRemovedCount = 0;
            for (int i = this.users.Count - 1; i >= 0; i--)
            {
                if (this.users[i].UserId == userInfo.UserId)
                {
                    this.users.RemoveAt(i);
                    usersRemovedCount++;
                }
            }

            Debug.AssertFormat(usersRemovedCount == 1, "Couldn't properly remove users {0}.  Found {1} when should have found 1", userInfo.UserId, usersRemovedCount);

            // Send the disconnected message to all remaining users
            var userDisconnected = (UserDisconnectedMessage)this.messageCollection.GetMessage(UserDisconnectedMessage.Id);
            userDisconnected.UserId = userInfo.UserId;
            userDisconnected.WasConnectionLost = connectionLost;
            this.SendMessageToAll(userDisconnected);
            this.messageCollection.RecycleMessage(userDisconnected);

            // Telling child classes of the disconnect
            this.ServerUserDisconnected?.Invoke(userInfo, connectionLost);
        }

        private void SendData(long connectionId, byte[] data, uint length, bool reliable = true)
        {
            if (this.transportLayer.IsRunning)
            {
                this.stats.MessagesSent += 1;
                this.stats.BytesSent += length;
                this.transportLayer.SendData(connectionId, data, 0, length, reliable);
            }
        }

        private void GetDataFromMessage(Message message, out byte[] data, out uint length)
        {
            this.messageWriter.SeekZero();
            message.Serialize(this.messageWriter);

            data = messageWriter.RawBuffer;
            length = (uint)messageWriter.Position;
        }

        private async Task<bool> StartSubsystems()
        {
            var runs = new List<Task<bool>>(this.subsystems.Count);

            foreach (var subsystem in this.subsystems)
            {
                runs.Add(subsystem.Run());
            }

            await Task.WhenAll(runs);

            return runs.Any(x => x.Result == false) ? false : true;
        }

        private Task StopSubsystems()
        {
            var runs = new List<Task>(this.subsystems.Count);

            foreach (var subsystem in this.subsystems)
            {
                runs.Add(subsystem.Shutdown());
            }

            return Task.WhenAll(runs);
        }

        private async Task RequestJoinServer(UserInfo userInfo)
        {
            // Making sure all the subsystems say yes
            foreach (var subsystem in this.subsystems)
            {
                if (await subsystem.AllowPlayerToJoin(userInfo) == false)
                {
                    Debug.LogError($"Subsystem {subsystem.Name} failed user {userInfo.UserId}");
                    this.failedJoinConnectionIds.Add(userInfo);
                    return;
                }
            }

            // Making sure all subsystems have a chance to alter the players custom data
            foreach (var subsystem in this.subsystems)
            {
                await subsystem.UpdatePlayerInfo(userInfo);
            }

            this.successfulJoinUserInfos.Add(userInfo);
        }

        private void SendJoinServerResponse(long connectionId, long userId, bool success)
        {
            Debug.Log($"GameServer: Sending JoinServerResponseMessage to UserId {userId} With Accepted = {success}");
            var joinServerResponse = (JoinServerResponseMessage)this.messageCollection.GetMessage(JoinServerResponseMessage.Id);
            joinServerResponse.Accepted = success;
            this.SendMessageToConnection(connectionId, joinServerResponse);
            this.messageCollection.RecycleMessage(joinServerResponse);
        }
    }

    public interface IServerStats
    {
        DateTime StartTime { get; }

        DateTime ShutdownTime { get; }

        uint MessagesSent { get; }

        uint BytesSent { get; }

        uint MaxConnectedUsers { get; }

        uint NumberOfReconnects { get; }

        uint NumberOfConnectionDrops { get; }
    }

    public class ServerStats : IServerStats
    {
        public DateTime StartTime { get; set; }

        public DateTime ShutdownTime { get; set; }

        public uint MessagesSent { get; set; }

        public uint BytesSent { get; set; }

        public uint MaxConnectedUsers { get; set; }

        public uint NumberOfReconnects { get; set; }

        public uint NumberOfConnectionDrops { get; set; }
    }
}
