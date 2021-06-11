//-----------------------------------------------------------------------
// <copyright file="DevicePlatform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    [System.Flags]
    public enum DevicePlatform
    {
        None              = 0,
        iOS               = 1 << 0,  // UNITY_IOS / UNITY_IPHONE (Depricated)
        Android           = 1 << 1,  // UNITY_ANDROID
        Windows           = 1 << 2,  // UNITY_STANDALONE_WIN
        Mac               = 1 << 3,  // UNITY_STANDALONE_OSX
        Linux             = 1 << 4,  // UNITY_STANDALONE_LINUX
        WindowsUniversal  = 1 << 5,  // UNITY_WSA_10_0
        XboxOne           = 1 << 6,  // UNITY_XBOXONE
        WebGL             = 1 << 7,  // UNITY_WEBGL
        MagicLeap         = 1 << 8,  // UNITY_LUMIN
    }
}
