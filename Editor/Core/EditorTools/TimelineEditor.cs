//-----------------------------------------------------------------------
// <copyright file="TimelineEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Playables;

    [CustomEditor(typeof(PlayableDirector))]
    public class TimelineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying && GUILayout.Button("Play"))
            {
                ((PlayableDirector)this.target).Play();
            }
        }
    }
}
