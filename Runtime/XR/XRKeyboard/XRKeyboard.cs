//-----------------------------------------------------------------------
// <copyright file="XRKeyboard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class XRKeyboard : DialogLogic
    {
#pragma warning disable 0649
        [SerializeField] private XRKeyboardData keyboardData;

        [Header("Layouts")]
        [SerializeField] private Transform lowerCaseLayout;
        [SerializeField] private Transform upperCaseLayout;
        [SerializeField] private Transform numbersLayout;
        [SerializeField] private Transform symbolsLayout;

        [Header("Rows")]
        [SerializeField] private Transform[] lowerCaseRows;
        [SerializeField] private Transform[] upperCaseRows;
        [SerializeField] private Transform[] numbersRows;
        [SerializeField] private Transform[] symbolsRows;
        [SerializeField] private Transform[] keypadRows;
#pragma warning restore 0649

        private int currentSelectedGameObjectInstanceId = int.MinValue;
        private TMP_InputField currentInputField;

        public bool IsUpperCase { get; private set; }

        public System.Action<string> KeyPressed;

        public void KeyboardKeyPressed(string key)
        {
            this.InternalKeyPressed(key);
        }

        protected override void Awake()
        {
            base.Awake();

#if USING_UNITY_INPUT_SYSTEM
            if (Application.isPlaying)
            {
                Keyboard.current.onTextInput += this.InternalKeyPressed;
            }
#endif
        }

#if !USING_UNITY_INPUT_SYSTEM
        private void Update()
        {
            string inputString = UnityEngine.Input.inputString;

            if (inputString != null)
            {
                for (int i = 0; i < inputString.Length; i++)
                {
                    this.InternalKeyPressed(inputString[i]);
                }
            }
        }

#endif

        private void InternalKeyPressed(char c)
        {
            this.InternalKeyPressed(c.ToString());
        }

        private void InternalKeyPressed(string key)
        {
            this.KeyPressed?.Invoke(key);

            // Sending this key to the currently select InputField
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            int instanceId = selected != null ? selected.GetInstanceID() : int.MinValue;

            if (instanceId != this.currentSelectedGameObjectInstanceId)
            {
                this.currentSelectedGameObjectInstanceId = instanceId;
                this.currentInputField = selected.GetComponent<TMP_InputField>();
            }

            if (this.currentInputField != null)
            {
                // TODO [bgish]: Process key with this object
            }
        }
    }
}
