//-----------------------------------------------------------------------
// <copyright file="Manager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public delegate void OnManagerInitializedDelegate();

    public abstract class Manager<T> : MonoBehaviour, IManager
        where T : MonoBehaviour
    {
        private static T instance;

        private bool hasInitializationRun;
        
        public static bool IsInitialized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => instance != null;
        }

        public static T Instance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => instance;
        }

        public static event OnManagerInitializedDelegate OnInitialized
        {
            add
            {
                if (Instance != null)
                {
                    value?.Invoke();
                }

                onInitialized += value;
            }

            remove
            {
                onInitialized -= value;
            }
        }

        private static event OnManagerInitializedDelegate onInitialized;

        public static IEnumerator WaitForInitialization()
        {
            while (IsInitialized == false)
            {
                yield return null;
            }
        }

        public abstract void Initialize();

        bool IManager.IsManagerInitialized()
        {
            return IsInitialized || this.enabled == false;
        }

        protected virtual void Start()
        {
            this.RunInitialization();
        }

        protected virtual void OnEnable()
        {
            ManagerTracker.AddManager(this);
        }

        protected virtual void OnDisable()
        {
            ManagerTracker.RemoveManager(this);

            instance = null;
            onInitialized = null;
        }

        protected void SetInstance(T newInstance)
        {
            Debug.Assert(Instance == null, $"Manager {typeof(T).Name}'s Instance is not null!");
            instance = newInstance;
            onInitialized?.Invoke();
        }

        protected Coroutine WaitForDependencies(params IManager[] managers)
        {
            return this.StartCoroutine(WaitForDepenciesCoroutine());

            IEnumerator WaitForDepenciesCoroutine()
            {
                while (true)
                {
                    bool allDone = true;

                    if (managers?.Length > 0)
                    {
                        for (int i = 0; i < managers.Length; i++)
                        {
                            if (managers[i].IsManagerInitialized() == false)
                            {
                                allDone = false;
                                break;
                            }
                        }
                    }

                    if (allDone)
                    {
                        yield break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }

        private void RunInitialization()
        {
            if (this.hasInitializationRun)
            {
                return;
            }

            this.hasInitializationRun = true;

            this.Initialize();
        }
    }
}

#endif
