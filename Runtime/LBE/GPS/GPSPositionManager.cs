//-----------------------------------------------------------------------
// <copyright file="GPSPositionManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class GPSPositionManager
        : Manager<GPSPositionManager>
    {
#pragma warning disable 0649
        [SerializeField] private double lerpSpeed = 0.002;
#pragma warning restore 0649

        private bool hasReceivedGpsData;
        private GPSLatLong currentLatLng;
        private GPSLatLong desiredLatLng;
        private GPSLatLong lastSentLatLng;
        private double speed;

        public Action<GPSLatLong> OnGPSChanged;

        public bool HasReceivedGPSData => this.hasReceivedGpsData;

        public GPSLatLong CurrentLatLong => this.currentLatLng;

        public float Speed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)this.speed;
        }

        public override void Initialize()
        {
            this.SetInstance(this);
            GPSManager.OnInitialized += InitializeGPS;

            void InitializeGPS()
            {
                GPSManager.Instance.OnGPSReceived += this.OnGpsReceived;
            }
        }

        private void OnGpsReceived(GPSLatLong latLong)
        {
            if (this.hasReceivedGpsData == false)
            {
                this.hasReceivedGpsData = true;
                this.currentLatLng = latLong;
                this.desiredLatLng = latLong;
            }
            else
            {
                this.desiredLatLng = latLong;
            }
        }

        private void Update()
        {
            double deltaTime = Time.deltaTime;

            // Calculate our new lat long
            this.currentLatLng = GPSUtil.MoveTowards(this.currentLatLng, this.desiredLatLng, this.lerpSpeed, deltaTime);

            if (this.lastSentLatLng.Latitude != this.currentLatLng.Latitude || this.lastSentLatLng.Longitude != this.currentLatLng.Longitude)
            {
                this.speed = GPSUtil.DistanceInMeters(this.lastSentLatLng, this.currentLatLng) / deltaTime;
                this.lastSentLatLng = this.currentLatLng;

                try
                {
                    this.OnGPSChanged?.Invoke(this.currentLatLng);
                }
                catch (Exception ex)
                {
                    Debug.LogError("GPSPositionManager caught exception when invoking OnGPSChanged.");
                    Debug.LogException(ex);
                }
            }
            else
            {
                this.speed = 0.0f;
            }
        }
    }
}

#endif
