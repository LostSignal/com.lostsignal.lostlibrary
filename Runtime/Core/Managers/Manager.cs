//-----------------------------------------------------------------------
// <copyright file="Manager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public delegate void OnManagerInitializedDelegate();

    public static class ManagerList
    {
        static ManagerList()
        {
            Bootloader.OnReset += Managers.Clear;
        }

        public static List<IManager> Managers = new List<IManager>();
    }

    public abstract class Manager<T> : MonoBehaviour, IManager
        where T : MonoBehaviour
    {
        private bool hasInitializationRun;

        public static bool IsInitialized => Instance != null;

        public static T Instance { get; private set; }

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

        protected virtual void Awake()
        {
            ManagerList.Managers.Add(this);
        }

        protected virtual void Start()
        {
            this.RunInitialization();
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
            Instance = null;
            onInitialized = null;
        }

        protected virtual void OnDestory()
        {
            ManagerList.Managers.Remove(this);
        }

        protected void SetInstance(T instance)
        {
            Debug.Assert(Instance == null, $"Manager {typeof(T).Name}'s Instance is not null!");
            Instance = instance;
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
