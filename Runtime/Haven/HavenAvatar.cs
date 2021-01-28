//-----------------------------------------------------------------------
// <copyright file="HavenAvatar.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
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
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;

        [SerializeField] private Renderer[] allRenderers;
        [SerializeField] private Renderer[] tintedMeshRenderers;
        [SerializeField] [HideInInspector] private NetworkIdentity networkIdentity;
        [SerializeField] [HideInInspector] private DissonancePlayerTracker dissonancePlayerTracker;
#pragma warning restore 0649

        private HavenRig havenRig;
        private Color avatarColor;
        private bool isPancake;
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
            this.ShowAvatar(false);
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
                    this.transform.position = this.havenRig.RigCamera.transform.position;
                    this.transform.rotation = this.havenRig.RigCamera.transform.rotation;
                    this.transform.localScale = new Vector3(this.havenRig.RigScale, this.havenRig.RigScale, this.havenRig.RigScale);

                    if (this.isPancake == false)
                    {
                        this.leftController.position = this.havenRig.LeftController.position;
                        this.leftController.rotation = this.havenRig.LeftController.rotation;
                        this.rightController.position = this.havenRig.RightController.position;
                        this.rightController.rotation = this.havenRig.RightController.rotation;
                    }
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
                        this.avatarColor = ColorUtil.ParseColorHexString(userInfo.CustomData["Color"]);
                        this.isPancake = userInfo.CustomData["Platform"] == "Pancake";

                        this.UpdateControllers(this.isPancake);
                        this.SetAvatarColor(this.avatarColor);
                        this.SetAvatarName(userInfo);
                        this.ShowAvatar(true);

                        //// TODO [bgish]: Start coroutine for updating Audio

                        break;
                    }
                }

                yield return null;
            }
        }

        private void SetAvatarColor(Color avatarColor)
        {
            if (this.tintedMeshRenderers != null)
            {
                foreach (var meshRenderer in this.tintedMeshRenderers)
                {
                    meshRenderer.material.color = avatarColor;
                }
            }
        }

        private void SetAvatarName(UserInfo userInfo)
        {
            string displayName = userInfo.GetDisplayName();

            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = $"Player{userInfo.GetPlayFabId().Substring(0, 4)}";
            }

            //// TODO [bgish]: Put facing text object above head and set to this
        }

        private void ShowAvatar(bool show)
        {
            if (this.allRenderers != null)
            {
                for (int i = 0; i < this.allRenderers.Length; i++)
                {
                    this.allRenderers[i].enabled = show;
                }
            }
        }

        private void UpdateControllers(bool isPancake)
        {
            this.leftController.gameObject.SetActive(this.isPancake == false);
            this.rightController.gameObject.SetActive(this.isPancake == false);
        }
    }
}

#endif
