//-----------------------------------------------------------------------
// <copyright file="VirtualCurrencyManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.PlayFab
{
    using System;
    using System.Collections.Generic;
    using global::PlayFab.ClientModels;
    using global::PlayFab.Internal;
    using UnityEngine;

    public class VirtualCurrencyManager
    {
        private Dictionary<string, int> virtualCurrencies = new Dictionary<string, int>();
        private Dictionary<string, int> virtualCurrencyRechargeTimes = new Dictionary<string, int>();
        private PlayFabManager playfabManager;

        public VirtualCurrencyManager(PlayFabManager playfabManager, Dictionary<string, int> virtualCurrency)
        {
            this.playfabManager = playfabManager;
            this.playfabManager.GlobalPlayFabResultHandler += this.OnGlobalPlayFabResultHandler;
        }

        public delegate void OnVirtualCurrencyChangedDelegate();

        public event OnVirtualCurrencyChangedDelegate VirtualCurrencyChanged;

        public int this[string virtualCurrencyId]
        {
            get
            {
                int value;
                if (this.virtualCurrencies.TryGetValue(virtualCurrencyId, out value))
                {
                    return value;
                }

                return -1;
            }
        }

        public UnityTask<GetUserInventoryResult> RefreshVirtualCurrency()
        {
            return this.playfabManager.Do(new GetUserInventoryRequest());
        }

        public int GetSecondsToRecharge(string virtualCurrencyId)
        {
            if (this.virtualCurrencyRechargeTimes == null)
            {
                return 0;
            }

            int rechargeFinishedTime = 0;

            if (this.virtualCurrencyRechargeTimes.TryGetValue(virtualCurrencyId, out rechargeFinishedTime))
            {
                return Math.Max(0, rechargeFinishedTime - (int)Time.realtimeSinceStartup);
            }

            return 0;
        }

        public void InternalAddVirtualCurrencyToInventory(string virtualCurrencyId, int amountToAdd)
        {
            if (virtualCurrencyId == "RM" || virtualCurrencyId == "AD")
            {
                return;
            }

            if (this.virtualCurrencies.ContainsKey(virtualCurrencyId) == false)
            {
                Debug.LogErrorFormat("Tried to add unknown virtual currency to inventory {0}", virtualCurrencyId);
                return;
            }

            this.virtualCurrencies[virtualCurrencyId] += amountToAdd;

            this.VirtualCurrencyChanged?.Invoke();
        }

        public void InternalSetVirtualCurrencyToInventory(string virtualCurrencyId, int newValue)
        {
            if (virtualCurrencyId == "RM" || virtualCurrencyId == "AD")
            {
                return;
            }

            if (this.virtualCurrencies.ContainsKey(virtualCurrencyId) == false)
            {
                Debug.LogErrorFormat("Tried to add unknown virtual currency to inventory {0}", virtualCurrencyId);
                return;
            }

            this.virtualCurrencies[virtualCurrencyId] = newValue;

            this.VirtualCurrencyChanged?.Invoke();
        }

        private void UpdateVirtualCurrencies(Dictionary<string, int> virtualCurrencies, Dictionary<string, VirtualCurrencyRechargeTime> rechargeTimes)
        {
            bool currenciesChanged = false;

            if (virtualCurrencies != null)
            {
                currenciesChanged = true;
                this.virtualCurrencies.Clear();

                foreach (var currencyId in virtualCurrencies.Keys)
                {
                    this.virtualCurrencies.Add(currencyId, virtualCurrencies[currencyId]);
                }
            }

            if (rechargeTimes != null)
            {
                currenciesChanged = true;
                this.virtualCurrencyRechargeTimes.Clear();

                foreach (var vc in rechargeTimes.Keys)
                {
                    if (rechargeTimes.ContainsKey(vc))
                    {
                        this.virtualCurrencyRechargeTimes.Add(vc, (int)(Time.realtimeSinceStartup + rechargeTimes[vc].SecondsToRecharge + 1));
                    }
                }
            }

            if (currenciesChanged)
            {
                this.VirtualCurrencyChanged?.Invoke();
            }
        }

        private void OnGlobalPlayFabResultHandler(PlayFabRequestCommon request, PlayFabResultCommon result)
        {
            if (result is LoginResult loginResult)
            {
                var payload = loginResult.InfoResultPayload;
                this.UpdateVirtualCurrencies(payload?.UserVirtualCurrency, payload?.UserVirtualCurrencyRechargeTimes);
            }
            else if (result is GetUserInventoryResult getUserInventoryResult)
            {
                this.UpdateVirtualCurrencies(getUserInventoryResult?.VirtualCurrency, getUserInventoryResult?.VirtualCurrencyRechargeTimes);
            }
            else if (result is GetPlayerCombinedInfoResult getPlayerCombinedInfoResult)
            {
                var payload = getPlayerCombinedInfoResult.InfoResultPayload;
                this.UpdateVirtualCurrencies(payload?.UserVirtualCurrency, payload?.UserVirtualCurrencyRechargeTimes);
            }
        }
    }
}

#endif
