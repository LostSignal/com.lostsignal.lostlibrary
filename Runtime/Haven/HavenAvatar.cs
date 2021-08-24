//-----------------------------------------------------------------------
// <copyright file="HavenAvatar.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

namespace Lost.Haven
{
    using System.Collections;
    using Lost;
    using Lost.DissonanceIntegration;
    using Lost.Networking;
    using UnityEngine;

    public enum HavenAvatarVisualsType
    {
        HeadOnly,
        HeadHands,
        FullBodyIK,
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class HavenAvatar : NetworkBehaviour
    {
        public static readonly ObjectTracker<HavenAvatar> Avatars = new ObjectTracker<HavenAvatar>(20);

#pragma warning disable 0649
        [SerializeField] private DissonancePlayerTracker dissonancePlayerTracker;

        [Header("Visuals")]
        [SerializeField] private HavenAvatarVisuals avatarVisualsHeadOnly;
        [SerializeField] private HavenAvatarVisuals avatarVisualsHeadHands;

        [HideInInspector]
        [SerializeField] private Transform avatarTransform;
#pragma warning restore 0649

        private HavenAvatarVisuals currentAvatarVisuals;
        private HavenRig havenRig;

        //// public static ObjectTracker<HavenAvatar> Avatars
        //// {
        ////     get => avatars;
        //// }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.havenRig.RigScale);
            writer.Write(this.havenRig.transform.position);
            writer.Write(this.havenRig.transform.rotation);

            bool hasVisuals = this.currentAvatarVisuals != null;
            writer.Write(hasVisuals);

            if (hasVisuals)
            {
                this.currentAvatarVisuals.Serialize(writer);
            }
        }

        public override void Deserialize(NetworkReader reader)
        {
            float rigScale = reader.ReadSingle();
            Vector3 rigPosition = reader.ReadVector3();
            Quaternion rigRotation = reader.ReadQuaternion();

            // Setting the avatar PRT
            this.avatarTransform.localScale = new Vector3(rigScale, rigScale, rigScale);
            this.avatarTransform.position = rigPosition;
            this.avatarTransform.rotation = rigRotation;

            // Deserializing Visualization State (if it exists)
            bool hasVisuals = reader.ReadBoolean();
            if (hasVisuals && this.currentAvatarVisuals != null)
            {
                this.currentAvatarVisuals.Deserialize(reader);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            this.AssertGetComponent(ref this.avatarTransform);
            this.AssertGetComponentInChildren(ref this.dissonancePlayerTracker);
        }

        protected override void Awake()
        {
            base.Awake();

            this.OnValidate();
        }

        protected override SendConfig GetInitialSendConfig()
        {
            return new SendConfig
            {
                NetworkUpdateType = NetworkUpdateType.Tick,
                SendReliable = false,
                UpdateFrequency = 0.1f,
            };
        }

        private void OnEnable()
        {
            Avatars.Add(this);
        }

        private void OnDisable()
        {
            Avatars.Remove(this);
        }

        private IEnumerator Start()
        {
            // If we're the owner, then get a reference to the Rig
            while (this.IsOwner && this.havenRig == null)
            {
                this.havenRig = HavenRig.GetRig();
                yield return null;
            }

            yield return this.InitializeAvatarCoroutine();
        }

        private IEnumerator InitializeAvatarCoroutine()
        {
            UserInfo userInfo;

            while (true)
            {
                if (NetworkingManager.IsInitialized && NetworkingManager.Instance.HasJoinedServer)
                {
                    userInfo = NetworkingManager.Instance.GetUserInfo(this.Identity.OwnerId);

                    if (userInfo != null &&
                        userInfo.CustomData.ContainsKey("Color") &&
                        userInfo.CustomData.ContainsKey("Platform"))
                    {
                        var avatarColor = ColorUtil.ParseColorHexString(userInfo.CustomData["Color"]);
                        var isPancake = userInfo.CustomData["Platform"] == "Pancake";

                        // TODO [bgish]: Need better way of selecting what visuals the user is using
                        if (isPancake)
                        {
                            this.currentAvatarVisuals = this.avatarVisualsHeadOnly;
                        }
                        else
                        {
                            this.currentAvatarVisuals = this.avatarVisualsHeadHands;
                        }

                        this.currentAvatarVisuals.SetAvatarName(userInfo);
                        this.currentAvatarVisuals.SetAvatarColor(avatarColor);
                        this.currentAvatarVisuals.gameObject.SetActive(true);

                        //// TODO [bgish]: Start coroutine for updating Audio

                        break;
                    }
                }

                yield return null;
            }
        }
    }
}

#endif
