//-----------------------------------------------------------------------
// <copyright file="ConcurrentList.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ConcurrentList<T>
    {
        private readonly object itemsLock = new object();

        [SerializeField]
        private List<T> items = new List<T>();

        public ConcurrentList()
        {
            this.items = new List<T>();
        }

        public ConcurrentList(int capacity)
        {
            this.items = new List<T>(capacity);
        }

        public void Add(T item)
        {
            lock (this.itemsLock)
            {
                if (this.items.Capacity == this.items.Count)
                {
                    Debug.LogWarning("ConcurrentList Had to grow at runtime, consider increasing it's default capacity.");
                }

                this.items.Add(item);
            }
        }

        public void GetItems(List<T> list)
        {
            lock (this.itemsLock)
            {
                list.Clear();
                list.AddRange(this.items);
            }
        }

        public void RemoveItems(List<T> itemsToRemove)
        {
            if (itemsToRemove == null || itemsToRemove.Count == 0)
            {
                return;
            }

            lock (this.itemsLock)
            {
                for (int i = 0; i < itemsToRemove.Count; i++)
                {
                    this.items.Remove(itemsToRemove[i]);
                }
            }
        }
    }
}
