//-----------------------------------------------------------------------
// <copyright file="RotateGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class RotateGameObject : GameObjectStateAction
    {
#pragma warning disable 0649
        [SerializeField] private Transform transformToRotate;
        [SerializeField] private Quaternion localRotation;
        [SerializeField] private AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
#pragma warning restore 0649

        private Coroutine coroutine;
        private Quaternion originalRoation;
        private Quaternion finalRotation;

        public override string EditorDisplayName => this.transformToRotate != null ? $"Rotate {this.transformToRotate.name}" : "Rotate";

        public override void Apply()
        {
            this.StopCoroutine();

            this.originalRoation = this.transformToRotate.localRotation;
            this.finalRotation = this.localRotation;
            this.coroutine = this.curve.Animate(this.SetRotation);
        }

        public override void Revert()
        {
            this.StopCoroutine();
        }

        private void StopCoroutine()
        {
            if (this.coroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(this.coroutine);
                this.coroutine = null;
            }
        }

        private void SetRotation(float time, float progress)
        {
            this.transformToRotate.localRotation = Quaternion.Slerp(this.originalRoation, this.finalRotation, progress);
        }
    }
}
