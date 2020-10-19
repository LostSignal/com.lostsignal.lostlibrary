//-----------------------------------------------------------------------
// <copyright file="NetworkLineRenderer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2018_3_OR_NEWER

namespace Lost.Networking
{
    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public class NetworkLineRenderer : NetworkBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private bool doPositionsChange;
#pragma warning restore 0649

        private Vector3[] currentPositions = new Vector3[100];
        private uint currentPositionsCount;

        private Vector3[] desiredPositions = new Vector3[100];
        private uint desiredPositionsCount;
        private uint version;

        public override void Serialize(NetworkWriter writer)
        {
            this.currentPositionsCount = (uint)this.lineRenderer.GetPositions(this.currentPositions);

            bool useWorldSpace = this.lineRenderer.useWorldSpace;

            writer.WritePackedUInt32(++this.version);
            writer.Write(this.lineRenderer.enabled);
            writer.WritePackedUInt32(this.currentPositionsCount);

            for (int i = 0; i < this.currentPositionsCount; i++)
            {
                if (useWorldSpace)
                {
                    writer.Write(NetworkTransformAnchor.InverseTransformPosition(this.currentPositions[i]));
                }
                else
                {
                    writer.Write(this.currentPositions[i]);
                }
            }
        }

        public override void Deserialize(NetworkReader reader)
        {
            var messageVersion = reader.ReadPackedUInt32();

            if (this.version > messageVersion)
            {
                return;
            }

            this.version = messageVersion;
            this.lineRenderer.enabled = reader.ReadBoolean();
            this.desiredPositionsCount = reader.ReadPackedUInt32();

            bool useWorldSpace = this.lineRenderer.useWorldSpace;

            for (int i = 0; i < this.desiredPositionsCount; i++)
            {
                this.desiredPositions[i] = useWorldSpace ?
                    NetworkTransformAnchor.TransformPosition(reader.ReadVector3()) :
                    this.desiredPositions[i] = reader.ReadVector3();
            }
        }

        protected override void NetworkUpdate()
        {
            base.NetworkUpdate();

            if (this.IsOwner == false)
            {
                this.currentPositionsCount = (uint)this.lineRenderer.GetPositions(this.currentPositions);

                // Lerp all the position that we already have
                for (int i = 0; i < this.currentPositionsCount; i++)
                {
                    float distance = Vector3.Distance(this.currentPositions[i], this.desiredPositions[i]);
                    this.currentPositions[i] = Vector3.MoveTowards(this.currentPositions[i], this.desiredPositions[i], distance * this.UpdateFrequency);
                }

                // If more have been added then snap to the new desired positions
                if (this.currentPositionsCount != this.desiredPositionsCount)
                {
                    this.lineRenderer.positionCount = (int)this.desiredPositionsCount;

                    for (uint i = this.currentPositionsCount; i < this.desiredPositionsCount; i++)
                    {
                        this.currentPositions[i] = this.desiredPositions[i];
                    }
                }

                this.lineRenderer.SetPositions(this.currentPositions);
            }
        }
    }
}

#endif
