//-----------------------------------------------------------------------
// <copyright file="Manager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public delegate void OnManagerInitializedDelegate();

    public abstract class Manager<T> : MonoBehaviour, IManager where T : MonoBehaviour
    {
        private bool hasInitializationRun;

        private static event OnManagerInitializedDelegate onInitialized;

        public static bool IsInitialized => Instance != null;

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

        public static T Instance { get; private set; }

        public abstract void Initialize();

        protected virtual void Start()
        {
            RunInitialization();
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
            Instance = null;
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

        bool IManager.IsManagerInitialized()
        {
            return IsInitialized || this.enabled == false;
        }
    }
}
