//-----------------------------------------------------------------------
// <copyright file="IGameClientSubsystem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public interface IGameClientSubsystem
    {
        void Initialize(GameClient gameClient);

        void Start();

        void Stop();
    }
}
