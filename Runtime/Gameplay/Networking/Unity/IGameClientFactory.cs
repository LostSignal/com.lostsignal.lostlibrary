//-----------------------------------------------------------------------
// <copyright file="IGameClientFactory.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public interface IGameClientFactory
    {
        GameClient CreateGameClientAndConnect(string ip, int port);
    }
}
