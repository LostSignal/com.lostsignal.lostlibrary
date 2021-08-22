//-----------------------------------------------------------------------
// <copyright file="GPSDirectionManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    ////
    //// TODO [bgish]: Eventually have different algorithms for determining the players facing direction, or use the magnatrometer if available
    ////
    public class GPSDirectionManager :
        Manager<GPSDirectionManager>
    {
        #pragma warning disable 0649
        [SerializeField] private float slerpSpeed = 2.0f;
        #pragma warning restore 0649

        private GPSLatLong previousLatLong;
        private GPSLatLong currentLatLong;
        private bool hasGpsBeenSet;

        private Quaternion currentRotation;
        private Quaternion desiredRotation;

        public Quaternion Direction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.currentRotation;
        }

        public override void Initialize()
        {
            this.SetInstance(this);

            GPSManager.OnInitialized += () =>
            {
                GPSManager.Instance.OnGPSReceived += this.GPSReceived;
            };
        }

        private void GPSReceived(GPSLatLong gps)
        {
            if (this.hasGpsBeenSet == false)
            {
                this.hasGpsBeenSet = true;
                this.previousLatLong = gps;
                this.currentLatLong = gps;
            }
            else if (this.currentLatLong.Latitude != gps.Latitude || this.currentLatLong.Longitude != gps.Longitude)
            {
                this.previousLatLong = this.currentLatLong;
                this.currentLatLong = gps;
            }

            // Calculate Direction Vector
            double latDir = (this.currentLatLong.Latitude - this.previousLatLong.Latitude) * 1000.0;
            double longDir = (this.currentLatLong.Longitude - this.previousLatLong.Longitude) * 1000.0;
            Vector2d latLongDirection = new Vector2d(latDir, longDir).normalized;

            if (latLongDirection.x != 0.0 || latLongDirection.y != 0.0)
            {
                var directionVector = new Vector3((float)latLongDirection.y, 0.0f, (float)latLongDirection.x);

                // Calculating the desired rotation
                this.desiredRotation = Quaternion.LookRotation(directionVector, Vector3.up);
            }
        }

        private void Update()
        {
            if (this.currentRotation != this.desiredRotation)
            {
                this.currentRotation = Quaternion.Slerp(this.currentRotation, this.desiredRotation, Time.deltaTime * this.slerpSpeed);
            }
        }
    }
}

#endif
