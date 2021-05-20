//-----------------------------------------------------------------------
// <copyright file="PlayFabManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::PlayFab;
    using global::PlayFab.ClientModels;
    using global::PlayFab.Internal;
    using Lost.IAP;
    using Newtonsoft.Json;
    using UnityEngine;

    public class PlayFabManager : Manager<PlayFabManager>
    {
        public delegate void Action<T1, T2, T3, T4, T5>(T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        private static ISerializerPlugin serializerPlugin;

#pragma warning disable 0649
        [SerializeField] private LoginMethod loginMethod;
#pragma warning restore 0649

        private bool regainFocusCoroutineRunning;
        private DateTime lostFocusTime = DateTime.UtcNow;

        public Action<PlayFabRequestCommon, PlayFabResultCommon> GlobalPlayFabResultHandler { get; set; }

        public CatalogManager Catalog { get; private set; }

        public CharacterManager Character { get; private set; }

        public InventoryManager Inventory { get; private set; }

        public LoginManager Login { get; private set; }

        public PurchasingManager Purchasing { get; private set; }

        public PushNotificationManager PushNotifications { get; private set; }

        public StoreManager Store { get; private set; }

        public TitleDataManager TitleData { get; private set; }

        public TitleNewsManager TitleNews { get; private set; }

        public UserManager User { get; private set; }

        public VirtualCurrencyManager VirtualCurrency { get; private set; }

        public static long ConvertPlayFabIdToLong(string playfabId)
        {
            return System.Convert.ToInt64(playfabId, 16);
        }

        public static ISerializerPlugin SerializerPlugin
        {
            get
            {
                if (serializerPlugin == null)
                {
                    serializerPlugin = global::PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                }

                return serializerPlugin;
            }
        }

        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                PlayFabSettings.GlobalErrorHandler += this.OnGlobalErrorEvent;

                yield return ReleasesManager.WaitForInitialization();
                yield return UnityPurchasingManager.WaitForInitialization();

                PlayFabManager.Settings playfabSettings = ReleasesManager.Instance.CurrentRelease.PlayfabManagerSettings;

                string catalogVersion = playfabSettings.CatalogVersion;
                PlayFabSettings.staticSettings.TitleId = Lost.BuildConfig.RuntimeBuildConfig.Instance.GetString(PlayFabConfigExtensions.TitleId);

                this.Login = new LoginManager(this);

                // Starting the Logging In loop
                var combinedParams = new GetPlayerCombinedInfoRequestParams
                {
                    GetTitleData = playfabSettings.LoadTitleDataKeys.Count > 0,
                    TitleDataKeys = playfabSettings.LoadTitleDataKeys,
                    GetUserInventory = playfabSettings.LoadInventory,
                    GetUserVirtualCurrency = playfabSettings.LoadVirtualCurrency,
                    GetPlayerProfile = playfabSettings.LoadPlayerProfile,
                    GetCharacterList = playfabSettings.LoadCharacters,
                    GetUserAccountInfo = true,
                };

                LostMessages.BootloaderLoggingIn();
                var login = this.Login.Login(this.loginMethod, combinedParams, playfabSettings.FacebookPermissions);

                yield return login;

                this.Catalog = new CatalogManager(this, catalogVersion);
                this.Character = new CharacterManager(this, login.Value?.InfoResultPayload?.CharacterList);
                this.Inventory = new InventoryManager(this, login.Value?.InfoResultPayload?.UserInventory);
                this.Purchasing = new PurchasingManager(this, catalogVersion);

                this.PushNotifications = new PushNotificationManager(this);
                this.Store = new StoreManager(this, catalogVersion);
                this.TitleData = new TitleDataManager(this, login.Value?.InfoResultPayload?.TitleData);
                this.TitleNews = new TitleNewsManager(this);
                this.User = new UserManager(this, login.Value);
                this.VirtualCurrency = new VirtualCurrencyManager(this, login.Value?.InfoResultPayload?.UserVirtualCurrency);

                // Catalog
                if (playfabSettings.LoadCatalog)
                {
                    LostMessages.BootloaderDownloadingCatalog();
                    yield return this.Catalog.GetCatalog();
                }

                // Stores
                if (playfabSettings.LoadStoresAtStartup?.Count > 0)
                {
                    LostMessages.BootloaderLoadingStores();

                    foreach (var store in playfabSettings.LoadStoresAtStartup)
                    {
                        // TODO [bgish]: Tell the loading dialog that we're getting store "X"
                        yield return this.Store.GetStore(store);
                    }
                }

                // Initializing purchasing, but no need to wait on it later
                if (playfabSettings.LoadPurchasing)
                {
                    LostMessages.BootloaderInitializingPurchasing();
                    yield return this.Purchasing.Initialize();
                }

                // Push Notifications
                if (Application.platform == RuntimePlatform.IPhonePlayer && playfabSettings.RegisterIosPushNotificationsAtStartup)
                {
                    this.PushNotifications.RegisterPushNotificationsWithPlayFab();
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    this.PushNotifications.RegisterPushNotificationsWithPlayFab();
                }

                this.SetInstance(this);
            }
        }

        public UnityTask<ExecuteCloudScriptResult> Do(ExecuteCloudScriptRequest request)
        {
            return Do<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>(request, PlayFabClientAPI.ExecuteCloudScriptAsync);
        }

        public UnityTask<EmptyResponse> Do(UpdateAvatarUrlRequest request)
        {
            return Do<UpdateAvatarUrlRequest, EmptyResponse>(request, PlayFabClientAPI.UpdateAvatarUrlAsync);
        }

        public UnityTask<UpdateUserTitleDisplayNameResult> Do(UpdateUserTitleDisplayNameRequest request)
        {
            return Do<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResult>(request, PlayFabClientAPI.UpdateUserTitleDisplayNameAsync);
        }

        public UnityTask<GetAccountInfoResult> Do(GetAccountInfoRequest request)
        {
            return Do<GetAccountInfoRequest, GetAccountInfoResult>(request, PlayFabClientAPI.GetAccountInfoAsync);
        }

        public UnityTask<GetContentDownloadUrlResult> Do(GetContentDownloadUrlRequest request)
        {
            return Do<GetContentDownloadUrlRequest, GetContentDownloadUrlResult>(request, PlayFabClientAPI.GetContentDownloadUrlAsync);
        }

        public UnityTask<GetUserInventoryResult> Do(GetUserInventoryRequest request)
        {
            return Do<GetUserInventoryRequest, GetUserInventoryResult>(request, PlayFabClientAPI.GetUserInventoryAsync);
        }

        public UnityTask<GetPlayerCombinedInfoResult> Do(GetPlayerCombinedInfoRequest request)
        {
            return Do<GetPlayerCombinedInfoRequest, GetPlayerCombinedInfoResult>(request, PlayFabClientAPI.GetPlayerCombinedInfoAsync);
        }

        public UnityTask<GetPlayerSegmentsResult> Do(GetPlayerSegmentsRequest request)
        {
            return Do<GetPlayerSegmentsRequest, GetPlayerSegmentsResult>(request, PlayFabClientAPI.GetPlayerSegmentsAsync);
        }

        public UnityTask<WriteEventResponse> Do(WriteClientPlayerEventRequest request)
        {
            return Do<WriteClientPlayerEventRequest, WriteEventResponse>(request, PlayFabClientAPI.WritePlayerEventAsync);
        }

        public UnityTask<GetTimeResult> RefreshServerTime()
        {
            return Do<GetTimeRequest, GetTimeResult>(new GetTimeRequest(), PlayFabClientAPI.GetTimeAsync);
        }

        #region User Data Related Functions

        public UnityTask<GetUserDataResult> Do(GetUserDataRequest request)
        {
            return Do<GetUserDataRequest, GetUserDataResult>(request, PlayFabClientAPI.GetUserDataAsync);
        }

        public UnityTask<UpdateUserDataResult> Do(UpdateUserDataRequest request)
        {
            return Do<UpdateUserDataRequest, UpdateUserDataResult>(request, PlayFabClientAPI.UpdateUserDataAsync);
        }

        #endregion

        #region Player Statistics Related Functions

        public UnityTask<GetPlayerStatisticsResult> Do(GetPlayerStatisticsRequest request)
        {
            return Do<GetPlayerStatisticsRequest, GetPlayerStatisticsResult>(request, PlayFabClientAPI.GetPlayerStatisticsAsync);
        }

        #endregion

        #region Title Data Related Functions

        public UnityTask<GetTitleDataResult> Do(GetTitleDataRequest request)
        {
            return Do<GetTitleDataRequest, GetTitleDataResult>(request, PlayFabClientAPI.GetTitleDataAsync);
        }

        public UnityTask<GetTitleNewsResult> Do(GetTitleNewsRequest request)
        {
            return Do<GetTitleNewsRequest, GetTitleNewsResult>(request, PlayFabClientAPI.GetTitleNewsAsync);
        }

        #endregion

        #region Push Notification Related Functions

        public UnityTask<AndroidDevicePushNotificationRegistrationResult> Do(AndroidDevicePushNotificationRegistrationRequest request)
        {
            return Do<AndroidDevicePushNotificationRegistrationRequest, AndroidDevicePushNotificationRegistrationResult>(request, PlayFabClientAPI.AndroidDevicePushNotificationRegistrationAsync);
        }

        public UnityTask<RegisterForIOSPushNotificationResult> Do(RegisterForIOSPushNotificationRequest request)
        {
            return Do<RegisterForIOSPushNotificationRequest, RegisterForIOSPushNotificationResult>(request, PlayFabClientAPI.RegisterForIOSPushNotificationAsync);
        }

        #endregion

        #region Leaderboard Related Functions

        public UnityTask<GetLeaderboardResult> Do(GetLeaderboardRequest request)
        {
            return Do<GetLeaderboardRequest, GetLeaderboardResult>(request, PlayFabClientAPI.GetLeaderboardAsync);
        }

        public UnityTask<GetLeaderboardResult> Do(GetFriendLeaderboardRequest request)
        {
            return Do<GetFriendLeaderboardRequest, GetLeaderboardResult>(request, PlayFabClientAPI.GetFriendLeaderboardAsync);
        }

        public UnityTask<GetLeaderboardAroundPlayerResult> Do(GetLeaderboardAroundPlayerRequest request)
        {
            return Do<GetLeaderboardAroundPlayerRequest, GetLeaderboardAroundPlayerResult>(request, PlayFabClientAPI.GetLeaderboardAroundPlayerAsync);
        }

        public UnityTask<GetFriendLeaderboardAroundPlayerResult> Do(GetFriendLeaderboardAroundPlayerRequest request)
        {
            return Do<GetFriendLeaderboardAroundPlayerRequest, GetFriendLeaderboardAroundPlayerResult>(request, PlayFabClientAPI.GetFriendLeaderboardAroundPlayerAsync);
        }

        #endregion

        #region Purchasing Related Functions

        public UnityTask<ConfirmPurchaseResult> Do(ConfirmPurchaseRequest request)
        {
            return Do<ConfirmPurchaseRequest, ConfirmPurchaseResult>(request, PlayFabClientAPI.ConfirmPurchaseAsync);
        }

        public UnityTask<ValidateIOSReceiptResult> Do(ValidateIOSReceiptRequest request)
        {
            return Do<ValidateIOSReceiptRequest, ValidateIOSReceiptResult>(request, PlayFabClientAPI.ValidateIOSReceiptAsync);
        }

        public UnityTask<ValidateGooglePlayPurchaseResult> Do(ValidateGooglePlayPurchaseRequest request)
        {
            return Do<ValidateGooglePlayPurchaseRequest, ValidateGooglePlayPurchaseResult>(request, PlayFabClientAPI.ValidateGooglePlayPurchaseAsync);
        }

        public UnityTask<ValidateAmazonReceiptResult> Do(ValidateAmazonReceiptRequest request)
        {
            return Do<ValidateAmazonReceiptRequest, ValidateAmazonReceiptResult>(request, PlayFabClientAPI.ValidateAmazonIAPReceiptAsync);
        }

        #endregion

        #region Friends Related Functions

        public UnityTask<AddFriendResult> Do(AddFriendRequest request)
        {
            // should update the cached list after it's done
            return Do<AddFriendRequest, AddFriendResult>(request, PlayFabClientAPI.AddFriendAsync);
        }

        public UnityTask<RemoveFriendResult> Do(RemoveFriendRequest request)
        {
            // should update the cached list after it's done
            return Do<RemoveFriendRequest, RemoveFriendResult>(request, PlayFabClientAPI.RemoveFriendAsync);
        }

        public UnityTask<GetFriendsListResult> Do(GetFriendsListRequest request)
        {
            // TODO [bgish] - should really cache this, and update friends list whenever add/remove.  If cached, then return the list instead of call this
            return Do<GetFriendsListRequest, GetFriendsListResult>(request, PlayFabClientAPI.GetFriendsListAsync);
        }

        #endregion

        #region Matchmaking Related Functions

        public UnityTask<CurrentGamesResult> Do(CurrentGamesRequest request)
        {
            return Do<CurrentGamesRequest, CurrentGamesResult>(request, PlayFabClientAPI.GetCurrentGamesAsync);
        }

        public UnityTask<MatchmakeResult> Do(MatchmakeRequest request)
        {
            return Do<MatchmakeRequest, MatchmakeResult>(request, PlayFabClientAPI.MatchmakeAsync);
        }

        public UnityTask<StartGameResult> Do(StartGameRequest request)
        {
            return Do<StartGameRequest, StartGameResult>(request, PlayFabClientAPI.StartGameAsync);
        }

        #endregion

        public UnityTask<Result> Do<Request, Result>(Request request, Func<Request, object, Dictionary<string, string>, Task<PlayFabResult<Result>>> playfabFunction)
            where Request : PlayFabRequestCommon
            where Result : PlayFabResultCommon
        {
            return UnityTask<Result>.Run(DoIterator(request, playfabFunction));
        }

        public IEnumerator<Result> DoIterator<Request, Result>(Request request, Func<Request, object, Dictionary<string, string>, Task<PlayFabResult<Result>>> playfabFunction)
            where Request : PlayFabRequestCommon
            where Result : PlayFabResultCommon
        {
            var task = playfabFunction.Invoke(request, null, null);

            while (task.IsCompleted == false)
            {
                yield return null;
            }

            if (task.Result.Error != null)
            {
                throw new PlayFabException(task.Result.Error);
            }

            // Checking cloud script logging for errors
            if (task.Result.Result is ExecuteCloudScriptResult executeCloudScriptResult)
            {
                for (int i = 0; i < executeCloudScriptResult.Logs.Count; i++)
                {
                    if (executeCloudScriptResult.Logs[i].Level == "Error")
                    {
                        throw new PlayFabCloudScriptException(executeCloudScriptResult.Logs[i].Message);
                    }
                }
            }

            GlobalPlayFabResultHandler?.Invoke(request, task.Result.Result);

            yield return task.Result.Result;
        }

        private void OnDestroy()
        {
            PlayFabSettings.GlobalErrorHandler -= this.OnGlobalErrorEvent;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                double minutesSinceLostFocus = this.lostFocusTime.Subtract(DateTime.UtcNow).TotalMinutes;

                if (minutesSinceLostFocus > 1)
                {
                    this.StartCoroutine(RefreshServerTimeCoroutine());
                }
            }
            else
            {
                this.lostFocusTime = DateTime.UtcNow;
            }

            IEnumerator RefreshServerTimeCoroutine()
            {
                if (this.regainFocusCoroutineRunning)
                {
                    yield break;
                }

                this.regainFocusCoroutineRunning = true;

                // NOTE [bgish]: We're getting the server time on application focus because if we've been gone for too long
                //               then it will fail with PlayFabErrorCode.InvalidSessionTicket and we'll reboot.
                yield return this.RefreshServerTime();

                this.regainFocusCoroutineRunning = false;
            }
        }

        private void OnGlobalErrorEvent(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.InvalidSessionTicket)
            {
                this.ExecuteAtEndOfFrame(Bootloader.Reboot);
            }
        }

        public enum TitleDataSerializationMethod
        {
            PlayFab,
            JsonDotNet,
            Unity,
        }

        public interface ITitleData
        {
            string TitleDataKey { get; }

            bool IsCompressed { get; }

            bool LoadAtStartup { get; }

            TitleDataSerializationMethod SerializationMethod { get; }
        }

        [Serializable]
        public class TitleDataKeys
        {
            public string TitleDataKey;
            public bool IsCompressed;
            public bool LoadAtStartup;
            public TitleDataSerializationMethod SerializationMethod;
        }

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            [SerializeField] private string catalogVersion = "1.0";
            [SerializeField] private List<string> facebookPermissions = new List<string> { "public_profile", "email", "user_friends" };
            [SerializeField] private List<string> loadTitleDataKeys;
            [SerializeField] private List<string> loadStoresAtStartup;
            [SerializeField] private bool loadInventory;
            [SerializeField] private bool loadVirtualCurrency;
            [SerializeField] private bool loadPlayerProfile;
            [SerializeField] private bool loadCatalog;
            [SerializeField] private bool loadCharacters;
            [SerializeField] private bool loadPurchasing;
            [SerializeField] private bool registerIosPushNotificationsAtStartup;
