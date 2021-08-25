//-----------------------------------------------------------------------
// <copyright file="UserInfoManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Lost.Networking;
using System;

namespace Lost
{
    public class UserInfoManager : Manager<UserInfoManager>
    {
        public long UserId { get; private set; }

        public string UserHexId { get; private set; }

        public string DisplayName { get; private set; }

        public UserInfo GetMyUserInfo()
        {
            UserInfo userInfo = new UserInfo
            {
                UserId = this.UserId,
                UserHexId = this.UserHexId,
                DisplayName = this.DisplayName,
            };

            #if USING_PLAYFAB
            userInfo.SetSessionTicket(Lost.PlayFab.PlayFabManager.Instance.Login.SessionTicket);
            #endif

            return userInfo;
        }

        public override void Initialize()
        {
            #if !USING_PLAYFAB

            this.UserId = ((long)UnityEngine.Random.Range(int.MinValue, int.MaxValue) << 32) & ((long)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            this.UserHexId = this.UserId.ToString("X");
            this.DisplayName = $"Player{this.UserHexId.Substring(0, Math.Min(4, this.UserHexId.Length))}";
            this.SetInstance(this);

            #else

            this.StartCoroutine(Initialize());

            System.Collections.IEnumerator Initialize()
            {
                yield return Lost.PlayFab.PlayFabManager.WaitForInitialization();
                this.UserId = Lost.PlayFab.PlayFabManager.Instance.User.PlayFabNumericId;
                this.UserHexId = Lost.PlayFab.PlayFabManager.Instance.User.PlayFabId;
                this.DisplayName = Lost.PlayFab.PlayFabManager.Instance.User.DisplayName;

                this.SetInstance(this);
            }

            #endif
        }
    }
}
