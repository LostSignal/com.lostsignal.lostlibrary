#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="FriendInviteDeclined.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions.Friends
{
    public sealed class FriendInviteDeclined : RealtimeMessage
    {
        public override string Type => nameof(FriendInviteDeclined);
    }
}