#pragma warning restore 0649

            public string CatalogVersion
            {
                get => this.catalogVersion;
                set => this.catalogVersion = value;
            }

            public List<string> FacebookPermissions
            {
                get => this.facebookPermissions;
                set => this.facebookPermissions = value;
            }

            public List<string> LoadTitleDataKeys
            {
                get => this.loadTitleDataKeys;
                set => this.loadTitleDataKeys = value;
            }

            public List<string> LoadStoresAtStartup
            {
                get => this.loadStoresAtStartup;
                set => this.loadStoresAtStartup = value;
            }

            public bool LoadInventory
            {
                get => this.loadInventory;
                set => this.loadInventory = value;
            }

            public bool LoadVirtualCurrency
            {
                get => this.loadVirtualCurrency;
                set => this.loadVirtualCurrency = value;
            }

            public bool LoadPlayerProfile
            {
                get => this.loadPlayerProfile;
                set => this.loadPlayerProfile = value;
            }

            public bool LoadCatalog
            {
                get => this.loadCatalog;
                set => this.loadCatalog = value;
            }

            public bool LoadCharacters
            {
                get => this.loadCharacters;
                set => this.loadCharacters = value;
            }

            public bool LoadPurchasing
            {
                get => this.loadPurchasing;
                set => this.loadPurchasing = value;
            }

            public bool RegisterIosPushNotificationsAtStartup
            {
                get => this.registerIosPushNotificationsAtStartup;
                set => this.registerIosPushNotificationsAtStartup = value;
            }
        }
    }
}
