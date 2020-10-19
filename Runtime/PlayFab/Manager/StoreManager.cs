//-----------------------------------------------------------------------
// <copyright file="StoreManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using System.Collections.Generic;
    using global::PlayFab;
    using global::PlayFab.ClientModels;

    public class StoreManager
    {
        private GetStoreItemsRequest getStoreRequest = new GetStoreItemsRequest();
        private Dictionary<string, List<StoreItem>> cachedStores = new Dictionary<string, List<StoreItem>>();
        private PlayFabManager playfabManager;

        public StoreManager(PlayFabManager playfabManager, string catalogVersion)
        {
            this.playfabManager = playfabManager;
            this.getStoreRequest.CatalogVersion = catalogVersion;
        }

        public UnityTask<List<StoreItem>> GetStore(string storeId, bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                this.cachedStores.Remove(storeId);
            }

            if (this.cachedStores.ContainsKey(storeId))
            {
                return UnityTask<List<StoreItem>>.Empty(this.cachedStores[storeId]);
            }
            else
            {
                return UnityTask<List<StoreItem>>.Run(FetchStore());
            }

            IEnumerator<List<StoreItem>> FetchStore()
            {
                this.getStoreRequest.StoreId = storeId;

                var getStore = this.playfabManager.Do<GetStoreItemsRequest, GetStoreItemsResult>(getStoreRequest, PlayFabClientAPI.GetStoreItemsAsync);

                while (getStore.IsDone == false)
                {
                    yield return null;
                }

                var store = getStore.Value?.Store;

                // Caching off the store in case the user requests it again
                if (store != null)
                {
                    this.cachedStores.Add(storeId, store);
                }

                yield return store;
            }
        }

        public void InvalidateStoreCache()
        {
            this.cachedStores.Clear();
        }
    }
}
