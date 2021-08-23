//-----------------------------------------------------------------------
// <copyright file="ClientEventType.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public enum ClientEventType
    {
        ConnectionOpened,
        ConnectionClosed,
        ConnectionLost,
        ReceivedData,
    }
}
