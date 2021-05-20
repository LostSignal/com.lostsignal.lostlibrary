//-----------------------------------------------------------------------
// <copyright file="SetAnimatorBool.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public class SetAnimatorBool : GameObjectStateAction
    {
#pragma warning disable 0649
        [SerializeField] private Animator animator;
        [SerializeField] private string boolName;
        [SerializeField] private bool boolValue;
#pragma warning restore 0649

        private bool previousValue;

        public override void Apply()
        {
            this.previousValue = this.animator.GetBool(this.boolName);
            this.animator.SetBool(this.boolName, this.boolValue);
        }

        public override void Revert()
        {
            this.animator.SetBool(this.boolName, this.previousValue);
        }
    }
}

#endif
