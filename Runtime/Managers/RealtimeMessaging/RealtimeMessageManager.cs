//-----------------------------------------------------------------------
// <copyright file="RealtimeMessageManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    #if USING_ABLY
    using IO.Ably;
    #endif

    using Lost.AppConfig;
    using Newtonsoft.Json.Linq;
    using global::PlayFab.ClientModels;
    using global::PlayFab.Internal;
    using UnityEngine;
    using Lost.PlayFab;
    using System.Collections;

    public class RealtimeMessageManager : Manager<RealtimeMessageManager>
    {
        #pragma warning disable 0649
        [Header("Dependencies")]
        [SerializeField] private ReleasesManager releasesManager;
        [SerializeField] private PlayFabManager playfabManager;
        #pragma warning restore 0649

        [ExposeInEditor("Go To ably.io")]
        public void GoToAblyWebsite()
        {
            Application.OpenURL("https://www.ably.io/accounts");
        }

        #if !USING_ABLY

        public override void Initialize()
        {
            Debug.LogError("Trying to use RealtimeMessageManager without USING_ABLY define set.");
        }

        [ExposeInEditor("Download Ably 1.1.14")]
        public void DownloadAbly()
        {
            Application.OpenURL("https://github.com/ably/ably-dotnet/tree/1.1.14");
        }

        [ExposeInEditor("Add USING_ABLY Define")]
        public void AddUsingAblyDefine()
        {
            ProjectDefinesHelper.AddDefineToProject("USING_ABLY");
        }

        #else

        private Dictionary<string, Type> messageTypes = new Dictionary<string, Type>();
        private HashSet<string> subscribedChannels = new HashSet<string>();
        private AblyRealtime ably;

        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                yield return this.WaitForDependencies(this.releasesManager, this.playfabManager);

                this.ably = new AblyRealtime(this.releasesManager.CurrentRelease.AblyClientKey);
                this.Subscribe(this.playfabManager.User.PlayFabId);
                this.SetInstance(this);
            }
        }

        public void RegisterType<T>() where T : RealtimeMessage, new()
        {
            this.messageTypes.Add(typeof(T).Name, typeof(T));
        }

        public void Subscribe(string channel)
        {
            if (this.subscribedChannels.Contains(channel) == false)
            {
                this.ably.Channels.Get(channel).Subscribe(this.MessageReceived);
                this.subscribedChannels.Add(channel);
            }
        }

        public void Unsubscribe(string channel)
        {
            if (this.subscribedChannels.Contains(channel))
            {
                this.ably.Channels.Get(channel).Unsubscribe(this.MessageReceived);
                this.subscribedChannels.Remove(channel);
            }
        }

        private void MessageReceived(Message message)
        {
            string json = message.Data as string;
            JObject jObject = null;

            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                Debug.LogError($"Received RealtimeMessage with invalid Json: {json}");
                return;
            }

            string realtimeMessageType = jObject["Type"]?.ToString();

            if (realtimeMessageType == null)
            {
                Debug.LogError($"Received Json that was not a RealtimeMessage: {json}");
            }
            else if (this.messageTypes.TryGetValue(realtimeMessageType, out Type type))
            {
                var realtimeMessage = JsonUtil.Deserialize(json, type);

                // TODO [bgish]: Forward realtimeMessage onto the message subscription system

                Debug.Log($"Received RealtimeMessage of type {realtimeMessageType} and json: {json}");
            }
            else
            {
                Debug.LogError($"Received RealtimeMessage of unknown type {realtimeMessageType}");
            }
        }

        #endif
    }
}
