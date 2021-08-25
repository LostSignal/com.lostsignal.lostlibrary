//-----------------------------------------------------------------------
// <copyright file="PlayFabSysHttpUnity.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB

namespace PlayFab.Internal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class PlayFabSysHttpUnity : ITransportPlugin
    {
        private static readonly PlayFabSysHttp PlayFabSysHttp = new PlayFabSysHttp();

        Task<object> ITransportPlugin.DoPost(string fullPath, object request, Dictionary<string, string> headers)
        {
            //// NOTE [bgish]: Making sure all PlayFab calls happen in a background thread.  This also ensures that
            ////               calling PlayFab functions inside the Unity Editor will work and not freeze Unity.
            return Task.Run<object>(function: async () =>
            {
                return await PlayFabSysHttp.DoPost(fullPath, request, headers);
            });
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            PluginManager.SetPlugin(new PlayFabSysHttpUnity(), PluginContract.PlayFab_Transport);
        }
    }
}

#endif
