//-----------------------------------------------------------------------
// <copyright file="CatalogManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.PlayFab
{
    using System.Collections.Generic;
    using global::PlayFab;
    using global::PlayFab.ClientModels;

    public class CatalogManager
    {
        private GetCatalogItemsRequest getCatalogRequest = new GetCatalogItemsRequest();
        private Dictionary<string, CatalogItem> catalogItemDictionary = new Dictionary<string, CatalogItem>();
        private Dictionary<string, Dictionary<string, string>> customDataCache = new Dictionary<string, Dictionary<string, string>>();
        private List<CatalogItem> cachedCatalog;
        private PlayFabManager playfabManager;

        private bool IsCatalogCached => this.cachedCatalog != null;

        public CatalogManager(PlayFabManager playfabManager, string catalogVersion)
        {
            this.playfabManager = playfabManager;
            this.getCatalogRequest.CatalogVersion = catalogVersion;
        }

        public UnityTask<List<CatalogItem>> GetCatalog()
        {
            if (this.cachedCatalog != null)
            {
                return UnityTask<List<CatalogItem>>.Empty(this.cachedCatalog);
            }
            else
            {
                return UnityTask<List<CatalogItem>>.Run(FetchCatalog());
            }

            IEnumerator<List<CatalogItem>> FetchCatalog()
            {
                var getCatalog = this.playfabManager.Do<GetCatalogItemsRequest, GetCatalogItemsResult>(this.getCatalogRequest, PlayFabClientAPI.GetCatalogItemsAsync);

                while (getCatalog.IsDone == false)
                {
                    yield return null;
                }

                var catalogItems = getCatalog.Value?.Catalog;

                if (catalogItems != null)
                {
                    this.cachedCatalog = catalogItems;
                    this.catalogItemDictionary.Clear();

                    foreach (var item in this.cachedCatalog)
                    {
                        this.catalogItemDictionary.Add(item.ItemId, item);
                    }
                }

                yield return catalogItems;
            }
        }

        public UnityTask<CatalogItem> GetCatalogItem(string itemId)
        {
            if (this.IsCatalogCached)
            {
                return UnityTask<CatalogItem>.Empty(this.GetCachedCatalogItem(itemId));
            }
            else
            {
                return UnityTask<CatalogItem>.Run(Coroutine());
            }

            IEnumerator<CatalogItem> Coroutine()
            {
                var getCatalog = this.GetCatalog();

                while (getCatalog.IsDone == false)
                {
                    yield return null;
                }

                yield return this.GetCachedCatalogItem(itemId);
            }
        }

        public Dictionary<string, string> GetCustomData(CatalogItem catalogItem)
        {
            if (this.customDataCache.TryGetValue(catalogItem.ItemId, out Dictionary<string, string> customData))
            {
                return customData;
            }
            else
            {
                Dictionary<string, string> newCustomData = null;

                if (string.IsNullOrWhiteSpace(catalogItem.CustomData))
                {
                    newCustomData = new Dictionary<string, string>();
                }
                else
                {
                    try
                    {
                        newCustomData = JsonUtil.Deserialize<Dictionary<string, string>>(catalogItem.CustomData);
                    }
                    catch
                    {
                        UnityEngine.Debug.LogError($"Unable to parse CustomData for ItemId {catalogItem.ItemId}.");
                    }
                }

                this.customDataCache.Add(catalogItem.ItemId, newCustomData);
                return newCustomData;
            }
        }

        private CatalogItem GetCachedCatalogItem(string itemId)
        {
            if (this.catalogItemDictionary.TryGetValue(itemId, out CatalogItem item))
            {
                return item;
            }

            return null;
        }
    }
}

#endif
