﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="NetworkIdentityReleaseOwnershipRequest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public class NetworkIdentityReleaseOwnershipRequest : Message
    {
        public const short Id = 207;

        public long NetworkId { get; set; }

        public override short GetId()
        {
            return Id;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.NetworkId = (long)reader.ReadPackedUInt64();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.WritePackedUInt64((ulong)this.NetworkId);
        }
    }
}
