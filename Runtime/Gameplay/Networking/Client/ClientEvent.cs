//-----------------------------------------------------------------------
// <copyright file="ClientEvent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public struct ClientEvent
    {
        public ClientEventType EventType;
        public byte[] Data;
    }
}
