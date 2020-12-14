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
#if USING_ABLY
        private Dictionary<string, Type> messageTypes = new Dictionary<string, Type>();
        private HashSet<string> subscribedChannels = new HashSet<string>();
        private AblyRealtime ably;
#endif

        public override void Initialize()
        {
            this.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                yield return ReleasesManager.WaitForInitialization();
                yield return PlayFabManager.WaitForInitialization();

                var settings = ReleasesManager.Instance.CurrentRelease.RealtimeMessageManagerSettings;
                var isEnabled = settings.IsEnabled;
                var ablyKey = settings.AblyClientKey;

                if (isEnabled)
                {
#if !USING_ABLY
                    Debug.LogError("RealtimeMessageManager: Trying to use this manager, but the USING_ABLY define is not set." +
                                   "Make sure you have installed Ably 1.1.14 (https://github.com/ably/ably-dotnet/tree/1.1.14) and that the" +
                                   "USING_ABLY define is set or else this manager will not work.", this);
#else
                    if (ablyKey.IsNullOrWhitespace() == false)
                    {
                        this.InitilializeAbly(ablyKey);
                    }
                    else
                    {
                        Debug.LogError("RealtimeMessageManager requires a valid Ably Client Key in the Releases RealtimeMessageManagerSettings. " +
                                       "This manager will not work properly. Go to https://www.ably.io/accounts to get a valid client key.", this);
                    }
#endif
                }

                this.SetInstance(this);
            }
        }

#if USING_ABLY
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

        private void InitilializeAbly(string ablyKey)
        {
            this.ably = new AblyRealtime(ablyKey);
            this.Subscribe(PlayFabManager.Instance.User.PlayFabId);
        }
#endif

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            [SerializeField] private bool isEnabled;
            [SerializeField] private string ablyClientKey;
#pragma warning restore 0649

            public bool IsEnabled
            {
                get => this.isEnabled;
                set => this.isEnabled = value;
            }

            public string AblyClientKey
            {
                get => this.ablyClientKey;
                set => this.ablyClientKey = value;
            }
        }
    }
}
