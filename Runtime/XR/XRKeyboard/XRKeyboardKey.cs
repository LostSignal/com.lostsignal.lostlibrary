//-----------------------------------------------------------------------
// <copyright file="XRKeyboardKey.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(LostButton))]
    public class XRKeyboardKey : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField][HideInInspector] private TMP_Text text;
        [SerializeField][HideInInspector] private Button button;
        [SerializeField][HideInInspector] private XRKeyboard xrKeyboard;
#pragma warning restore 0649

        private XRKeyboard keyboard;
        private char keyChar;
        private string keyString;
        private string secondaryKeys;
        private System.Action<char, string> keyPressed;

        public void SetData(XRKeyboard keyboard, char keyChar, string keyString, System.Action<char, string> keyPressed)
        {
            this.keyboard = keyboard;
            this.keyChar = keyChar;
            this.keyString = keyString;
            this.keyPressed = keyPressed;
            this.secondaryKeys = keyboard.CurrentKeyboard.GetSeconardyKeys(keyChar);
            this.text.text = keyString;

            //// TODO [bgish]: Do something with the secondary keys (show them on hover?)
        }

        public void ProcessKeyPress()
        {
            this.keyPressed?.Invoke(this.keyChar, this.keyString);
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.text);
            this.AssertGetComponent(ref this.button);
            this.AssertGetComponentInParent(ref this.xrKeyboard);
        }

        private void Awake()
        {
            this.OnValidate();
            this.button.onClick.AddListener(this.ProcessKeyPress);
        }
    }
}
