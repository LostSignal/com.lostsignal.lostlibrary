//-----------------------------------------------------------------------
// <copyright file="NetworkIdentityRequestUpdate.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using UnityEngine;

    public class NetworkIdentityRequestUpdate : Message
    {
        public const short Id = 204;

        public long NetworkId { get; set; }

        public bool IsEnabled { get; set; }

        public string ResourceName { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public int BehaviourCount { get; set; }

        public bool DestoryOnDisconnect { get; set; }

        public bool CanChangeOwner { get; set; }

        public override short GetId()
        {
            return Id;
        }

#if UNITY
        public void PopulateMessage(NetworkIdentity identity)
        {
            this.NetworkId = identity.NetworkId;
            this.IsEnabled = identity.gameObject.activeSelf;
            this.ResourceName = identity.ResourceName;
            this.Position = identity.transform.position;
            this.Rotation = identity.transform.rotation;
            this.BehaviourCount = identity.Behaviours.Length;
            this.DestoryOnDisconnect = identity.DestoryOnDisconnect;
            this.CanChangeOwner = identity.CanChangeOwner;
        }

#endif

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.NetworkId = (long)reader.ReadPackedUInt64();
            this.IsEnabled = reader.ReadBoolean();
            this.ResourceName = reader.ReadString();
            this.Position = reader.ReadVector3();
            this.Rotation = reader.ReadQuaternion();
            this.BehaviourCount = (int)reader.ReadPackedUInt32();
            this.DestoryOnDisconnect = reader.ReadBoolean();
            this.CanChangeOwner = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.WritePackedUInt64((ulong)this.NetworkId);
            writer.Write(this.IsEnabled);
            writer.Write(this.ResourceName);
            writer.Write(this.Position);
            writer.Write(this.Rotation);
            writer.WritePackedUInt32((uint)this.BehaviourCount);
            writer.Write(this.DestoryOnDisconnect);
            writer.Write(this.CanChangeOwner);
        }
    }
}
