//-----------------------------------------------------------------------
// <copyright file="LostCommsNetwork.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE && UNITY

namespace Lost.DissonanceIntegration
{
    using System;
    using Dissonance;
    using Dissonance.Networking;
    using Lost.Networking;

    public class LostCommsNetwork
        : BaseCommsNetwork<
            LostServer, // A class which implements BaseServer
            LostClient, // A class which implements BaseClient
            LostConn,   // A struct which contains a HLAPI NetworkConnection
            Unit,
            Unit>
    {
        private bool hasClientStarted;

        protected override LostClient CreateClient(Unit connectionParameters)
        {
            return new LostClient(this);
        }

        protected override LostServer CreateServer(Unit connectionParameters)
        {
            // This is a game component and we have a separate server running so the game component should never create one of these
            throw new NotImplementedException();
        }

        protected override void Update()
        {
            if (this.IsInitialized)
            {
                if (NetworkingManager.IsInitialized && NetworkingManager.Instance.HasJoinedServer)
                {
                    if (this.hasClientStarted == false)
                    {
                        this.hasClientStarted = true;
                        this.RunAsClient(default);
                    }
                }
                else
                {
                    if (this.hasClientStarted)
                    {
                        this.hasClientStarted = false;
                        this.Stop();
                    }
                }
            }

            base.Update();
        }
    }
}

#endif
