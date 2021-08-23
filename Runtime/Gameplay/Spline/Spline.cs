//-----------------------------------------------------------------------
// <copyright file="Spline.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class Spline : MonoBehaviour
    {
        private static readonly List<SplinePoint> SplinePointsCache = new List<SplinePoint>();

        #pragma warning disable 0649
        [SerializeField] private bool isLooping;
        [SerializeField][HideInInspector] private List<SplinePoint> children;
        #pragma warning restore 0649

        // Used to calculate the length of the spline
        private float splineLength;

        // Used for caching
        private float lastDesiredLength = float.MaxValue;
        private float currentSplineLength = 0;
        private int cachedIndex = 0;

        public bool IsLooping
        {
            get { return this.isLooping; }
        }

        public float SplineLength
        {
            get { return this.splineLength; }
        }

        public static Vector3 Interpolate(SplinePoint p1, SplinePoint p2, float percentage)
        {
            Vector3 p1LocalPosition = p1.LocalPosition;
            Vector3 p2LocalPosition = p2.LocalPosition;

            Vector3 v1 = p1LocalPosition;
            Vector3 v2 = p1LocalPosition + (p1.LocalRotation * p1.OutHandle);
            Vector3 v3 = p2LocalPosition + (p2.LocalRotation * p2.InHandle);
            Vector3 v4 = p2LocalPosition;

            Vector3 interpolatedPoint = new Vector3(
                Interpolate(v1.x, v2.x, v3.x, v4.x, percentage),
                Interpolate(v1.y, v2.y, v3.y, v4.y, percentage),
                Interpolate(v1.z, v2.z, v3.z, v4.z, percentage));

            return p1.transform.parent.localToWorldMatrix.MultiplyPoint(interpolatedPoint);
        }

        public Vector3 Evaluate(float desiredLength)
        {
            if (this.children.Count == 0)
            {
                Debug.LogError($"Trying to Evaluate Spline {this.name} which has no Spline Points.", this);
                return Vector3.negativeInfinity;
            }

            // Early out if we've reached the end
            if (desiredLength >= this.splineLength)
            {
                return this.isLooping ? this.children[0].Position : this.children[this.children.Count - 1].Position;
            }

            // If this desiredLength isn't greater than the last, then start from beginning and don't used cached values
            if (desiredLength < this.lastDesiredLength)
            {
                this.cachedIndex = 0;
                this.currentSplineLength = 0.0f;
            }

            // Cache the desired length to test against for next time
            this.lastDesiredLength = desiredLength;

            for (int i = this.cachedIndex; i < this.children.Count; i++)
            {
                float currentLength = desiredLength - this.currentSplineLength;
                float childLength = this.children[i].Length;

                if (currentLength <= childLength)
                {
                    return Interpolate(this.children[i], this.children[i].Next, currentLength / childLength);
                }
                else
                {
                    this.currentSplineLength += childLength;
                    this.cachedIndex++;
                }
            }

            Debug.LogError("Unable to correctly evaluate spline", this);
            return this.children[0].Position;
        }

        public int GetSplinePointCount() => this.children.Count;

        public int GetSplinePointIndex(SplinePoint splinePoint)
        {
            return this.children.IndexOf(splinePoint);
        }

        public SplinePoint GetSplinePoint(int index)
        {
            return this.children[index];
        }

        private static float Interpolate(float p0, float p1, float p2, float p3, float t)
        {
            // Formula from "Cubic Bézier curves" section on http://en.wikipedia.org/wiki/B%C3%A9zier_curve
            return ((1.0f - t) * (1.0f - t) * (1.0f - t) * p0) +
                    (3 * (1.0f - t) * (1.0f - t) * t * p1) +
                    (3 * (1.0f - t) * t * t * p2) +
                    (t * t * t * p3);
        }

        private void Awake()
        {
            Debug.Assert(this.children.Count > 1, "Spline doesn't have enough child nodes.  Must have at least 2 spline points.", this);

            // Getting the total length by summing the children
            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].Initialize();
                this.splineLength += this.children[i].Length;
            }
        }

        #if UNITY_EDITOR

        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            SplinePointsCache.Clear();
            this.GetComponentsInChildren(SplinePointsCache);

            // Making sure we're initialized
            if (this.children == null)
            {
                this.children = new List<SplinePoint>();
            }

            // Making sure we at least have 2 Children to work with
            if (SplinePointsCache.Count == 0)
            {
                var p1 = new GameObject("SplinePoint (0)", typeof(SplinePoint));
                p1.transform.SetParent(this.transform);
                p1.transform.localPosition = Vector3.zero;

                var p2 = new GameObject("SplinePoint (1)", typeof(SplinePoint));
                p2.transform.SetParent(this.transform);
                p2.transform.localPosition = Vector3.one;

                return;
            }

            // If Children have changed (added/remove), then get the child list again
            if (this.children.Count != SplinePointsCache.Count)
            {
                this.children.Clear();
                this.children.AddRange(SplinePointsCache);
            }

            // Auto Orienting Child Points
            for (int i = 0; i < this.children.Count; i++)
            {
                SplinePoint currentPoint = this.children[i];

                if (currentPoint.AutoOrient == false)
                {
                    continue;
                }

                int previousIndex;
                int nextIndex;

                if (this.IsLooping)
                {
                    previousIndex = i == 0 ? this.children.Count - 1 : i - 1;
                    nextIndex = (i == (this.children.Count - 1)) ? 0 : i + 1;
                }
                else
                {
                    previousIndex = i == 0 ? 0 : i - 1;
                    nextIndex = (i == (this.children.Count - 1)) ? this.children.Count - 1 : i + 1;
                }

                Vector3 previousPosition = this.children[previousIndex].Position;
                Vector3 currentPosition = currentPoint.Position;
                Vector3 nextPosition = this.children[nextIndex].Position;

                // setting the spline points rotation
                Vector3 direction = nextPosition - previousPosition;
                currentPoint.transform.LookAt(currentPosition + direction);

                // setting the In/Out tangent lengths
                currentPoint.InHandle = new Vector3(0, 0, -Mathf.Max(0.25f, Vector3.Magnitude(previousPosition - currentPosition) * 0.3f));
                currentPoint.OutHandle = new Vector3(0, 0, Mathf.Max(0.25f, Vector3.Magnitude(nextPosition - currentPosition) * 0.3f));
            }
        }

        #endif
    }
}

#endif
