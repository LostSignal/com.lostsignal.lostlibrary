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
    public class FlagCollection : ScriptableObject
    {
        public const int NullId = -1;
        private const int NumberOfBits = sizeof(byte) * 8;

        public enum Location
        {
            PlayerData,
            GameData,
        }

        #pragma warning disable 0649
        [SerializeField] private Location location;
        [SerializeField] private List<Flag> flags;
        #pragma warning restore 0649

        public Action FlagsChanged;

        private List<byte> flagBits;
        private bool isDirty;

        public bool IsFlagSet(int id)
        {
            int index = id / NumberOfBits;

            if (index >= this.flagBits.Count)
            {
                return false;
            }
            else
            {
                return (this.flagBits[index] & (1 << (id % NumberOfBits))) != 0;
            }
        }

        public void SetFlag(int id)
        {
            if (id == NullId)
            {
                throw new ArgumentException("Can not add Null Id to IdBag");
            }
            else if (id < 0)
            {
                throw new ArgumentException("IdBag can not store negative ids");
            }

            // Don't add it if it already exists
            if (this.IsFlagSet(id))
            {
                return;
            }

            this.isDirty = true;

            int index = id / NumberOfBits;
            int listSizeNeeded = index + 1;

            // Making sure there are enough space in the bits list for the new id
            while (this.flagBits.Count < listSizeNeeded)
            {
                this.flagBits.Add(0);
            }

            this.flagBits[index] = (byte)(this.flagBits[index] | 1 << (id % NumberOfBits));
        }

        [Serializable]
        public class Flag
        {
            #pragma warning disable 0649
            [SerializeField] private string name;
            [SerializeField] private int id;
            #pragma warning restore 0649

            public int Id => this.id;

            public string Name => this.name;
        }
    }
}
