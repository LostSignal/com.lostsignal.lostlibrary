//-----------------------------------------------------------------------
// <copyright file="DestoryComponentsOnBuild.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DestoryComponentsOnBuild : MonoBehaviour
    {
#pragma warning disable 0649
        [EnumFlag]
        [SerializeField] private DevicePlatform platformsToKeep;
        [SerializeField] private List<Component> componentsToDestory;
#pragma warning restore 0649

#if UNITY_EDITOR

        public List<Component> ComponentsToDestory
        {
            get => this.componentsToDestory;
            set => this.componentsToDestory = value;
        }

        [EditorEvents.OnProcessScene]
        private static void OnProcessScene()
        {
            if (Application.isPlaying == false && (Application.isBatchMode || UnityEditor.BuildPipeline.isBuildingPlayer))
            {
                foreach (var destoryOnBuild in GameObject.FindObjectsOfType<DestoryComponentsOnBuild>())
                {
                    if (destoryOnBuild && destoryOnBuild.componentsToDestory?.Count > 0 && destoryOnBuild.ShouldDestroyObject())
                    {
                        foreach (var component in destoryOnBuild.componentsToDestory)
                        {
                            GameObject.DestroyImmediate(component);
                        }
                    }
                }
            }
        }

        private void Awake()
        {
            if (this.ShouldDestroyObject())
            {
                foreach (var component in this.componentsToDestory)
                {
                    GameObject.Destroy(component);
                }
            }
        }

        private bool ShouldDestroyObject()
        {
            return (Platform.CurrentDevicePlatform & this.platformsToKeep) == 0;
        }

#endif
    }
}

#endif
