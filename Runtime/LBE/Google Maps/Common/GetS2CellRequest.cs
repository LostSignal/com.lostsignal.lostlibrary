#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="GetS2CellRequest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.LBE
{
    using System;

    [Serializable]
    public class GetS2CellRequest
    {
        public ulong S2CellId { get; set; }

        public GPSLatLong CurrentLatLong { get; set; }
    }
}
