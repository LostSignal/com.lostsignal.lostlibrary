//-----------------------------------------------------------------------
// <copyright file="EditorEvents.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class EditorEvents
    {
        public class OnDomainReloadAttribute : System.Attribute
        {
        }

        public class OnUserBuildInitiatedAttribute : System.Attribute
        {
        }

        public class OnCloudBuildInitiatedAttribute : System.Attribute
        {
        }

        public class OnPreprocessBuildAttribute : System.Attribute
        {
        }

        public class OnPostprocessBuildAttribute : System.Attribute
        {
        }

        public class OnPostGenerateGradleAndroidProjectAttribute : System.Attribute
        {
        }

        public class OnProcessSceneAttribute : System.Attribute
        {
        }

        public class OnExitingPlayModeAttribute : System.Attribute
        {
        }

        public class OnExitPlayModeAttribute : System.Attribute
        {
        }

        public class OnEnterPlayModeAttribute : System.Attribute
        {
        }
    }
}
