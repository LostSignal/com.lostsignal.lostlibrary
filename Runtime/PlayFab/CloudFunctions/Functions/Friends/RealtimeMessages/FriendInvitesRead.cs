#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="FriendInvitesRead.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions.Friends
{
    public sealed class FriendInvitesRead : RealtimeMessage
    {
        public override string Type => nameof(FriendInvitesRead);
    }
}
