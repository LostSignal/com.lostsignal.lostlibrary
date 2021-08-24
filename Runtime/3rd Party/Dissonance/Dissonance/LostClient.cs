//-----------------------------------------------------------------------
// <copyright file="LostClient.cs" company="Lost Signal LLC">
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

    public class LostClient : BaseClient<LostServer, LostClient, LostConn>
    {
        private readonly ConcurrentList<DissonanceMessage> messages = new ConcurrentList<DissonanceMessage>(50);

        // Temp Data
        private readonly List<DissonanceMessage> tempMessages = new List<DissonanceMessage>();
        private readonly DissonanceMessage tempMessage = new DissonanceMessage();

        public LostClient(ICommsNetworkState network)
            : base(network)
        {
        }

        public static LostClient Current { get; private set; }

        public void AddMessage(DissonanceMessage message)
        {
            this.messages.Add(message.Copy());
        }

        public override void Connect()
        {
            Current = this;

            this.Connected();
        }

        public override void Disconnect()
        {
            base.Disconnect();

            Current = null;
        }

        protected override void ReadMessages()
        {
            this.messages.GetItems(this.tempMessages);

            foreach (var message in this.tempMessages)
            {
                this.NetworkReceivedPacket(new ArraySegment<byte>(message.Data));
            }

            this.messages.RemoveItems(this.tempMessages);
        }

        protected override void SendReliable(ArraySegment<byte> packet)
        {
            this.SendPacket(packet, true);
        }

        protected override void SendUnreliable(ArraySegment<byte> packet)
        {
            this.SendPacket(packet, false);
        }

        private void SendPacket(ArraySegment<byte> packet, bool reliable)
        {
#if UNITY
            this.tempMessage.PlayerId = PlayFab.PlayFabManager.Instance.User.PlayFabNumericId;
            this.tempMessage.IsReliable = reliable;
            this.tempMessage.Data = packet.ToArray();

            // TODO [bgish]: Need send it reliably or not based on the reliable bool
            NetworkingManager.Instance.SendClientMessage(this.tempMessage);
#endif
        }
    }
}

#endif
