﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="PlayFabContextClasses.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable

namespace Lost.CloudFunctions
{
    using System.Collections.Generic;
    using global::PlayFab;
    using global::PlayFab.ProfilesModels;

    public class TitleAuthenticationContext
    {
        public string Id { get; set; }

        public string EntityToken { get; set; }
    }

    public class FunctionExecutionContext<T>
    {
        public EntityProfileBody CallerEntityProfile { get; set; }

        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }

        public T FunctionArgument { get; set; }
    }

    public class FunctionExecutionContext : FunctionExecutionContext<string>
    {
        public FunctionExecutionContext(string argument, string playfabId, string titleEntityToken)
        {
            this.FunctionArgument = argument;

            this.TitleAuthenticationContext = new TitleAuthenticationContext
            {
                Id = PlayFabSettings.staticSettings.TitleId,
                EntityToken = titleEntityToken,
            };

            this.CallerEntityProfile = new EntityProfileBody
            {
                Lineage = new EntityLineage
                {
                    MasterPlayerAccountId = playfabId,
                },
            };
        }

        public FunctionExecutionContext()
        {
        }
    }

    public class PlaystreamExecutionContext
    {
        public PlayStreamEventEnvelope PlayStreamEventEnvelope { get; set; }

        public PlayerProfile PlayerProfile { get; set; }

        public bool PlayerProfileTruncated { get; set; }

        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }

        public bool GeneratePlayStreamEvent { get; set; }
    }

    public class PlayStreamEventEnvelope
    {
        public string EntityId { get; set; }

        public string EntityType { get; set; }

        public string EventName { get; set; }

        public string EventNamespace { get; set; }

        public string EventData { get; set; }
    }

    public class Statistics
    {
    }

    public class ValuesToDate
    {
    }

    public class Locations
    {
    }

    public class VirtualCurrencyBalances
    {
    }

    public class PlayerProfile
    {
        public string PlayerId { get; set; }

        public string TitleId { get; set; }

        public string PublisherId { get; set; }

        public Statistics Statistics { get; set; }

        public ValuesToDate ValuesToDate { get; set; }

        public List<object> Tags { get; set; }

        public Locations Locations { get; set; }

        public VirtualCurrencyBalances VirtualCurrencyBalances { get; set; }

        public List<object> AdCampaignAttributions { get; set; }

        public List<object> PushNotificationRegistrations { get; set; }

        public List<object> LinkedAccounts { get; set; }

        public List<object> PlayerStatistics { get; set; }

        public List<object> PlayerMemberships { get; set; }

        public List<object> ContactEmailAddresses { get; set; }

        public List<object> PlayerExperimentVariants { get; set; }
    }
}
