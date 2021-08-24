//-----------------------------------------------------------------------
// <copyright file="VRIK.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//// NOTE [bgish]: There is a bug in StyleCop that thinks that Local Functions need this prefix, so turning it off.
#pragma warning disable SA1101

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using Lost.Haven;
    using UnityEngine;
    using UnityEngine.Animations.Rigging;

    public class VRIK : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Transform root;
        [SerializeField] private Transform headConstraint;
        [SerializeField] private float turnSmoothness = 5.0f;

        [SerializeField] private VRMap head = new VRMap
        {
            TrackingPositionOffset = new Vector3(0.0f, -0.23f, -0.18f),
            TrackingRotationOffset = new Vector3(180.0f, 90.0f, 90.0f),
        };

        [SerializeField] private VRMap leftHand = new VRMap { TrackingRotationOffset = new Vector3(-90.0f, 90.0f, 0.0f) };
        [SerializeField] private VRMap rightHand = new VRMap { TrackingRotationOffset = new Vector3(90.0f, -90.0f, 0.0f) };
        #pragma warning restore 0649

        private Vector3 headBodyOffset;
        private HavenRig rig;

        [ExposeInEditor("Setup Rig")]
        public void SetupRig()
        {
            var rigAnimator = this.GetComponent<Animator>();
            var rigNameToTransform = new Dictionary<string, Transform>();
            var humanNameToBoneName = new Dictionary<string, string>();

            // Making a quick dictionary lookup for transforms and their names
            foreach (var transform in this.root.GetComponentsInChildren<Transform>())
            {
                rigNameToTransform.Add(transform.name, transform);
            }

            // Setting up the Bone Renderer
            var boneTransforms = new List<Transform>();
            foreach (var humanBone in rigAnimator.avatar.humanDescription.human)
            {
                humanNameToBoneName.Add(humanBone.humanName, humanBone.boneName);
                boneTransforms.Add(rigNameToTransform[humanBone.boneName]);
            }

            var boneRenderer = GetOrAddComponentToGameObject<BoneRenderer>(this.gameObject);
            boneRenderer.transforms = boneTransforms.ToArray();

            // Setting up Rig
            var vrConstraints = GetOrCreateChildGameObject(this.gameObject, "VR Constraints");
            var rig = GetOrAddComponentToGameObject<Rig>(vrConstraints);

            // Setting up Rig Builder
            var rigBuilder = GetOrAddComponentToGameObject<RigBuilder>(this.gameObject);

            if (rigBuilder.layers == null || rigBuilder.layers.Count == 0 || rigBuilder.layers[0].rig != rig)
            {
                rigBuilder.layers = new List<RigLayer> { new RigLayer(rig) };
            }

            // Setting Two Bone IK Constraints (Arms)
            SetupTwoBoneContraint(vrConstraints, "Right Arm IK", "RightUpperArm", "RightLowerArm", "RightHand", this.rightHand);
            SetupTwoBoneContraint(vrConstraints, "LeftArm IK", "LeftUpperArm", "LeftLowerArm", "LeftHand", this.leftHand);

            // Setting up Head Constraint
            var headBone = rigNameToTransform[humanNameToBoneName["Head"]];

            var headConstraint = GetOrCreateChildGameObject(vrConstraints, "Head Constraint");
            headConstraint.transform.SetPositionAndRotation(headBone.position, headBone.rotation);

            var multiParentConstraint = GetOrAddComponentToGameObject<MultiParentConstraint>(headConstraint);
            multiParentConstraint.data.constrainedObject = headBone;
            multiParentConstraint.data.sourceObjects = new WeightedTransformArray
            {
                new WeightedTransform { transform = headConstraint.transform, weight = 1.0f },
            };

            // Storing Head Constraint for Start/LateUpdate function
            this.headConstraint = headConstraint.transform;

            void SetupTwoBoneContraint(GameObject parent, string gameObjectName, string rootName, string midName, string tipName, VRMap vrMap)
            {
                var armIk = GetOrCreateChildGameObject(parent, gameObjectName);
                var armIkConstraint = GetOrAddComponentToGameObject<TwoBoneIKConstraint>(armIk);

                armIkConstraint.data.root = rigNameToTransform[humanNameToBoneName[rootName]];
                armIkConstraint.data.mid = rigNameToTransform[humanNameToBoneName[midName]];
                armIkConstraint.data.tip = rigNameToTransform[humanNameToBoneName[tipName]];

                // Setup Target
                var target = GetOrCreateChildGameObject(armIk, "Target");
                target.transform.SetPositionAndRotation(armIkConstraint.data.tip.position, armIkConstraint.data.tip.rotation);
                armIkConstraint.data.target = target.transform;

                // Setup Hint
                Vector3 rootToMid = armIkConstraint.data.root.position - armIkConstraint.data.mid.position;
                Vector3 tipToMid = armIkConstraint.data.tip.position - armIkConstraint.data.mid.position;
                Vector3 average = (rootToMid.normalized + tipToMid.normalized) / -2.0f;
                Vector3 hintPosition = armIkConstraint.data.mid.position + (average * 0.1f);

                var hint = GetOrCreateChildGameObject(armIk, "Hint");
                hint.transform.position = hintPosition;
                armIkConstraint.data.hint = hint.transform;

                // Updating VR Map object
                vrMap.RigTarget = target.transform;
            }

            T GetOrAddComponentToGameObject<T>(GameObject gameObject)
                where T : Component
            {
                var component = gameObject.GetComponent<T>();

                if (component == null)
                {
                    return gameObject.AddComponent<T>();
                }

                return component;
            }

            GameObject GetOrCreateChildGameObject(GameObject gameObject, string name)
            {
                var childTransform = gameObject.transform.Find(name);

                if (childTransform == null)
                {
                    childTransform = new GameObject(name).transform;
                    childTransform.SetParent(gameObject.transform);
                    childTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    childTransform.localScale = Vector3.one;
                }

                return childTransform.gameObject;
            }
        }

        private void Start()
        {
            this.headBodyOffset = this.transform.position - this.headConstraint.position;
            this.enabled = false;

            Bootloader.OnManagersReady += Initialize;

            void Initialize()
            {
                if (XRManager.Instance.CurrentDevice.XRType == XRType.VRHeadset)
                {
                    this.rig = HavenRig.GetRig();
                    this.enabled = true;
                }
            }
        }

        private void FixedUpdate()
        {
            this.transform.position = this.headConstraint.position + this.headBodyOffset;

            this.transform.forward = Vector3.Lerp(
                this.transform.forward,
                Vector3.ProjectOnPlane(this.headConstraint.up, Vector3.up).normalized,
                Time.deltaTime * this.turnSmoothness);

            this.head.Map(this.rig.RigCamera.transform);
            this.leftHand.Map(this.rig.LeftController.transform);
            this.rightHand.Map(this.rig.RightController.transform);
        }

        [Serializable]
        private class VRMap
        {
            #pragma warning disable 0649, CA2235
            [SerializeField] private Transform rigTarget;
            [SerializeField] private Vector3 trackingPositionOffset;
            [SerializeField] private Vector3 trackingRotationOffset;
            #pragma warning restore 0649, CA2235

            public Transform RigTarget
            {
                get => this.rigTarget;
                set => this.rigTarget = value;
            }

            public Vector3 TrackingPositionOffset
            {
                get => this.trackingPositionOffset;
                set => this.trackingPositionOffset = value;
            }

            public Vector3 TrackingRotationOffset
            {
                get => this.trackingRotationOffset;
                set => this.trackingRotationOffset = value;
            }

            public void Map(Transform targetTransform)
            {
                this.rigTarget.SetPositionAndRotation(
                    targetTransform.TransformPoint(this.trackingPositionOffset),
                    targetTransform.rotation * Quaternion.Euler(this.trackingRotationOffset));
            }
        }
    }
}
