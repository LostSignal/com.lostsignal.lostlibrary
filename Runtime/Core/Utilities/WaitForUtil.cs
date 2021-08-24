//-----------------------------------------------------------------------
// <copyright file="WaitForUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class WaitForUtil
    {
        public static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();
        private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsCache = new Dictionary<float, WaitForSeconds>();
        private static readonly Dictionary<float, WaitForSecondsRealtime> WaitForSecondsRealtimeCache = new Dictionary<float, WaitForSecondsRealtime>();

        public static WaitForSeconds Seconds(float time)
        {
            if (WaitForSecondsCache.TryGetValue(time, out WaitForSeconds waitForSeconds) == false)
            {
                waitForSeconds = new WaitForSeconds(time);
                WaitForSecondsCache.Add(time, waitForSeconds);
            }

            return waitForSeconds;
        }

        public static WaitForSecondsRealtime RealtimeSeconds(float time)
        {
            if (WaitForSecondsRealtimeCache.TryGetValue(time, out WaitForSecondsRealtime waitForSecondsRealtime) == false)
            {
                waitForSecondsRealtime = new WaitForSecondsRealtime(time);
                WaitForSecondsRealtimeCache.Add(time, waitForSecondsRealtime);
            }

            return waitForSecondsRealtime;
        }
    }
}

#endif
