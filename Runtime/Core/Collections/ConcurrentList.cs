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

        public void Add(T item)
        {
            lock (this.itemsLock)
            {
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
