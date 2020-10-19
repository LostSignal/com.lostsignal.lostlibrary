//-----------------------------------------------------------------------
// <copyright file="DebugCloudFunctions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions.Debug
{
    using System;
    using System.Threading.Tasks;

    public static class DebugCloudFunctions
    {
        [DevCloudFunction("Debug", "PurchaseItem")]
        public static Task PurchaseItem(CloudFunctionContext context, DebugPurchaseItemRequest request)
        {
            throw new NotImplementedException();
        }

        [DevCloudFunction("Debug", "GiveCurrency")]
        public static Task GiveCurrency(CloudFunctionContext context, DebugGiveCurrencyRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
