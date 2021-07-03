//-----------------------------------------------------------------------
// <copyright file="SplinePoint.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    public class SplinePoint : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private bool autoOrient = true;
        [SerializeField] private Vector3 inHandle = new Vector3(0, 0, -0.25f);
        [SerializeField] private Vector3 outHandle = new Vector3(0, 0, 0.25f);

        [SerializeField] [HideInInspector] private Transform myTransform;
        [SerializeField] [HideInInspector] private Spline parent;
        #pragma warning restore 0649

        private bool isIntialized;
        private SplinePoint nextPoint;
        private float length;

        public bool AutoOrient
        {
            get { return this.autoOrient; }
        }

        public Vector3 InHandle
        {
            get { return this.inHandle; }

            #if UNITY_EDITOR
            set { this.inHandle = value; }
            #endif
        }

        public Vector3 OutHandle
        {
            get { return this.outHandle; }

            #if UNITY_EDITOR
            set { this.outHandle = value; }
            #endif
        }

        public SplinePoint Next
        {
            get { return this.nextPoint; }
        }

        public float Length
        {
            get { return this.length; }
        }

        public void Initialize()
        {
            if (this.isIntialized)
            {
                return;
            }

            this.isIntialized = true;

            this.OnValidate();

            int splinePointCount = this.parent.GetSplinePointCount();
            int myIndex = this.parent.GetSplinePointIndex(this);
            bool isLastSplinePoint = myIndex == splinePointCount - 1;

            // If we've hit the end of the spline, early out
            if (isLastSplinePoint && this.parent.IsLooping == false)
            {
                return;
            }

            int nextIndex = (myIndex + 1) % splinePointCount;
            this.nextPoint = this.parent.GetSplinePoint(nextIndex);
            this.length = this.GetLengthBetweenSplinePoints(this, this.nextPoint);
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.myTransform);
            this.AssertGetComponentInParent(ref this.parent);
        }

        private void Awake()
        {
            this.Initialize();
        }

        private float GetLengthBetweenSplinePoints(SplinePoint p1, SplinePoint p2)
        {
            float length = 0.0f;

            for (int i = 0; i < 100; i++)
            {
                Vector3 v1 = Spline.Interpolate(p1, p2, (i + 0) / 100.0f);
                Vector3 v2 = Spline.Interpolate(p1, p2, (i + 1) / 100.0f);
                length += (v2 - v1).magnitude;
            }

            return length;
        }
    }
}

#endif
