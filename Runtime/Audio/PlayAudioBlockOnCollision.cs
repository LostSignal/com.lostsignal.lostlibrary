//-----------------------------------------------------------------------
// <copyright file="PlayAudioBlockOnCollision.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public class PlayAudioBlockOnCollision : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private AudioBlock audioBlock;
        [SerializeField] private float delayToPlayOnAwake = 0.5f;
        [SerializeField] private float playAudioBlockCooldown = 0.2f;
        [SerializeField] private LayerMask layerFilter = 0;
#pragma warning restore 0649

        private float minimumNextPlayTime;

        private void Awake()
        {
            this.minimumNextPlayTime = Time.time + this.delayToPlayOnAwake;

            if (this.audioBlock == null)
            {
                Debug.LogError($"GameObject {this.gameObject.name} does not have an Audio Block!", this);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            // Making sure we have an audio block
            if (this.audioBlock == null)
            {
                return;
            }

            // Making sure it matches the layer filter
            if (this.layerFilter != 0)
            {
                if ((other.gameObject.layer & this.layerFilter) == 0)
                {
                    return;
                }
            }

            // Making sure we haven't played too recently
            if (Time.time < this.minimumNextPlayTime)
            {
                return;
            }

            this.minimumNextPlayTime = Time.time + this.playAudioBlockCooldown;

            int contactsCount = other.GetContacts(Caching.ContactPointsCache);

            if (contactsCount > 0)
            {
                this.audioBlock.Play(Caching.ContactPointsCache[0].point);
            }
        }
    }
}

#endif
