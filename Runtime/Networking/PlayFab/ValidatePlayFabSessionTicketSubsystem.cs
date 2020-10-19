//-----------------------------------------------------------------------
// <copyright file="ValidatePlayFabSessionTicketSubsystem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Threading.Tasks;
    using Lost.Networking;

    public class ValidatePlayFabSessionTicketSubsystem : IGameServerSubsystem
    {
#if !UNITY_2018_3_OR_NEWER
        private PlayFab.PlayFabAuthenticationContext titleAuthenticationContext;
#endif

        public void Initialize(GameServer gameServer)
        {
        }

        public Task<bool> Run()
        {
            return Task<bool>.FromResult(true);
        }

        public Task Shutdown()
        {
            return Task.Delay(0);
        }

        public async Task<bool> AllowPlayerToJoin(UserInfo userInfo)
        {
            var sessionTicket = userInfo.GetSessionTicket();

            if (string.IsNullOrEmpty(sessionTicket))
            {
                UnityEngine.Debug.LogError("ValidatePlayFabSessionTicketSubsystem requires all clients send their PlayFab session ticket in their CustomData in the \"SessionTicket\" key.");
                return false;
            }

#if UNITY_2018_3_OR_NEWER
            await Task.Delay(0);
            return true;
#else
            if (this.titleAuthenticationContext == null)
            {
                var getTitleAuthentication = await PlayFab.PlayFabAuthenticationAPI.GetEntityTokenAsync(new PlayFab.AuthenticationModels.GetEntityTokenRequest
                {
                    Entity = new PlayFab.AuthenticationModels.EntityKey
                    {
                        Id = PlayFab.PlayFabSettings.staticSettings.TitleId,
                        Type = "title",
                    }
                });

                this.titleAuthenticationContext = new PlayFab.PlayFabAuthenticationContext
                {
                    EntityId = getTitleAuthentication.Result.Entity.Id,
                    EntityType = getTitleAuthentication.Result.Entity.Type,
                    EntityToken = getTitleAuthentication.Result.EntityToken,
                };
            }

            var authenticate = await PlayFab.PlayFabServerAPI.AuthenticateSessionTicketAsync(new PlayFab.ServerModels.AuthenticateSessionTicketRequest
            {
                SessionTicket = sessionTicket,
                AuthenticationContext = this.titleAuthenticationContext,
            });

            if (authenticate.Error == null && authenticate.Result != null)
            {
                userInfo.SetPlayFabId(authenticate.Result.UserInfo.PlayFabId);
                userInfo.SetDisplayName(authenticate.Result.UserInfo.TitleInfo.DisplayName);

                return true;
            }
            else
            {
                return false;
            }
#endif
        }

        public Task UpdatePlayerInfo(UserInfo userInfo)
        {
            userInfo.SetSessionTicket(null);
            return Task.Delay(0);
        }
    }
}
