//-----------------------------------------------------------------------
// <copyright file="FriendsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Friends
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Lost.CloudFunctions.Friends;
    using UnityEngine;

    public class FriendsManager : Manager<FriendsManager>
    {
#pragma warning disable 0649
        [SerializeField] private RealtimeMessageManager messageManager;
#pragma warning restore 0649

        public override void Initialize()
        {
            CoroutineRunner.Instance.StartCoroutine(InitializeCoroutine());

            IEnumerator InitializeCoroutine()
            {
                yield return this.WaitForDependencies(this.messageManager);

                this.messageManager.RegisterType<FriendInviteAccepted>();
                this.messageManager.RegisterType<FriendInviteDeclined>();
                this.messageManager.RegisterType<FriendInviteReceived>();
                this.messageManager.RegisterType<FriendInvitesRead>();

                this.SetInstance(this);
            }
        }

        public List<Friend> GetFriends()
        {
            throw new NotImplementedException();
        }

        public List<FriendInvite> GetFriendInvites()
        {
            throw new NotImplementedException();
        }

        public void SendFriendInvite(string playfabId)
        {
            throw new NotImplementedException();
        }

        public void AcceptFriendInvite(string playfabId)
        {
            throw new NotImplementedException();
        }

        public void DeclineFriendInvite(string playfabId)
        {
            throw new NotImplementedException();
        }

        public void MarkFriendInvitesRead()
        {
            throw new NotImplementedException();
        }

        public List<Friend> FindFriendsByDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public List<Friend> FindFriendsByEmail(List<string> emails)
        {
            throw new NotImplementedException();
        }
    }
}
