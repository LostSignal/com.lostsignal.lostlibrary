//-----------------------------------------------------------------------
// <copyright file="StartManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public sealed class StartManager : LoadBalancingManager<StartManager, IStart>
    {
        public override string Name => nameof(StartManager);

        public override void Initialize()
        {
            base.Initialize();
            this.SetInstance(this);
        }

        public override void OnUpdate(float deltaTime)
        {
            // Start Manager should only do work if all of the AwakeManager has been processed
            if (AwakeManager.IsInitialized && AwakeManager.Instance.IsProcessing == false)
            {
                base.OnUpdate(deltaTime);
            }
        }

        protected override void Execute(IStart action)
        {
            action.OnStart();
        }
    }
}

#endif
