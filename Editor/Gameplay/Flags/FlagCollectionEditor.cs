#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="FlagCollectionEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEditor;

    [CustomEditor(typeof(FlagCollection))]
    public class FlagCollectionEditor : Editor
    {
        private HashSet<int> ids = new HashSet<int>();
        private HashSet<string> names = new HashSet<string>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ids.Clear();
            names.Clear();

            var flagCollection = this.target as FlagCollection;

            foreach (var flag in flagCollection.Flags)
            {
                if (flag.Id < 0)
                {
                    EditorGUILayout.HelpBox($"FlagCollection {this.name} has a flag {flag.Name} with a negative id, this will break the flag system.", MessageType.Error);
                }

                if (flag.Id > BitArray.MaxBitIndex)
                {
                    EditorGUILayout.HelpBox($"FlagCollection {this.name} has a flag {flag.Name} with an id greater than BitArray.MaxBitIndex {BitArray.MaxBitIndex}, this will break the flag system.", MessageType.Error);
                }

                if (ids.Contains(flag.Id))
                {
                    EditorGUILayout.HelpBox($"FlagCollection {this.name} has a flag {flag.Name} with a duplicate id {flag.Id}, this will break the flag system.", MessageType.Error);
                }

                if (names.Contains(flag.Name))
                {
                    EditorGUILayout.HelpBox($"FlagCollection {this.name} has a flag {flag.Name} with a duplicate name, this will break the flag system.", MessageType.Error);
                }

                ids.AddIfUnique(flag.Id);
                names.AddIfUnique(flag.Name);
            }
        }
    }
}
