#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="RoomServerInfo.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions
{
    using System.Collections.Generic;
    using global::PlayFab.MultiplayerModels;

    public class RoomServerInfo
    {
        public string RoomId { get; set; }

        public string BuildId { get; set; }

        public string SessionId { get; set; }

        public string ServerId { get; set; }

        public string Region { get; set; }

        public string FQDN { get; set; }

        public List<Port> Ports { get; set; }
    }
}
