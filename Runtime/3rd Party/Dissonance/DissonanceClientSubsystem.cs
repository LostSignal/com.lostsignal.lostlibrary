//-----------------------------------------------------------------------
// <copyright file="DissonanceClientSubsystem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE && UNITY

namespace Lost.DissonanceIntegration
{
    using Lost.Networking;

    public class DissonanceClientSubsystem : IGameClientSubsystem
    {
        private GameClient gameClient;

        public void Initialize(GameClient gameClient)
        {
            this.gameClient = gameClient;
            this.gameClient.RegisterMessage<DissonanceMessage>();
        }

        public void Start()
        {
            this.gameClient.ClientReceivedMessage += this.ClientReceivedMessage;
        }

        public void Stop()
        {
            this.gameClient.ClientReceivedMessage -= this.ClientReceivedMessage;
        }

        public void ClientReceivedMessage(Message message)
        {
            if (message.GetId() == DissonanceMessage.MessageId)
            {
                LostClient.Current?.AddMessage((DissonanceMessage)message);
            }
        }
    }
}

#endif
