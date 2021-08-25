//-----------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System.Collections.Generic;

    public class UserInfo
    {
        public UserInfo()
        {
            this.CustomData = new Dictionary<string, string>();
        }

        public UserInfo(UserInfo copy)
        {
            this.CustomData = new Dictionary<string, string>();
            this.CopyFrom(copy);
        }

        /// <summary>
        /// Gets or sets the connection id.  This is only used/set by the server.
        /// </summary>
        /// <value>The connection id.</value>
        public long ConnectionId { get; set; }

        public long UserId { get; set; }

        public string UserHexId { get; set; }

        public string DisplayName { get; set; }

        public Dictionary<string, string> CustomData { get; set; }

        public void Deserialize(NetworkReader reader)
        {
            this.ConnectionId = reader.ReadInt64();
            this.UserId = reader.ReadInt64();
            this.UserHexId = reader.ReadString();
            this.DisplayName = reader.ReadString();

            // CustomData
            this.CustomData.Clear();

            byte count = reader.ReadByte();

            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();

                this.CustomData.Add(key, value);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.ConnectionId);
            writer.Write(this.UserId);
            writer.Write(this.UserHexId);
            writer.Write(this.DisplayName);

            // CustomData
            writer.Write((byte)this.CustomData.Count);

            foreach (var pair in this.CustomData)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }

        public void CopyFrom(UserInfo source)
        {
            this.ConnectionId = source.ConnectionId;
            this.UserId = source.UserId;

            // CustomData
            this.CustomData.Clear();

            foreach (var pair in source.CustomData)
            {
                this.CustomData.Add(pair.Key, pair.Value);
            }
        }
    }
}
