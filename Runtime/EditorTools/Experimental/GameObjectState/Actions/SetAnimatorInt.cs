//-----------------------------------------------------------------------
// <copyright file="SetAnimatorInt.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class SetAnimatorInt : GameObjectStateAction
    {
#pragma warning disable 0649
        [SerializeField] private Animator animator;
        [SerializeField] private string intName;
        [SerializeField] private int intValue;
#pragma warning restore 0649

        private int previousValue;

        public override void Apply()
        {
            this.previousValue = this.animator.GetInteger(this.intName);
            this.animator.SetInteger(this.intName, this.intValue);
        }

        public override void Revert()
        {
            this.animator.SetInteger(this.intName, this.previousValue);
        }
    }
}
