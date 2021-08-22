//-----------------------------------------------------------------------
// <copyright file="VisitLocationResult.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class VisitLocationResult
    {
        public enum VisitLocationResultCode
        {
            Success,
            TooFar,
            TravelingTooFast,
        }

        public VisitLocationResultCode ResultCode { get; set; }

        public List<string> Rewards { get; set; }
    }
}
