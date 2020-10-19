//-----------------------------------------------------------------------
// <copyright file="XRKeyboardData.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Lost/XR Keyboard Data")]
    public class XRKeyboardData : ScriptableObject
    {
        [SerializeField] private List<Keyboard> keyboards = new List<Keyboard>();

        [Serializable]
        public class Keyboard
        {
            [SerializeField] private string name;
            [SerializeField][Multiline(4)] private string lowerCaseText;
            [SerializeField][Multiline(4)] private string upperCaseText;
            [SerializeField][Multiline(4)] private string numbersText;
            [SerializeField][Multiline(4)] private string symbolsText;
            [SerializeField][Multiline(5)] private string keypadText;
            [SerializeField] private List<string> alternates = new List<string>();
        }

        [Serializable]
        public class Key
        {
            [SerializeField] private string key;
            [SerializeField] private List<string> alternateKeys;
        }
    }
}
