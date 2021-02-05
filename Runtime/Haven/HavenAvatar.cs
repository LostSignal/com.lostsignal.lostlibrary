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
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(NetworkIdentity))]
    public class HavenAvatar : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private NetworkIdentity networkIdentity;
        [SerializeField] private DissonancePlayerTracker dissonancePlayerTracker;

        [SerializeField] private Transform headTransform;
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;

        [Header("Canvas")]
        [SerializeField] private Canvas avatarCanvas;
        [SerializeField] private TMP_Text displayName;
        [SerializeField] private Vector3 canvasGlobalOffset;

        [Header("Rendering")]
        [SerializeField] private Renderer[] allRenderers;
        [SerializeField] private Renderer[] tintedMeshRenderers;
#pragma warning restore 0649

        private HavenRig havenRig;
        private Color avatarColor;
        private bool isPancake;
        private bool isOwner;

        public long OwnerId => this.networkIdentity.OwnerId;

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.networkIdentity);
            this.AssertGetComponentInChildren(ref this.dissonancePlayerTracker);
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
                    var rigScale = new Vector3(this.havenRig.RigScale, this.havenRig.RigScale, this.havenRig.RigScale);

                    this.headTransform.position = this.havenRig.RigCamera.transform.position;
                    this.headTransform.rotation = this.havenRig.RigCamera.transform.rotation;
                    this.headTransform.localScale = rigScale;

                    if (this.isPancake == false)
                    {
                        this.leftController.position = this.havenRig.LeftController.position;
                        this.leftController.rotation = this.havenRig.LeftController.rotation;
                        this.leftController.localScale = rigScale;

                        this.rightController.position = this.havenRig.RightController.position;
                        this.rightController.rotation = this.havenRig.RightController.rotation;
                        this.rightController.localScale = rigScale;
                    }
                }
            }
            else
            {
                // BUG [bgish]: Does not account for rig scale
                this.avatarCanvas.transform.position = this.headTransform.position + this.canvasGlobalOffset;
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

            if (this.displayName)
            {
                this.displayName.text = displayName;
            }
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

            if (this.avatarCanvas)
            {
                this.avatarCanvas.gameObject.SetActive(show);
            }
        }

        private void UpdateControllers(bool isPancake)
        {
            this.leftController.gameObject.SetActive(isPancake == false);
            this.rightController.gameObject.SetActive(isPancake == false);
        }
    }
}

#endif
