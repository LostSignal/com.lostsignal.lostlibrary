//-----------------------------------------------------------------------
// <copyright file="DisableGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class DisableGameObject : GameObjectStateAction
    {
#pragma warning disable 0649
        [SerializeField] private GameObject gameObjectToDisable;
#pragma warning restore 0649

        private bool previousValue;

        public override void Apply()
        {
            this.previousValue = this.gameObjectToDisable.activeSelf;
            this.gameObjectToDisable.SetActive(false);
        }

        public override void Revert()
        {
            this.gameObjectToDisable.SetActive(this.previousValue);
        }
    }
}
