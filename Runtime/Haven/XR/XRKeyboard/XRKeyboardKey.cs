//-----------------------------------------------------------------------
// <copyright file="XRKeyboardKey.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class XRKeyboardKey : LostButton
    {
#pragma warning disable 0649
        [SerializeField][HideInInspector] private TMP_Text text;
        [SerializeField][HideInInspector] private XRKeyboard xrKeyboard;
#pragma warning restore 0649

        private char keyChar;
        private string keyString;
        private string secondaryKeys;
        private System.Action<char, string> keyPressed;

        public void SetData(XRKeyboard keyboard, char keyChar, string keyString, System.Action<char, string> keyPressed)
        {
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

        public override void OnPointerClick(PointerEventData eventData)
        {
            this.ProcessKeyPress();
            base.OnPointerClick(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            InputFieldTracker.ReselectLastKnownInputField();
            base.OnPointerDown(eventData);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.FindComponents();
        }

#endif

        protected override void Awake()
        {
            base.Awake();
            this.FindComponents();
        }

        private void FindComponents()
        {
            this.AssertGetComponent(ref this.text);
            this.AssertGetComponentInParent(ref this.xrKeyboard);
        }
    }
}

#endif
