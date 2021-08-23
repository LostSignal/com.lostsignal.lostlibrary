//-----------------------------------------------------------------------
// <copyright file="AwakeManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public sealed class AwakeManager : LoadBalancingManager<AwakeManager, IAwake>
    {
        public override string Name => nameof(AwakeManager);

        public override void Initialize()
        {
            base.Initialize();
            this.SetInstance(this);
        }

        protected override void Execute(IAwake action)
        {
            action.OnAwake();
        }
    }
}

#endif
