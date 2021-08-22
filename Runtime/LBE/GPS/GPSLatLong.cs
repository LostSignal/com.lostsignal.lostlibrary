//-----------------------------------------------------------------------
// <copyright file="GPSLatLong.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct GPSLatLong
    {
        public double Latitude;
        public double Longitude;

        public Vector2d ToVector2d()
        {
            return new Vector2d(this.Latitude, this.Longitude);
        }

        public static GPSLatLong FromVector2d(Vector2d vector)
        {
            return new GPSLatLong { Latitude = vector.x, Longitude = vector.y };
        }

        public override string ToString() => $"{this.Latitude}, {this.Longitude}";
    }
}
