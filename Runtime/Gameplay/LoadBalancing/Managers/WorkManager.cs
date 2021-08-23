//-----------------------------------------------------------------------
// <copyright file="WorkManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public sealed class WorkManager : LoadBalancingManager<WorkManager, IWork>
    {
        public override string Name => nameof(WorkManager);

        public override void Initialize()
        {
            base.Initialize();
            this.SetInstance(this);
        }

        protected override void Execute(IWork action)
        {
            action.OnWork();
        }
    }
}

#endif
