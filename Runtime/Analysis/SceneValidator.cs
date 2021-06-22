//-----------------------------------------------------------------------
// <copyright file="SceneValidator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    //// ### Scene Validator
    //// * Missing Behaviour Errors
    ////   * The referenced script on this Behaviour (Game Object 'Oroshi Footstep Decal') is missing!
    //// * Bad Collider Error
    //// * Bad Scale Error
    //// * FPS Below 60 Error
    //// * General LogError/Exception/Warning Error (if not one of the above)
    //// 
    //// 
    //// ### Build Validation
    //// * If build spits out any Debug.LogError like this, then it needs to be reported
    ////   * Failed to create Physics Mesh from source mesh "Roof_LOD0". One of the triangles is too large. It's recommended to tesselate the large triangles.
    ////     UnityEngine.GUIUtility:ProcessEvent (int,intptr)
    //// * Should this be spit out to a slack channel?
    //// * When entering playmode should we put a popup in the face of the user????
    //// * Any code warnings should be errors that need action items to fix
    //// * Any physics collision runtime rebuilds must be errors in this validation system (must listen to console out to catch these)
    ////   * Can I use Preprocess Build and PostProcess build to catch these?
    ////   * Is the float PreProcessBuild -> Process Scene(s) -> PostProcessBuild????
    ////   * What about scene that live in addressables... Is Process Scene called on them too?
    //// 
    //// * Can I make a Scene Validator that finds all UnityEvents in a scene and makes sure if they
    ////   reference a GameObjectState object that the StateName parameter is valid?
    //// 
    //// 
    //// 

    public class SceneValidator : MonoBehaviour
    {
        #if UNITY_EDITOR

        #pragma warning disable 0649
        [SerializeReference] private List<Validator> validators;
        #pragma warning restore 0649

        private void OnValidate()
        {
            this.validators = this.validators ?? new List<Validator>();

            // Get all classes that implement Validator
            // Add them to the validators list if an instace of them doesn't already exist in the list
        }

        [Serializable]
        public abstract class Validator
        {
            #pragma warning disable 0649
            [ReadOnly]
            [SerializeField] protected string name;
            [SerializeField] protected bool disableValidator;
            [SerializeField] protected GameObject objectsToIgnore;
            #pragma warning restore 0649

            public string Name => this.name;

            public bool DisableValidator => this.disableValidator;

            public GameObject ObjectsToIgnore => this.objectsToIgnore;

            public virtual List<Error> Run()
            {
                return null;
            }
        }

        public class Error
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public GameObject AffectedObject { get; set; }
        }

        #endif
    }
}

#endif
