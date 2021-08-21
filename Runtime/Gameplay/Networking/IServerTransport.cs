#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="IServerTransportLayer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public enum ServerEventType
    {
        ConnectionOpened,
        ConnectionClosed,
        ConnectionLost,
        ReceivedData,
    }

    public struct ServerEvent
    {
        public ServerEventType EventType;
        public long ConnectionId;
        public byte[] Data;
        public bool Reliable;
    }

    public interface IServerTransportLayer
    {
        bool IsStarting { get; }

        bool IsRunning { get; }

        void Start(int port);

        void Update();

        void SendData(long connectionId, byte[] data, uint offset, uint length, bool reliable);

        void Shutdown();

        bool TryDequeueServerEvent(out ServerEvent serverEvent);
    }
}
