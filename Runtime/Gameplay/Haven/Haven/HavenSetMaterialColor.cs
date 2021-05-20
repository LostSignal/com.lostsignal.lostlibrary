//-----------------------------------------------------------------------
// <copyright file="HavenSetMaterialColor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Haven
{
    using UnityEngine;

    //// NOTE [bgish]: This is a temporary class that will be replaced by lost actions.  Purely using this for testing purposes only
    public class HavenSetMaterialColor : MonoBehaviour
    {
        public Renderer rendererToSet;
        public Color color;

        public void SetColor()
        {
            this.rendererToSet.material.color = this.color;
        }
    }
}

#endif
