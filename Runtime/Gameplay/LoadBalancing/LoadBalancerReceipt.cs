//-----------------------------------------------------------------------
// <copyright file="LoadBalancerReceipt.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;

    public struct LoadBalancerReceipt
    {
        public static LoadBalancerReceipt New(int index, UnityEngine.Object context, Action<int, UnityEngine.Object> cancelAction)
        {
            return new LoadBalancerReceipt
            {
                index = index,
                context = context,
                cancelAction = cancelAction,
            };
        }

        private int index;
        private UnityEngine.Object context;
        private Action<int, UnityEngine.Object> cancelAction;

        public void Cancel()
        {
            if (this.index < 0 || this.cancelAction == null)
            {
                return;
            }

            this.cancelAction.Invoke(this.index, this.context);
            this.index = -1;
            this.context = null;
            this.cancelAction = null;
        }
    }
}

#endif
