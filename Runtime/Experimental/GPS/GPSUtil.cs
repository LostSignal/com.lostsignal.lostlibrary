//-----------------------------------------------------------------------
// <copyright file="GPSUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public struct GPSLatLong
    {
        public double Latitude;
        public double Longitude;
    }

    public static class GPSUtil
    {
#if UNITY_EDITOR
        private static double fakeEditorLatitude;
        private static double fakeEditorLongitude;
#endif

        public static void SetEditorLatLon(double lat, double lon)
        {
#if UNITY_EDITOR
            fakeEditorLatitude = lat;
            fakeEditorLongitude = lon;
#endif
        }

        public static bool IsGpsEnabledByUser()
        {
            return UnityEngine.Input.location.isEnabledByUser;
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
                Latitude = fakeEditorLatitude,
                Longitude = fakeEditorLongitude,
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
