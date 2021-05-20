//-----------------------------------------------------------------------
// <copyright file="SetAnimatorTrigger.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class SetAnimatorTrigger : GameObjectStateAction
    {
#pragma warning disable 0649
        [SerializeField] private Animator animator;
        [SerializeField] private string triggerName;
#pragma warning restore 0649

        public override void Apply()
        {
            this.animator.SetTrigger(this.triggerName);
        }

        public override void Revert()
        {
        }
    }
}
