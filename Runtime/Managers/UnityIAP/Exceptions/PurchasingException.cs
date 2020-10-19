//-----------------------------------------------------------------------
// <copyright file="PurchasingException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_PURCHASING && !UNITY_XBOXONE && !UNITY_LUMIN
#define PURCHASING_ENABLED
#endif

#if PURCHASING_ENABLED

namespace Lost
{
    using UnityEngine.Purchasing;

    public class PurchasingException : System.Exception
    {
        public PurchaseFailureReason FailureReason { get; private set; }

        public PurchasingException(PurchaseFailureReason failureReason)
        {
            this.FailureReason = failureReason;
        }
    }
}

#endif
