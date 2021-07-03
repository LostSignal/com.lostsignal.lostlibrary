//-----------------------------------------------------------------------
// <copyright file="StartManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public sealed class StartManager : LoadBalancingManager<StartManager>
    {
        public override string Name => nameof(StartManager);
        
        public override void Initialize()
        {
            base.Initialize();
            this.SetInstance(this);
        }

        public override void DoUpdate(float deltaTime)
        {
            // Start Manager should only do work if all of the AwakeManager has been processed
            if (AwakeManager.IsInitialized && AwakeManager.Instance.IsProcessing == false)
            {
                base.DoUpdate(deltaTime);
            }
        }
    }
}
