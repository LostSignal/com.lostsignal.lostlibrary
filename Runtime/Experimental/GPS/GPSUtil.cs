//-----------------------------------------------------------------------
// <copyright file="GPSUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;

    [Serializable]
    public struct GPSLatLong
    {
        public double Latitude;
        public double Longitude;

        public override string ToString() => $"{this.Latitude}, {this.Longitude}";
    }
    
    ////
    //// This class is a major work in progress.  It is not the best thing to use just yet.
    ////
    public static class GPSUtil
    {
        public static double DistanceInMeters(GPSLatLong latLong1, GPSLatLong latLong2)
        {
            return DistanceTo(latLong1.Latitude, latLong1.Longitude, latLong2.Latitude, latLong2.Longitude) * 1000.0;

            // https://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates
            double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
            {
                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
                double rtheta = Math.PI*theta/180;

                double dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                              Math.Cos(rlat2) * Math.Cos(rtheta);

                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515;

                switch (unit)
                {
                    case 'K': //Kilometers -> default
                        return dist*1.609344;
                    case 'N': //Nautical Miles 
                        return dist*0.8684;
                    case 'M': //Miles
                        return dist;
                }

                return dist;
            }
        }

        public static GPSLatLong MoveTowards(GPSLatLong currentLatLong, GPSLatLong desiredLatLong, double speed, double deltaTime)
        {
            // Creating a direction vector for where we want to lerp towards
            var directionLatLong = new GPSLatLong
            {
                Latitude = desiredLatLong.Latitude - currentLatLong.Latitude,
                Longitude = desiredLatLong.Longitude - currentLatLong.Longitude,
            };

            // Getting the squared length of this directional vector
            var directionSquareDistance = (directionLatLong.Latitude * directionLatLong.Latitude) + (directionLatLong.Longitude * directionLatLong.Longitude);

            if (directionSquareDistance == 0.0f)
            {
                return currentLatLong;
            }

            // Calculating the Distance we wish to travel this tick
            double distance = speed * deltaTime;
            double squareDistance = distance * distance;

            if (directionSquareDistance < squareDistance)
            {
                // Our desired distance is further than our actual, so just set the distance to the actual
                return desiredLatLong;
            }
            else
            {
                // We're not at the end yet, so lets lerp towards our desired
                var directionDistance = System.Math.Sqrt(directionSquareDistance);

                return new GPSLatLong
                {
                    Latitude = currentLatLong.Latitude + ((directionLatLong.Latitude / directionDistance) * distance),
                    Longitude = currentLatLong.Longitude + ((directionLatLong.Longitude / directionDistance) * distance),
                };
            }
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
}

#endif
