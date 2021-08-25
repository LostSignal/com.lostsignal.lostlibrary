//-----------------------------------------------------------------------
// <copyright file="PlayFabUserInfoExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || USING_PLAYFAB

namespace Lost
{
    using Lost.Networking;

    public static class PlayFabUserInfoExtensions
    {
        public static string GetPlayFabId(this UserInfo userInfo)
        {
            return GetValue(userInfo, "PlayFabId");
        }

        public static void SetPlayFabId(this UserInfo userInfo, string playFabId)
        {
            SetValue(userInfo, "PlayFabId", playFabId);
        }

        public static string GetCharacterId(this UserInfo userInfo)
        {
            return GetValue(userInfo, "CharacterId");
        }

        public static void SetCharacterId(this UserInfo userInfo, string characterId)
        {
            SetValue(userInfo, "CharacterId", characterId);
        }

        public static string GetUsername(this UserInfo userInfo)
        {
            return GetValue(userInfo, "Username");
        }

        public static void SetUsername(this UserInfo userInfo, string username)
        {
            SetValue(userInfo, "Username", username);
        }

        public static string GetDisplayName(this UserInfo userInfo)
        {
            return GetValue(userInfo, "DisplayName");
        }

        public static void SetDisplayName(this UserInfo userInfo, string displayName)
        {
            SetValue(userInfo, "DisplayName", displayName);
        }

        public static string GetSessionTicket(this UserInfo userInfo)
        {
            return GetValue(userInfo, "SessionTicket");
        }

        public static void SetSessionTicket(this UserInfo userInfo, string sessionTicket)
        {
            SetValue(userInfo, "SessionTicket", sessionTicket);
        }

        private static void SetValue(UserInfo userInfo, string key, string value)
        {
            userInfo.CustomData.AddOrOverwrite(key, value);
        }

        private static string GetValue(UserInfo userInfo, string key)
        {
            if (userInfo.CustomData.TryGetValue(key, out string value))
            {
                return value;
            }

            return null;
        }
    }
}

#endif
