//-----------------------------------------------------------------------
// <copyright file="PurchasingInitializationTimeOutException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_PURCHASING && !UNITY_XBOXONE && !UNITY_LUMIN
#define PURCHASING_ENABLED
#endif

#if PURCHASING_ENABLED

namespace Lost
{
    public class PurchasingInitializationTimeOutException : System.Exception
    {
    }
}

#endif
