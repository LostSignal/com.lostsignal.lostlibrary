//-----------------------------------------------------------------------
// <copyright file="LostCommsNetwork.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE && UNITY_2018_4_OR_NEWER

namespace Lost.DissonanceIntegration
{
    using Dissonance;
    using Dissonance.Networking;
    using Lost.Networking;
    using System;

    public class LostCommsNetwork
        : BaseCommsNetwork<
            LostServer, // A class which implements BaseServer
            LostClient, // A class which implements BaseClient
            LostConn,   // A struct which contains a HLAPI NetworkConnection
            Unit,       // Nothing
            Unit>       // Nothing
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
                if (NetworkingManager.Instance && NetworkingManager.Instance.HasJoinedServer)
                {
                    if (this.hasClientStarted == false)
                    {
                        this.hasClientStarted = true;
                        this.RunAsClient(default(Unit));
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
