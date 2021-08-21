#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="AwakeManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public interface IAwakable
    {
        void DoAwake();
    }

    public sealed class AwakeManager : LoadBalancingManager<AwakeManager, IAwakable>
    {
        public override string Name => nameof(AwakeManager);

        public override void Initialize()
        {
            base.Initialize();
            this.SetInstance(this);
        }

        protected override void Execute(IAwakable action)
        {
            action.DoAwake();
        }
    }
}

#endif
