//-----------------------------------------------------------------------
// <copyright file="HavenAvatar.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace HavenXR
{
    using System.Collections;
    using Lost;
    using Lost.DissonanceIntegration;
    using Lost.Networking;
    using UnityEngine;

    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(DissonancePlayerTracker))]
    public class HavenAvatar : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Renderer[] allRenderers;
        [SerializeField] private Renderer[] tintedMeshRenderers;
        [SerializeField] [HideInInspector] private NetworkIdentity networkIdentity;
        [SerializeField] [HideInInspector] private DissonancePlayerTracker dissonancePlayerTracker;
#pragma warning restore 0649

        private HavenRig havenRig;
        private bool isOwner;

        public long OwnerId => this.networkIdentity.OwnerId;

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.networkIdentity);
            this.AssertGetComponent(ref this.dissonancePlayerTracker);
        }

        private void Awake()
        {
            this.OnValidate();

            // Hiding all the renderers
            if (this.allRenderers != null)
            {
                for (int i = 0; i < this.allRenderers.Length; i++)
                {
                    this.allRenderers[i].enabled = false;
                }
            }
        }

        private void OnEnable()
        {
            Bootloader.OnBoot += this.UpdateObjectTracker;
        }

        private void OnDisable()
        {
            Bootloader.OnBoot -= this.UpdateObjectTracker;
            this.UpdateObjectTracker();
        }

        private void Start()
        {
            this.isOwner = this.networkIdentity.IsOwner;

            if (this.isOwner == false)
            {
                this.StartCoroutine(this.ShowAvatarCoroutine());
            }
        }

        private void Update()
        {
            if (this.isOwner)
            {
                if (this.havenRig == null && ObjectTracker.IsInitialized)
                {
                    this.havenRig = ObjectTracker.Instance.GetFirstObject<HavenRig>();
                }

                if (this.havenRig != null)
                {
                    this.transform.position = this.havenRig.transform.position;
                    this.transform.rotation = this.havenRig.transform.rotation;

                    //// TODO [bgish]: Make sure to update any controllers as well
                }
            }
        }

        private void UpdateObjectTracker()
        {
            ObjectTracker.UpdateRegistration(this);
        }

        private IEnumerator ShowAvatarCoroutine()
        {
            UserInfo userInfo;

            while (true)
            {
                if (NetworkingManager.IsInitialized && NetworkingManager.Instance.HasJoinedServer)
                {
                    userInfo = NetworkingManager.Instance.GetUserInfo(this.networkIdentity.OwnerId);

                    if (userInfo != null &&
                        userInfo.CustomData.ContainsKey("Color") &&
                        userInfo.CustomData.ContainsKey("Platform"))
                    {
                        break;
                    }
                }

                yield return null;
            }

            // TODO [bgish]: Show all ui/rederers
            Color avatarColor = ColorUtil.ParseColorHexString(userInfo.CustomData["Color"]);

            if (this.tintedMeshRenderers != null)
            {
                foreach (var meshRenderer in this.tintedMeshRenderers)
                {
                    meshRenderer.material.color = avatarColor;
                }
            }

            //// TODO [bgish]: Set the Avatar Display Name

            // TODO [bgish]: Show all ui/meshes associated with this avatar
            if (this.allRenderers != null)
            {
                for (int i = 0; i < this.allRenderers.Length; i++)
                {
                    this.allRenderers[i].enabled = true;
                }
            }
        }
    }
}
