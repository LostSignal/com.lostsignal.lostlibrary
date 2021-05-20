//-----------------------------------------------------------------------
// <copyright file="GuidComponent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Experimental.SceneManagement;
    using UnityEditor.SceneManagement;
#endif

    // This component gives a GameObject a stable, non-replicatable Globally Unique IDentifier.
    // It can be used to reference a specific instance of an object no matter where it is.
    // This can also be used for other systems, such as Save/Load game
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class GuidComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        // System guid we use for comparison and generation
        private System.Guid guid = System.Guid.Empty;

        // Unity's serialization system doesn't know about System.Guid, so we convert to a byte array
        // Fun fact, we tried using strings at first, but that allocated memory and was twice as slow
        [SerializeField]
        private byte[] serializedGuid;

        public bool IsGuidAssigned()
        {
            return this.guid != System.Guid.Empty;
        }

        // We cannot allow a GUID to be saved into a prefab, and we need to convert to byte[]
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // This lets us detect if we are a prefab instance or a prefab asset.
            // A prefab asset cannot contain a GUID since it would then be duplicated when instanced.
            if (this.IsAssetOnDisk())
            {
                this.serializedGuid = null;
                this.guid = System.Guid.Empty;
            }
            else
#endif
            {
                if (this.guid != System.Guid.Empty)
                {
                    this.serializedGuid = this.guid.ToByteArray();
                }
            }
        }

        // On load, we can go head a restore our system guid for later use
        public void OnAfterDeserialize()
        {
            if (this.serializedGuid != null && this.serializedGuid.Length == 16)
            {
                this.guid = new System.Guid(this.serializedGuid);
            }
        }

        // Never return an invalid GUID
        public System.Guid GetGuid()
        {
            if (this.guid == System.Guid.Empty && this.serializedGuid != null && this.serializedGuid.Length == 16)
            {
                this.guid = new System.Guid(this.serializedGuid);
            }

            return this.guid;
        }

        // Let the manager know we are gone, so other objects no longer find this
        public void OnDestroy()
        {
            GuidManager.Remove(this.guid);
        }

        private void Awake()
        {
            this.CreateGuid();
        }

        // When de-serializing or creating this component, we want to either restore our serialized GUID
        // or create a new one.
        private void CreateGuid()
        {
            // if our serialized data is invalid, then we are a new object and need a new GUID
            if (this.serializedGuid == null || this.serializedGuid.Length != 16)
            {
#if UNITY_EDITOR
                // if in editor, make sure we aren't a prefab of some kind
                if (this.IsAssetOnDisk())
                {
                    return;
                }

                Undo.RecordObject(this, "Added GUID");
#endif
                this.guid = System.Guid.NewGuid();
                this.serializedGuid = this.guid.ToByteArray();

#if UNITY_EDITOR
                // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
                // force a save of the modified prefab instance properties
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }
#endif
            }
            else if (this.guid == System.Guid.Empty)
            {
                // otherwise, we should set our system guid to our serialized guid
                this.guid = new System.Guid(this.serializedGuid);
            }

            // register with the GUID Manager so that other components can access this
            if (this.guid != System.Guid.Empty)
            {
                if (!GuidManager.Add(this))
                {
                    // if registration fails, we probably have a duplicate or invalid GUID, get us a new one.
                    this.serializedGuid = null;
                    this.guid = System.Guid.Empty;
                    this.CreateGuid();
                }
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            // similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
            // at a time that lets us detect what we are
            if (this.IsAssetOnDisk())
            {
                this.serializedGuid = null;
                this.guid = System.Guid.Empty;
            }
            else
#endif
            {
                this.CreateGuid();
            }
        }

#if UNITY_EDITOR
        private bool IsEditingInPrefabMode()
        {
            if (EditorUtility.IsPersistent(this))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(this.gameObject);
                if (currentStage != mainStage)
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage(this.gameObject);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsAssetOnDisk()
        {
            return PrefabUtility.IsPartOfPrefabAsset(this) || this.IsEditingInPrefabMode();
        }

#endif
    }
}

#endif
