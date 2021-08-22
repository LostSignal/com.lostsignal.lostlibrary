//-----------------------------------------------------------------------
// <copyright file="DissonanceMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE

namespace Lost.DissonanceIntegration
{
    using Lost.Networking;

    public class DissonanceMessage : Message
    {
        public const short MessageId = 501;

        public bool IsReliable { get; set; }

        public long PlayerId { get; set; }

        public byte[] Data { get; set; }

        public override short GetId()
        {
            return MessageId;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.Write(this.IsReliable);
            writer.Write(this.PlayerId);
            writer.WriteBytesFull(this.Data);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.IsReliable = reader.ReadBoolean();
            this.PlayerId = reader.ReadInt64();
            this.Data = reader.ReadBytesAndSize();
        }

        public DissonanceMessage Copy()
        {
            byte[] newData = new byte[this.Data.Length];
            this.Data.CopyTo(newData, 0);

            return new DissonanceMessage
            {
                IsReliable = this.IsReliable,
                PlayerId = this.PlayerId,
                Data = newData,
            };
        }
    }
}

#endif
