//-----------------------------------------------------------------------
// <copyright file="ObjectTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ObjectTracker<T>
        where T : class
    {
        private T[] objs;
        private int count;

        public ObjectTracker(int initialCapacity)
        {
            this.objs = new T[initialCapacity];
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
            get => this.objs[index];
        }

        public void GetObjects(List<T> objects)
        {
            for (int i = 0; i < this.count; i++)
            {
                objects.Add(this.objs[i]);
            }
        }

        public void Clear()
        {
            this.count = 0;

            for (int i = 0; i < this.objs.Length; i++)
            {
                this.objs[i] = null;
            }
        }

        public void Add(T instance)
        {
            // Making sure it doesn't already exist in the list
            for (int i = 0; i < this.count; i++)
            {
                if (this.objs[i] == instance)
                {
                    return;
                }
            }

            // Making sure we have enough room
            if (this.count == this.objs.Length)
            {
                Debug.LogWarning($"ComponentTracker<{typeof(T).Name}>'s array grew in size!  Consider increasing it's initial capacity to not do this at runtime.");
                Array.Resize(ref this.objs, this.objs.Length * 2);
            }

            this.objs[this.count] = instance;
            this.count++;
        }

        public void Remove(T instance)
        {
            for (int i = 0; i < this.count; i++)
            {
                if (this.objs[i] == instance)
                {
                    if (i == this.count - 1)
                    {
                        this.objs[i] = null;
                    }
                    else
                    {
                        T lastInstnce = this.objs[this.count - 1];
                        this.objs[this.count - 1] = null;
                        this.objs[i] = lastInstnce;
                    }

                    this.count--;
                }
            }
        }
    }
}

#endif
