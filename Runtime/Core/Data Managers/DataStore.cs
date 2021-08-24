//-----------------------------------------------------------------------
// <copyright file="Data.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public class DataStore
    {
        private static readonly Networking.NetworkWriter Writer = new Networking.NetworkWriter((byte[])null);
        private static readonly Networking.NetworkReader Reader = new Networking.NetworkReader((byte[])null);
        private static readonly uint CurrentVersion = 1;

        private Dictionary<string, int> intData;
        private Dictionary<string, int> enumData;
        private Dictionary<string, bool> boolData;
        private Dictionary<string, long> longData;
        private Dictionary<string, string> stringData;
        private Dictionary<string, byte[]> byteArrayData;
        private Dictionary<string, DateTime> dateTimeData;

        public bool IsDirty { get; private set; }

        public int Serialize(byte[] saveDataBuffer)
        {
            this.IsDirty = false;

            Writer.ResetBuffer(saveDataBuffer);
            Writer.WritePackedUInt32(CurrentVersion);
            Writer.WritePackedUInt32(this.intData == null ? 0 : (uint)this.intData.Count);
            Writer.WritePackedUInt32(this.enumData == null ? 0 : (uint)this.enumData.Count);
            Writer.WritePackedUInt32(this.boolData == null ? 0 : (uint)this.boolData.Count);
            Writer.WritePackedUInt32(this.longData == null ? 0 : (uint)this.longData.Count);
            Writer.WritePackedUInt32(this.stringData == null ? 0 : (uint)this.stringData.Count);
            Writer.WritePackedUInt32(this.byteArrayData == null ? 0 : (uint)this.byteArrayData.Count);
            Writer.WritePackedUInt32(this.dateTimeData == null ? 0 : (uint)this.dateTimeData.Count);

            if (this.intData?.Count > 0)
            {
                foreach (var keyValuePair in this.intData)
                {
                    Writer.Write(keyValuePair.Key);
                    Writer.Write(keyValuePair.Value);
                }
            }

            if (this.enumData?.Count > 0)
            {
                foreach (var keyValuePair in this.enumData)
                {
                    Writer.Write(keyValuePair.Key);
                    Writer.Write(keyValuePair.Value);
                }
            }

            if (this.boolData?.Count > 0)
            {
                foreach (var keyValuePair in this.boolData)
                {
                    Writer.Write(keyValuePair.Key);
                    Writer.Write(keyValuePair.Value);
                }
            }

            if (this.longData?.Count > 0)
            {
                foreach (var keyValuePair in this.longData)
                {
                    Writer.Write(keyValuePair.Key);
                    Writer.Write(keyValuePair.Value);
                }
            }

            if (this.stringData?.Count > 0)
            {
                foreach (var keyValuePair in this.stringData)
                {
                    Writer.Write(keyValuePair.Key);
                    Writer.Write(keyValuePair.Value);
                }
            }

            if (this.byteArrayData?.Count > 0)
            {
                foreach (var keyValuePair in this.byteArrayData)
                {
                    Writer.Write(keyValuePair.Key);
                    Writer.WriteBytesFull(keyValuePair.Value);
                }
            }

            if (this.dateTimeData?.Count > 0)
            {
                foreach (var keyValuePair in this.dateTimeData)
                {
                    Writer.Write(keyValuePair.Key);
                    Writer.Write(keyValuePair.Value.ToFileTimeUtc());
                }
            }

            int byteCount = Writer.Position;
            Writer.ResetBuffer(null);
            return byteCount;
        }

        public void Deserialize(byte[] serializedData)
        {
            this.IsDirty = false;

            // Making sure everything is initialized
            this.intData ??= new Dictionary<string, int>();
            this.enumData ??= new Dictionary<string, int>();
            this.boolData ??= new Dictionary<string, bool>();
            this.longData ??= new Dictionary<string, long>();
            this.stringData ??= new Dictionary<string, string>();
            this.byteArrayData ??= new Dictionary<string, byte[]>();
            this.dateTimeData ??= new Dictionary<string, DateTime>();

            // Make sure everything is cleared
            this.intData.Clear();
            this.enumData.Clear();
            this.boolData.Clear();
            this.longData.Clear();
            this.stringData.Clear();
            this.byteArrayData.Clear();
            this.dateTimeData.Clear();

            // Making our network reader point to the given byte data
            Reader.ResetBuffer(serializedData);

            uint version = Reader.ReadPackedUInt32();

            if (version == 1)
            {
                this.DeserializeVersion1();
            }
            else
            {
                UnityEngine.Debug.LogError($"Serialization Failed! {nameof(DataStore)} found unknown version number \"{version}\"");
            }

            Reader.ResetBuffer(null);
        }

        // Delete
        public void DeleteInt(string key) => this.DeleteKey(this.intData, key);

        public void DeleteEnum(string key) => this.DeleteKey(this.enumData, key);

        public void DeleteBool(string key) => this.DeleteKey(this.boolData, key);

        public void DeleteLong(string key) => this.DeleteKey(this.longData, key);

        public void DeleteString(string key) => this.DeleteKey(this.stringData, key);

        public void DeleteByteArray(string key) => this.DeleteKey(this.byteArrayData, key);

        public void DeleteDateTime(string key) => this.DeleteKey(this.dateTimeData, key);

        // Gets
        public int GetInt(string key, int defaultValue = 0) => this.GetKey(this.intData, key, defaultValue);

        public T GetEnumt<T>(string key, T defaultValue = default(T)) => (T)Enum.ToObject(typeof(T), this.GetKey(this.enumData, key, Convert.ToInt32(defaultValue)));

        public bool GetBool(string key, bool defaultValue = false) => this.GetKey(this.boolData, key, defaultValue);

        public long GetLong(string key, long defaultValue = 0) => this.GetKey(this.longData, key, defaultValue);

        public string GetString(string key, string defaultValue = null) => this.GetKey(this.stringData, key, defaultValue);

        public byte[] GetByteArray(string key, byte[] defaultValue = null) => this.GetKey(this.byteArrayData, key, defaultValue);

        public DateTime GetDateTime(string key, DateTime defaultValue) => this.GetKey(this.dateTimeData, key, defaultValue);

        // Has
        public bool HasInt(string key) => this.HasKey(this.intData, key);

        public bool HasEnum(string key) => this.HasKey(this.enumData, key);

        public bool HasBool(string key) => this.HasKey(this.boolData, key);

        public bool HasLong(string key) => this.HasKey(this.longData, key);

        public bool HasString(string key) => this.HasKey(this.stringData, key);

        public bool HasByteArray(string key) => this.HasKey(this.byteArrayData, key);

        public bool HasDateTime(string key) => this.HasKey(this.dateTimeData, key);

        public void SetInt(string key, int value) => this.SetValueTypeKey(ref this.intData, key, value);

        public void SetEnum<T>(string key, T value) => this.SetValueTypeKey(ref this.enumData, key, Convert.ToInt32(value));

        public void SetBool(string key, bool value) => this.SetValueTypeKey(ref this.boolData, key, value);

        public void SetLong(string key, long value) => this.SetValueTypeKey(ref this.longData, key, value);

        public void SetString(string key, string value) => this.SetClassTypeKey(ref this.stringData, key, value);

        public void SetByteArray(string key, byte[] value) => this.SetClassTypeKey(ref this.byteArrayData, key, value);

        public void SetDateTime(string key, DateTime value) => this.SetValueTypeKey(ref this.dateTimeData, key, value);

        private void DeleteKey<T>(Dictionary<string, T> dictionary, string key)
        {
            if (dictionary?.Remove(key) == true)
            {
                this.IsDirty = true;
            }
        }

        private T GetKey<T>(Dictionary<string, T> dictionary, string key, T defaultValue)
        {
            if (dictionary == null)
            {
                return defaultValue;
            }

            return dictionary.TryGetValue(key, out T value) ? value : defaultValue;
        }

        private bool HasKey<T>(Dictionary<string, T> dictionary, string key)
        {
            return dictionary?.ContainsKey(key) ?? false;
        }

        private void SetValueTypeKey<T>(ref Dictionary<string, T> dictionary, string key, T value)
            where T : struct
        {
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, T>();
                this.IsDirty = true;
            }

            if (dictionary.TryGetValue(key, out T currentDictionaryValue))
            {
                if (value.Equals(currentDictionaryValue))
                {
                    // Do nothing
                }
                else
                {
                    dictionary[key] = value;
                    this.IsDirty = true;
                }
            }
            else
            {
                dictionary.Add(key, value);
                this.IsDirty = true;
            }
        }

        private void SetClassTypeKey<T>(ref Dictionary<string, T> dictionary, string key, T value)
            where T : class
        {
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, T>();
                this.IsDirty = true;
            }

            if (dictionary.TryGetValue(key, out T currentDictionaryValue))
            {
                if (value == null && currentDictionaryValue == null)
                {
                    // Do Nothing
                }
                else if (value?.Equals(currentDictionaryValue) == true)
                {
                    // Do Nothing
                }
                else
                {
                    dictionary[key] = value;
                    this.IsDirty = true;
                }
            }
            else
            {
                dictionary.Add(key, value);
                this.IsDirty = true;
            }
        }

        private void DeserializeVersion1()
        {
            uint intCount = Reader.ReadPackedUInt32();
            uint enumCount = Reader.ReadPackedUInt32();
            uint boolCount = Reader.ReadPackedUInt32();
            uint longCount = Reader.ReadPackedUInt32();
            uint stringCount = Reader.ReadPackedUInt32();
            uint byteArrayCount = Reader.ReadPackedUInt32();
            uint dateTimeCount = Reader.ReadPackedUInt32();

            for (int i = 0; i < intCount; i++)
            {
                this.intData.Add(Reader.ReadString(), Reader.ReadInt32());
            }

            for (int i = 0; i < enumCount; i++)
            {
                this.enumData.Add(Reader.ReadString(), Reader.ReadInt32());
            }

            for (int i = 0; i < boolCount; i++)
            {
                this.boolData.Add(Reader.ReadString(), Reader.ReadBoolean());
            }

            for (int i = 0; i < longCount; i++)
            {
                this.longData.Add(Reader.ReadString(), Reader.ReadInt64());
            }

            for (int i = 0; i < stringCount; i++)
            {
                this.stringData.Add(Reader.ReadString(), Reader.ReadString());
            }

            for (int i = 0; i < byteArrayCount; i++)
            {
                this.byteArrayData.Add(Reader.ReadString(), Reader.ReadBytesAndSize());
            }

            for (int i = 0; i < dateTimeCount; i++)
            {
                this.dateTimeData.Add(Reader.ReadString(), DateTime.FromFileTimeUtc(Reader.ReadInt64()));
            }
        }
    }
}
