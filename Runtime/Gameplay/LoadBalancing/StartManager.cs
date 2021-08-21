#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="StartManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public interface IStartable
    {
        void DoStart();
    }

    public sealed class StartManager : LoadBalancingManager<StartManager, IStartable>
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

        protected override void Execute(IStartable action)
        {
            action.DoStart();
        }
    }
}

#endif
