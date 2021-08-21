//-----------------------------------------------------------------------
// <copyright file="LoggingManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class LoggingManager : Manager<LoggingManager>
    {
        private const string LogEventName = "log_event";
        private const string LogTypeName = "log_type";
        private const string ConditionName = "condition";
        private const string HashCodeName = "hash_code";
        private const string CallstackName = "callstack";

        private List<ILoggingProvider> loggingProviders = new List<ILoggingProvider>();
        private HashSet<int> sentLogs = new HashSet<int>();
        private Settings settings;

        private Dictionary<string, object> eventArgsCache = new Dictionary<string, object>()
        {
            { LogTypeName, string.Empty },
            { ConditionName, string.Empty },
            { HashCodeName, 0 },
        };

        private Dictionary<LogType, string> logTypeCache = new Dictionary<LogType, string>
        {
            { LogType.Assert, "Assert" },
            { LogType.Error, "Error" },
            { LogType.Exception, "Exception" },
            { LogType.Log, "Log" },
            { LogType.Warning, "Warning" },
        };

        public override void Initialize()
        {
            this.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                if (Application.isEditor == false)
                {
                    yield return ReleasesManager.WaitForInitialization();

                    this.settings = ReleasesManager.Instance.CurrentRelease.LoggingManagerSettings;

                    Application.logMessageReceived += this.Application_logMessageReceived;
                }

                this.SetInstance(this);
            }
        }

        public void AddLoggingProvider(ILoggingProvider loggingProvider)
        {
            this.loggingProviders.Add(loggingProvider);
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            try
            {
                int stackTraceHashCode = stackTrace.GetHashCode();

                // Forward all Logging as an Analytic Event (if we haven't seen it before this session)
                if (this.settings.ForwardLoggingAsAnalyticsEvents && this.sentLogs.Contains(stackTraceHashCode) == false)
                {
                    // Making sure we don't send regular logs up if that flag is set
                    if (this.settings.DontForwardInfoLevelLogging == false || type != LogType.Log)
                    {
                        this.sentLogs.Add(stackTraceHashCode);

                        if (this.IsForwardingException(condition) == false)
                        {
                            this.eventArgsCache[LogTypeName] = this.logTypeCache[type];
                            this.eventArgsCache[ConditionName] = condition;
                            this.eventArgsCache[HashCodeName] = stackTraceHashCode;

                            // NOTE [bgish]: Currently can't do this, because it puts us over event size limits
                            this.eventArgsCache[CallstackName] = string.Empty; // stackTrace

                            Lost.Analytics.AnalyticsEvent.Custom(LogEventName, this.eventArgsCache);
                        }
                    }
                }

                // Sending the log to all the providers
                for (int i = 0; i < this.loggingProviders.Count; i++)
                {
                    try
                    {
                        this.loggingProviders[i].Log(condition, stackTrace, type);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private bool IsForwardingException(string condition)
        {
            if (this.settings?.ForwardingExceptions?.Count > 0)
            {
                for (int i = 0; i < this.settings.ForwardingExceptions.Count; i++)
                {
                    if (this.settings.ForwardingExceptions[i]?.IsNullOrWhitespace() == false &&
                        condition.Contains(this.settings.ForwardingExceptions[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            [SerializeField] private bool forwardLoggingAsAnalyticsEvents = true;
            [SerializeField] private bool dontForwardInfoLevelLogging = true;

            [Tooltip("Don't forward any log events that contain these strings.")]
            [SerializeField] private List<string> forwardingExceptions;
#pragma warning restore 0649

            public bool ForwardLoggingAsAnalyticsEvents
            {
                get => this.forwardLoggingAsAnalyticsEvents;
                set => this.forwardLoggingAsAnalyticsEvents = value;
            }

            public bool DontForwardInfoLevelLogging
            {
                get => this.dontForwardInfoLevelLogging;
                set => this.dontForwardInfoLevelLogging = value;
            }

            public List<string> ForwardingExceptions
            {
                get => this.forwardingExceptions;
                set => this.forwardingExceptions = value;
            }
        }
    }
}

#endif
