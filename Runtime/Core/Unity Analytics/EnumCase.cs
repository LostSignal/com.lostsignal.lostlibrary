//-----------------------------------------------------------------------
// <copyright file="EnumCase.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ANALYTICS && !UNITY_XBOXONE
#define UNITY_ANALYTICS_SUPPORTED
#endif

#if UNITY && UNITY_ANALYTICS_SUPPORTED

namespace Lost
{
    using UnityEngine.Analytics;

    public sealed class EnumCase : AnalyticsEventAttribute
    {
        private readonly Styles style;

        public EnumCase(Styles style)
        {
            this.style = style;
        }

        public enum Styles
        {
            None = 0,
            Snake = 1,
            Lower = 2,
        }

        public Styles Style => this.style;
    }
}

#endif
