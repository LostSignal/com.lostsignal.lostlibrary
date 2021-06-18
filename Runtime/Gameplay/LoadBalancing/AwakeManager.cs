//-----------------------------------------------------------------------
// <copyright file="AwakeManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public sealed class AwakeManager : LoadBalancingManager<AwakeManager>
    {
        public override string Name => nameof(AwakeManager);

        public override void Initialize()
        {
            base.Initialize();
            this.SetInstance(this);
        }
    }
}
