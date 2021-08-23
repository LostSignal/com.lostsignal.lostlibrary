//-----------------------------------------------------------------------
// <copyright file="EditorEvents.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class EditorEvents
    {
        public sealed class OnDomainReloadAttribute : System.Attribute
        {
        }

        public sealed class OnUserBuildInitiatedAttribute : System.Attribute
        {
        }

        public sealed class OnCloudBuildInitiatedAttribute : System.Attribute
        {
        }

        public sealed class OnPreprocessBuildAttribute : System.Attribute
        {
        }

        public sealed class OnPostprocessBuildAttribute : System.Attribute
        {
        }

        public sealed class OnPostGenerateGradleAndroidProjectAttribute : System.Attribute
        {
        }

        public sealed class OnProcessSceneAttribute : System.Attribute
        {
        }

        public sealed class OnExitingPlayModeAttribute : System.Attribute
        {
        }

        public sealed class OnExitPlayModeAttribute : System.Attribute
        {
        }

        public sealed class OnEnterPlayModeAttribute : System.Attribute
        {
        }
    }
}
