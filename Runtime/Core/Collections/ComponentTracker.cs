//-----------------------------------------------------------------------
// <copyright file="ComponentTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ComponentTracker<T>
        where T : MonoBehaviour
    {
        private T[] components;
        private int count;

        public ComponentTracker(int initialCapacity)
        {
            this.components = new T[initialCapacity];
            this.count = 0;

            Bootloader.OnReset += this.Clear;
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.count;
        }
        
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.components[index];
        }

        public void Clear()
        {
            this.count = 0;

            for (int i = 0; i < this.components.Length; i++)
            {
                this.components[i] = null;
            }
        }

        public void Add(T instance)
        {
            // Making sure it doesn't already exist in the list
            for (int i = 0; i < this.count; i++)
            {
                if (this.components[i] == instance)
                {
                    return;
                }
            }

            // Making sure we have enough room
            if (this.count == this.components.Length)
            {
                Debug.LogWarning($"ComponentTracker<{typeof(T).Name}>'s array grew in size!  Consider increasing it's initial capacity to not do this at runtime.");
                Array.Resize(ref this.components, this.components.Length * 2);
            }
                        
            this.components[this.count] = instance;
            this.count++;
        }

        public void Remove(T instance)
        {
            for (int i = 0; i < this.count; i++)
            {
                if (this.components[i] == instance)
                {
                    if (i == this.count - 1)
                    {
                        this.components[i] = null;
                    }
                    else
                    {
                        T lastInstnce = this.components[this.count - 1];
                        this.components[this.count - 1] = null;
                        this.components[i] = lastInstnce;
                    }

                    this.count--;
                }
            }
        }
    }
}

#endif
