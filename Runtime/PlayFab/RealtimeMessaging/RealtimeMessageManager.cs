//-----------------------------------------------------------------------
// <copyright file="RealtimeMessageManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB

namespace Lost
{
    using System;
    using System.Collections.Generic;

#if USING_ABLY
    using IO.Ably;
#endif

    using System.Collections;
    using global::PlayFab.ClientModels;
    using global::PlayFab.Internal;
    using Lost.BuildConfig;
    using Lost.PlayFab;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    public sealed class RealtimeMessageManager : Manager<RealtimeMessageManager>
    {
        #if USING_ABLY
        private readonly Dictionary<string, Type> messageTypes = new Dictionary<string, Type>();
        private readonly HashSet<string> subscribedChannels = new HashSet<string>();
        #endif

        #pragma warning disable 0649
        [SerializeField] private bool printDebugOutput;
        #pragma warning restore 0649

        #if USING_ABLY
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
                        Debug.LogError(
                            "RealtimeMessageManager requires a valid Ably Client Key in the Releases RealtimeMessageManagerSettings. " +
                            "This manager will not work properly. Go to https://www.ably.io/accounts to get a valid client key.", this);
                    }
#endif
                }

                this.SetInstance(this);
            }
        }

        public void RegisterType<T>()
            where T : RealtimeMessage, new()
        {
#if USING_ABLY
            var instance = Activator.CreateInstance(typeof(T)) as RealtimeMessage;
            this.messageTypes.Add(instance.Type, typeof(T));
#endif
        }

        public void Subscribe(string channel)
        {
#if USING_ABLY
            if (this.subscribedChannels.Contains(channel) == false)
            {
                this.ably.Channels.Get(channel).Subscribe(this.MessageReceived);
                this.subscribedChannels.Add(channel);
            }
#endif
        }

        public void Unsubscribe(string channel)
        {
#if USING_ABLY
            if (this.subscribedChannels.Contains(channel))
            {
                this.ably.Channels.Get(channel).Unsubscribe(this.MessageReceived);
                this.subscribedChannels.Remove(channel);
            }
#endif
        }

#if USING_ABLY
        private void MessageReceived(Message message)
        {
            string json = message.Data as string;
            JObject javaObject;

            try
            {
                javaObject = JObject.Parse(json);
            }
            catch
            {
                Debug.LogError($"Received RealtimeMessage with invalid Json: {json}");
                return;
            }

            string realtimeMessageType = javaObject["Type"]?.ToString();

            if (realtimeMessageType == null)
            {
                Debug.LogError($"Received Json that was not a RealtimeMessage: {json}");
            }
            else if (this.messageTypes.TryGetValue(realtimeMessageType, out Type type))
            {
                var realtimeMessage = JsonUtil.Deserialize(json, type);

                //// TODO [bgish]: Forward realtimeMessage onto the message subscription system

                if (this.printDebugOutput)
                {
                    Debug.Log($"Received RealtimeMessage of type {realtimeMessageType} and json: {json}");
                }
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
#pragma warning disable 0649, CA2235
            [SerializeField] private bool isEnabled;
            [SerializeField] private string ablyClientKey;
#pragma warning restore 0649, CA2235

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

#endif
