//-----------------------------------------------------------------------
// <copyright file="FlagCollection.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "FlagCollection", menuName = "Lost/Flag Collection")]
    public class FlagCollection : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly ObjectTracker<FlagCollection> ActiveCollections = new ObjectTracker<FlagCollection>(20);

        public enum Location
        {
            PlayerData,
            GameData,
        }

        #pragma warning disable 0649
        [SerializeField] private Location location;
        [SerializeField] private List<Flag> flags;

        [HideInInspector]
        [SerializeField] private BitArray flagBits;
        #pragma warning restore 0649

        private string dataStoreKeyName;
        private bool isInitialized;

        public Action FlagsChanged;

        private string DataStoreKeyName
        {
            get
            {
                if (this.dataStoreKeyName == null)
                {
                    this.dataStoreKeyName = $"FC_" + this.name;
                }

                return this.dataStoreKeyName;
            }
        }

        private void OnEnable()
        {
            ActiveCollections.Add(this);
            Initialize();
        }

        private void OnDisable()
        {
            ActiveCollections.Remove(this);
        }

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            IDataManager dataManager = null;

            if (location == Location.PlayerData && PlayerDataManager.IsInitialized)
            {
                dataManager = PlayerDataManager.Instance;
            }
            else if (location == Location.GameData && GameDataManager.IsInitialized)
            {
                dataManager = GameDataManager.Instance;
            }

            if (dataManager != null && dataManager.DataStore != null)
            {
                this.isInitialized = true;
                var flagBits = dataManager.DataStore.GetByteArray(this.DataStoreKeyName, null);
                this.flagBits.SetBits(flagBits);
            }
        }

        public void SetFlag(int flagId)
        {
            this.AssertInitialized(flagId);
            this.flagBits.SetBit(flagId);
        }

        public bool IsFlagSet(int flagId)
        {
            this.AssertInitialized(flagId);
            return this.flagBits.IsBitSet(flagId);
        }

        private void AssertInitialized(int flagId)
        {
            if (this.isInitialized == false)
            {
                this.Initialize();

                if (this.isInitialized == false)
                {
                    Debug.LogError($"FlagCollection {this.name} tried to set flag {flagId} before {this.location} was available.");
                    return;
                }
            }
        }

        private int GetMaxFlagId()
        {
            int maxId = 0;

            if (this.flagBits != null)
            {
                for (int i = 0; i < this.flags.Count; i++)
                {
                    maxId = this.flags[i].Id > maxId ? this.flags[i].Id : maxId;
                }
            }

            return maxId;
        }

        private void OnValidate()
        {
            if (Application.isEditor)
            {
                if (this.flags == null)
                {
                    this.flags = new List<Flag>();
                }

                if (this.flagBits == null)
                {
                    this.flagBits = new BitArray();
                }

                this.flagBits.SetCapacity(this.GetMaxFlagId());

                if (this.flagBits.IsEmpty() == false)
                {
                    this.flagBits.Clear();
                }

                HashSet<int> ids = new HashSet<int>();
                HashSet<string> names = new HashSet<string>();

                foreach (var flag in this.flags)
                {
                    if (flag.Id < 0)
                    {
                        Debug.LogError($"FlagCollection {this.name} has a flag {flag.Name} with a negative id, this will break the flag system.", this);
                    }
                    else if (flag.Id > BitArray.MaxBitIndex)
                    {
                        Debug.LogError($"FlagCollection {this.name} has a flag {flag.Name} with an id greater than BitArray.MaxBitIndex {BitArray.MaxBitIndex}, this will break the flag system.", this);
                    }
                    else if (ids.Contains(flag.Id))
                    {
                        Debug.LogError($"FlagCollection {this.name} has a flag {flag.Name} with a duplicate id {flag.Id}, this will break the flag system.", this);
                    }
                    else if (names.Contains(flag.Name))
                    {
                        Debug.LogError($"FlagCollection {this.name} has a flag {flag.Name} with a duplicate name, this will break the flag system.", this);
                    }
                    
                    ids.AddIfUnique(flag.Id);
                    names.AddIfUnique(flag.Name);
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.OnValidate();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        [Serializable]
        public class Flag
        {
            #pragma warning disable 0649
            [SerializeField] private string name;
            [SerializeField] private int id;
            [SerializeField] private bool isDisabled;
            #pragma warning restore 0649

            public int Id => this.id;

            public string Name => this.name;

            public bool IsDisabled => this.isDisabled;
        }
    }
}
