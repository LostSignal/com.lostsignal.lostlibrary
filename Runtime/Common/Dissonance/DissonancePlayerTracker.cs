//-----------------------------------------------------------------------
// <copyright file="DissonancePlayerTracker.cs" company="Giant Cranium">
//     Copyright (c) Giant Cranium. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2019_4_OR_NEWER

namespace Lost.DissonanceIntegration
{
    using Lost.Networking;
    using UnityEngine;

    [RequireComponent(typeof(NetworkIdentity))]
    public class DissonancePlayerTracker : MonoBehaviour
#if USING_DISSONANCE
        , Dissonance.IDissonancePlayer
#endif
    {
#pragma warning disable 0649
        [SerializeField] [HideInInspector] private NetworkIdentity networkIdentity;
#pragma warning disable 0649

        public bool IsTracking { get; private set; }

        public string PlayerId { get; private set; }

        public Vector3 Position
        {
            get => this.transform.position;
        }

        public Quaternion Rotation
        {
            get => this.transform.rotation;
        }

#if USING_DISSONANCE

        public Dissonance.NetworkPlayerType Type
        {
            get => this.networkIdentity.IsOwner ? Dissonance.NetworkPlayerType.Local : Dissonance.NetworkPlayerType.Remote;
        }

#endif

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.networkIdentity);
        }

        private void Awake()
        {
            this.OnValidate();
        }

#if USING_DISSONANCE

        private void OnEnable()
        {
            this.StartCoroutine(Coroutine());

            System.Collections.IEnumerator Coroutine()
            {
                if (this.IsTracking)
                {
                    yield break;
                }

                while (this.PlayerId == null && NetworkingManager.Instance && DissonanceManager.Instance)
                {
                    var userInfo = NetworkingManager.Instance.GetUserInfo(this.networkIdentity.OwnerId);

                    if (userInfo != null)
                    {
                        this.PlayerId = userInfo.GetPlayFabId();
                        this.IsTracking = true;

                        DissonanceManager.Instance.DissonanceComms.TrackPlayerPosition(this);
                    }

                    yield return null;
                }
            }
        }

        private void OnDisable()
        {
            if (this.IsTracking)
            {
                this.IsTracking = false;

                if (DissonanceManager.Instance)
                {
                    DissonanceManager.Instance.DissonanceComms.StopTracking(this);
                }
            }
        }

#endif
    }
}

#endif
