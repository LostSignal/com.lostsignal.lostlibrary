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
        private char key;
        private string secondaryKeys;
        private System.Action<char> keyPressed;

        public void SetData(XRKeyboard keyboard, char key, System.Action<char> keyPressed)
        {
            this.keyboard = keyboard;
            this.key = key;
            this.keyPressed = keyPressed;
            this.secondaryKeys = keyboard.CurrentKeyboard.GetSeconardyKeys(key);
            this.text.text = key.ToString();

            //// TODO [bgish]: Do something with the secondary keys (show them on hover?)
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
            this.button.onClick.AddListener(this.OnButtonClick);
        }

        private void OnButtonClick()
        {
            this.keyPressed?.Invoke(this.key);
        }
    }
}
