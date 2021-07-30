//-----------------------------------------------------------------------
// <copyright file="FlagManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FlagManager : Manager<FlagManager>
    {
#pragma warning disable 0649
        [SerializeField] private bool printDebugOutput;
#pragma warning restore 0649

        private List<FlagCollection> activeFlagCollections = new List<FlagCollection>();

        public bool PrintDebugOutput => this.printDebugOutput;

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        public void AddFlagCollection(FlagCollection flagCollection)
        {
            this.activeFlagCollections.AddIfUnique(flagCollection);
        }

        public void RemoveFlagCollection(FlagCollection flagCollection)
        {
            this.activeFlagCollections.Remove(flagCollection);
        }
    }
}

#endif
