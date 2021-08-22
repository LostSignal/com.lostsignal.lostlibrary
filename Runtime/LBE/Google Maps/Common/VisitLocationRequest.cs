//-----------------------------------------------------------------------
// <copyright file="VisitLocationRequest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.LBE
{
    using System;

    [Serializable]
    public class VisitLocationRequest
    {
        public string LocationId { get; set; }

        public GPSLatLong CurrentLatLong { get; set; }
    }
}
