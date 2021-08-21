#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="GetS2CellResult.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.LBE
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GetS2CellResult
    {
        public enum GetS2CellResultCode
        {
            Success,
            TooFar,
            TravelingTooFast,
            DatabaseError,
        }

        public GetS2CellResultCode ResultCode { get; set; }

        public ulong S2CellId { get; set; }

        public List<LBELocation> Locations { get; set; }

        public DateTime ExpirationUtc { get; set; }

        public double SecondsToLive { get; set; }

        public bool IsExpired() => this.ExpirationUtc < DateTime.UtcNow;

        public void CalculateSecondsToLive()
        {
            this.SecondsToLive = this.ExpirationUtc.Subtract(DateTime.UtcNow).TotalSeconds;
        }
    }
}
