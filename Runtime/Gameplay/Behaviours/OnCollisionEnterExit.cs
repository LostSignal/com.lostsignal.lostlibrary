//-----------------------------------------------------------------------
// <copyright file="OnCollisionEnterExit.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("Lost/OnCollisionEnterExit")]
    public class OnCollisionEnterExit : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private UnityEvent onCollisionEnter;
        [SerializeField] private UnityEvent onCollisionExit;
        [SerializeField] private LayerMask layerFilter = 0;
        [SerializeField] private string tagFilter;
#pragma warning restore 0649

        private void OnCollisionEnter(Collision collision)
        {
            if (this.onCollisionEnter != null && this.DidPassFilters(collision))
            {
                this.onCollisionEnter.Invoke();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (this.onCollisionExit != null && this.DidPassFilters(collision))
            {
                this.onCollisionExit.Invoke();
            }
        }

        private bool DidPassFilters(Collision collision)
        {
            if (string.IsNullOrEmpty(this.tagFilter) != false)
            {
                if (collision.collider.gameObject.CompareTag(this.tagFilter) == false)
                {
                    return false;
                }
            }

            if (this.layerFilter != 0)
            {
                if ((collision.gameObject.layer & this.layerFilter) == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

#endif
