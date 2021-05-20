//-----------------------------------------------------------------------
// <copyright file="Room.cs" company="Giant Cranium">
//     Copyright (c) Giant Cranium. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions
{
    public enum RoomVisibility
    {
        Members,
        OrgMembers,
        Public,
    }

    public class Room
    {
        public string Id { get; set; }

        public RoomInfo Info { get; set; }

        public string OwnerId { get; set; }

        public string OrgId { get; set; }

        public RoomVisibility Visibility { get; set; }
    }
}
