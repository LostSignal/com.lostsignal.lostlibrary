//-----------------------------------------------------------------------
// <copyright file="DissonanceServerSubsystem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE

namespace Lost.DissonanceIntegration
{
    using System.Threading.Tasks;
    using Lost.Networking;

    public class DissonanceServerSubsystem : IGameServerSubsystem
    {
        private LostServer dissonanceServer = null;
        private GameServer gameServer = null;

        public string Name => nameof(DissonanceServerSubsystem);

        public void Initialize(GameServer gameServer)
        {
            this.gameServer = gameServer;
            this.gameServer.RegisterMessage<DissonanceMessage>();

            this.dissonanceServer = new LostServer(this.gameServer);
        }

        public Task<bool> Run()
        {
            this.gameServer.ServerReceivedMessage += this.ServerReceivedMessage;
            this.gameServer.ServerUpdated += this.ServerUpdated;

            return Task.FromResult<bool>(true);
        }

        public Task Shutdown()
        {
            this.gameServer.ServerReceivedMessage -= this.ServerReceivedMessage;
            this.gameServer.ServerUpdated -= this.ServerUpdated;

            return Task.Delay(0);
        }

        public Task<bool> AllowPlayerToJoin(UserInfo userInfo)
        {
            return Task.FromResult<bool>(true);
        }

        public Task UpdatePlayerInfo(UserInfo userInfo)
        {
            return Task.Delay(0);
        }

        public void ServerReceivedMessage(UserInfo userInfo, Message message, bool reliable)
        {
            if (message.GetId() == DissonanceMessage.MessageId)
            {
                this.dissonanceServer.AddMessage((DissonanceMessage)message);
            }
        }

        private void ServerUpdated()
        {
            this.dissonanceServer.Update();
        }
    }
}

#endif
