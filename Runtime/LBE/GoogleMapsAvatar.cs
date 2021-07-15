//-----------------------------------------------------------------------
// <copyright file="GoogleMapsAvatar.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.LBE;
    using UnityEngine;

    ////
    //// To Do:
    ////  * Update Avatar to use Cinemachine Camera System
    ////  * Add Rotate (Pan Side to side / Right Mouse side to side)
    ////  * Add Zoom (Pinch / Scroll Wheel)
    ////
    public class GoogleMapsAvatar : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private GameObject body;
        #pragma warning restore 0649

        private bool isBodyVisible;

        private void Awake()
        {
            this.body.SetActive(false);
            this.isBodyVisible = false;
        }

        private void Update()
        {
            if (GPSManager.IsInitialized == false || GoogleMapsManager.IsInitialized == false || GoogleMapsManager.Instance.IsMapLoaded == false)
            {
                return;
            }

            if (this.isBodyVisible != GoogleMapsManager.Instance.IsMapLoaded)
            {
                this.isBodyVisible = GoogleMapsManager.Instance.IsMapLoaded;
                this.body.SetActive(this.isBodyVisible);
            }

            // TODO [bgish]: Eventually listen for GPS data to figure out which way to point the avatar (if phone has magnatrometer, then use that instead)
        }
    }
}
