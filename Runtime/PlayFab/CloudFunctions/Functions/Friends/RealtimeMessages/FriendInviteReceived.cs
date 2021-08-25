//-----------------------------------------------------------------------
// <copyright file="FriendInviteReceived.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || USING_PLAYFAB

namespace Lost.CloudFunctions.Friends
{
    public sealed class FriendInviteReceived : RealtimeMessage
    {
        public override string Type => nameof(FriendInviteReceived);
    }
}

#endif
