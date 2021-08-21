#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LBELocation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.LBE
{
    using System;

    [Serializable]
    public class LBELocation
    {
        public string LocationId { get; set; }

        public string TypeId { get; set; }

        public GPSLatLong LatLong { get; set; }
    }
}
