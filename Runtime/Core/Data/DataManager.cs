//-----------------------------------------------------------------------
// <copyright file="DataManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class DataManager<T> : Manager<T>, IDataManager
         where T : MonoBehaviour
    {
        private DataStore dataStore;

        public abstract string Name { get; }

        public Action DataUpdated { get; }

        public DataStore DataStore
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.dataStore;

            protected set
            {
                if (this.dataStore != value)
                {
                    this.dataStore = value;
                    this.DataUpdated?.Invoke();
                }
            }
        }

        public bool IsDirty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.dataStore.IsDirty;
        }

        public void Save()
        {
            SaveToPlayerPrefs();
        }

        protected void SaveToPlayerPrefs()
        {
            int length = this.dataStore.Serialize(Caching.ByteBuffer);
            string base64String = Convert.ToBase64String(Caching.ByteBuffer, 0, length);
            PlayerPrefs.SetString(this.Name, base64String);
            PlayerPrefs.Save();
        }

        protected void InitializeDataStroreFromPlayerPrefs()
        {
            var base64String = PlayerPrefs.GetString(this.Name, null);
            var newDataStore = new DataStore();

            if (string.IsNullOrEmpty(base64String) == false)
            {
                var bytes = Convert.FromBase64String(base64String);
                newDataStore.Deserialize(bytes);
            }

            this.DataStore = newDataStore;
        }
    }
}
