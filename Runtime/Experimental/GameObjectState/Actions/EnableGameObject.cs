//-----------------------------------------------------------------------
// <copyright file="EnableGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public class EnableGameObject : GameObjectStateAction
    {
#pragma warning disable 0649
        [SerializeField] private GameObject gameObjectToEnable;
#pragma warning restore 0649

        private bool previousValue;

        public override void Apply()
        {
            this.previousValue = this.gameObjectToEnable.activeSelf;
            this.gameObjectToEnable.SetActive(true);
        }

        public override void Revert()
        {
            this.gameObjectToEnable.SetActive(this.previousValue);
        }
    }
}

#endif
