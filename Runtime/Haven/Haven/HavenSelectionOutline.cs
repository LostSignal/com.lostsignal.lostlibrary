//-----------------------------------------------------------------------
// <copyright file="HavenSelectionOutline.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Haven
{
    using UnityEngine;

    [AddComponentMenu("Haven XR/HXR Selection Outline")]
    public class HavenSelectionOutline : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Renderer selectionRenderer;
#pragma warning restore 0649

        private float highlighted = 0.0f;
        private MaterialPropertyBlock block;
        private int highlightActiveID;

        private void Start()
        {
            if (this.selectionRenderer == null)
            {
                this.selectionRenderer = this.GetComponent<Renderer>();
            }

            this.highlightActiveID = Shader.PropertyToID("HighlightActive");
            this.block = new MaterialPropertyBlock();
            this.block.SetFloat(this.highlightActiveID, this.highlighted);
            this.selectionRenderer.SetPropertyBlock(this.block);
        }

        public void Highlight()
        {
            this.highlighted = 1.0f;

            this.selectionRenderer.GetPropertyBlock(this.block);
            this.block.SetFloat(this.highlightActiveID, this.highlighted);
            this.selectionRenderer.SetPropertyBlock(this.block);
        }

        public void RemoveHighlight()
        {
            this.highlighted = 0.0f;

            this.selectionRenderer.GetPropertyBlock(this.block);
            this.block.SetFloat(this.highlightActiveID, this.highlighted);
            this.selectionRenderer.SetPropertyBlock(this.block);
        }
    }
}
