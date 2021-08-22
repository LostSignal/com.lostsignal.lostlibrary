//-----------------------------------------------------------------------
// <copyright file="GPSUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

#if UNITY

    ////
    //// This class is a major work in progress.  It is not the best thing to use just yet.
    ////
    public static class GPSUtil
    {
        public enum Unit
        {
            Meters,
            Kilometers,
            NauticalMiles,
            Miles,
        }

        public static double DistanceInMeters(GPSLatLong latLong1, GPSLatLong latLong2)
        {
            return DistanceTo(latLong1.Latitude, latLong1.Longitude, latLong2.Latitude, latLong2.Longitude);

            // https://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates
            double DistanceTo(double lat1, double lon1, double lat2, double lon2, Unit unit = Unit.Meters)
            {
                double rlat1 = Math.PI * lat1 / 180.0;
                double rlat2 = Math.PI * lat2 / 180.0;
                double theta = lon1 - lon2;
                double rtheta = Math.PI * theta / 180.0;

                double dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                    Math.Cos(rlat2) * Math.Cos(rtheta);

                dist = Math.Acos(dist);
                dist = dist * 180.0 / Math.PI;
                dist = dist * 60.0 * 1.1515;

                switch (unit)
                {
                    case Unit.Meters:        return dist * 1609.344;
                    case Unit.Kilometers:    return dist * 1.609344;
                    case Unit.NauticalMiles: return dist * 0.8684;
                    case Unit.Miles:         return dist;
                    default:                 return dist;
                }
            }
        }

        public static GPSLatLong MoveTowards(GPSLatLong fromLatLong, GPSLatLong toLatLong, double speed, double deltaTime)
        {
            var newPosition = Vector2d.MoveTowards(fromLatLong.ToVector2d(), toLatLong.ToVector2d(), speed * deltaTime);

            return new GPSLatLong
            {
                Latitude = newPosition.x,
                Longitude = newPosition.y,
            };
        }

        public static bool IsGpsEnabledByUser()
        {
#if UNITY_EDITOR
            return true;
#else
            return UnityEngine.Input.location.isEnabledByUser;
#endif
        }

        public static void AskForPermissionToUseGPS()
        {
#if UNITY_ANDROID && !UNITY_EDITOR

            if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation) == false)
            {
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
            }

#elif UNITY_IOS && !UNITY_EDITOR

            //// Do Nothing, iOS can't ask for permissions

#elif UNITY_EDITOR

            //// Do Nothing, permissions aren't needed

#else

            throw new System.PlatformNotSupportedException();

#endif
        }

        public static GPSLatLong GetGPSLatLong()
        {
#if UNITY_IOS && !UNITY_EDITOR

            return new GPSLatLong
            {
                Latitude = _GetLatitude(),
                Longitude = _GetLongitude(),
            };

#elif UNITY_ANDROID && !UNITY_EDITOR

            return new GPSLatLong
            {
                Latitude = UnityEngine.Input.location.lastData.latitude,
                Longitude = UnityEngine.Input.location.lastData.longitude,
            };

#elif UNITY_EDITOR

            return new GPSLatLong
            {
                Latitude = UnityEngine.Input.location.lastData.latitude,
                Longitude = UnityEngine.Input.location.lastData.longitude,
            };

#else

            throw new System.PlatformNotSupportedException();
#endif
        }

#if UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern double _GetLatitude();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern double _GetLongitude();
#endif
    }
#endif
}
