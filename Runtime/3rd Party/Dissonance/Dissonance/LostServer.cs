//-----------------------------------------------------------------------
// <copyright file="LostServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE

namespace Lost.DissonanceIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dissonance.Networking;
    using Lost.Networking;

    public class LostServer : BaseServer<LostServer, LostClient, LostConn>
    {
        private readonly ConcurrentList<DissonanceMessage> messages = new ConcurrentList<DissonanceMessage>(50);
        private readonly GameServer gameServer;

        // Temp Data
        private readonly List<DissonanceMessage> tempMessages = new List<DissonanceMessage>();
        private readonly DissonanceMessage tempMessage = new DissonanceMessage();

        public LostServer(GameServer gameServer)
        {
            this.gameServer = gameServer;
        }

        public void AddMessage(DissonanceMessage message)
        {
            this.messages.Add(message.Copy());
        }

        protected override void ReadMessages()
        {
            this.messages.GetItems(this.tempMessages);

            foreach (var message in this.tempMessages)
            {
                this.NetworkReceivedPacket(new LostConn { PlayerId = message.PlayerId }, new ArraySegment<byte>(message.Data));
            }

            this.messages.RemoveItems(this.tempMessages);
        }

        protected override void SendReliable(LostConn connection, ArraySegment<byte> packet)
        {
            this.SendMessage(connection, packet, true);
        }

        protected override void SendUnreliable(LostConn connection, ArraySegment<byte> packet)
        {
            this.SendMessage(connection, packet, false);
        }

        private void SendMessage(LostConn connection, ArraySegment<byte> packet, bool reliable)
        {
            foreach (var user in this.gameServer.ConnectedUsers)
            {
                if (user.UserId == connection.PlayerId)
                {
                    this.tempMessage.IsReliable = reliable;
                    this.tempMessage.PlayerId = connection.PlayerId;
                    this.tempMessage.Data = packet.ToArray();

                    this.gameServer.SendMessageToUser(user, this.tempMessage, reliable);
                }
            }
        }
    }
}

#endif
