//-----------------------------------------------------------------------
// <copyright file="NetworkIdentityUpdate.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using UnityEngine;

    public class NetworkIdentityUpdate : Message
    {
        public const short Id = 203;

        public long NetworkId { get; set; }
        public long OwnerId { get; set; }
        public bool IsEnabled { get; set; }
        public string ResourceName { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public bool CanChangeOwner { get; set; }

        public override short GetId()
        {
            return Id;
        }

#if UNITY

        public void PopulateMessage(NetworkIdentity identity)
        {
            this.NetworkId = identity.NetworkId;
            this.OwnerId = identity.OwnerId;
            this.IsEnabled = identity.gameObject.activeSelf;
            this.ResourceName = identity.ResourceName;
            this.CanChangeOwner = identity.CanChangeOwner;
            this.Position = NetworkTransformAnchor.InverseTransformPosition(identity.transform.position);
            this.Rotation = NetworkTransformAnchor.InverseTransformRotation(identity.transform.rotation);
        }

        public void PopulateNetworkIdentity(NetworkIdentity identity, bool updatePositionAndRotation)
        {
            identity.SetOwner(this.OwnerId, this.CanChangeOwner);
            identity.gameObject.SafeSetActive(this.IsEnabled);
            identity.ResourceName = identity.ResourceName;

            if (updatePositionAndRotation)
            {
                identity.transform.position = NetworkTransformAnchor.TransformPosition(this.Position);
                identity.transform.rotation = NetworkTransformAnchor.TransformRotation(this.Rotation);
            }
        }

#endif

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.NetworkId = (long)reader.ReadPackedUInt64();
            this.OwnerId = (long)reader.ReadPackedUInt64();
            this.IsEnabled = reader.ReadBoolean();
            this.ResourceName = reader.ReadString();
            this.Position = reader.ReadVector3();
            this.Rotation = reader.ReadQuaternion();
            this.CanChangeOwner = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.WritePackedUInt64((ulong)this.NetworkId);
            writer.WritePackedUInt64((ulong)this.OwnerId);
            writer.Write(this.IsEnabled);
            writer.Write(this.ResourceName);
            writer.Write(this.Position);
            writer.Write(this.Rotation);
            writer.Write(this.CanChangeOwner);
        }
    }
}
