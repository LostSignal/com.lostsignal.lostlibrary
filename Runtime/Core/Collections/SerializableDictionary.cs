//-----------------------------------------------------------------------
// <copyright file="SerializableDictionary.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private List<TKey> keys;
        [SerializeField] [HideInInspector] private List<TValue> values;

        public void OnBeforeSerialize()
        {
            this.keys = new List<TKey>(this.Count);
            this.values = new List<TValue>(this.Count);

            foreach (var keyValuePair in this)
            {
                this.keys.Add(keyValuePair.Key);
                this.values.Add(keyValuePair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            int count = Mathf.Min(this.keys.Count, this.values.Count);

            for (int i = 0; i < count; i++)
            {
                this.Add(this.keys[i], this.values[i]);
            }
        }
    }
}

#endif
