//-----------------------------------------------------------------------
// <copyright file="PurchasingInitializationException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_PURCHASING && !UNITY_XBOXONE && !UNITY_LUMIN
#define PURCHASING_ENABLED
#endif

#if PURCHASING_ENABLED

namespace Lost
{
    using UnityEngine.Purchasing;

    public class PurchasingInitializationException : System.Exception
    {
        public PurchasingInitializationException(InitializationFailureReason failureReason)
        {
            this.FailureReason = failureReason;
        }

        public InitializationFailureReason FailureReason { get; private set; }
    }
}

#endif
