//-----------------------------------------------------------------------
// <copyright file="TitleDataManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.PlayFab
{
    using System.Collections.Generic;

    public class TitleDataManager
    {
        private Dictionary<string, string> titleDataCache = new Dictionary<string, string>();
        private Dictionary<string, object> titleDataObjectCache = new Dictionary<string, object>();
        private PlayFabManager playfabManager;

        public TitleDataManager(PlayFabManager playfabManager, Dictionary<string, string> titleData)
        {
            this.playfabManager = playfabManager;

            if (titleData?.Count > 0)
            {
                foreach (var key in titleData.Keys)
                {
                    this.titleDataCache.Add(key, titleData[key]);
                }
            }
        }

        public UnityTask<string> GetTitleData(string titleDataKey)
        {
            if (this.titleDataCache.TryGetValue(titleDataKey, out string titleDataValue))
            {
                return UnityTask<string>.Empty(titleDataValue);
            }
            else
            {
                return UnityTask<string>.Run(FetchTitleData());
            }

            IEnumerator<string> FetchTitleData()
            {
                if (UnityEngine.Time.realtimeSinceStartup < 10.0)
                {
                    UnityEngine.Debug.LogWarning($"Retrieving Title Data Key {titleDataKey} early in app startup.  Prehaps add this to PlayFabManager to download at startup.");
                }

                yield return null;
            }
        }

        public UnityTask<T> GetTitleData<T>(string titleDataKey) where T : class
        {
            if (this.titleDataObjectCache.TryGetValue(titleDataKey, out object obj))
            {
                return UnityTask<T>.Empty(obj as T);
            }
            else
            {
                return UnityTask<T>.Run(FetchTitleDataObject());
            }

            IEnumerator<T> FetchTitleDataObject()
            {
                // TODO [bgish]: Get the string, parse as T, cache and return
                yield return null;
            }
        }
    }
}

#endif
