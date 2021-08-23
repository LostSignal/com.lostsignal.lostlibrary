//-----------------------------------------------------------------------
// <copyright file="DisableAnimatorOnExit.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public class DisableAnimatorOnExit : StateMachineBehaviour
    {
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            animator.enabled = false;
        }
    }
}

#endif
