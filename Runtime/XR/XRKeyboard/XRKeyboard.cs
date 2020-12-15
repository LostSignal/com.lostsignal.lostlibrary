//-----------------------------------------------------------------------
// <copyright file="XRKeyboard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class XRKeyboard : DialogLogic
    {
#pragma warning disable 0649
        [SerializeField] private XRKeyboardData keyboardData;
        [SerializeField] private XRKeyboardKey keyPrefab;

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

        private bool isUpperCase;
        private bool isShowingLetters = true;
        private bool isShowingNumbers;
        private bool isShowingSymbols;

        public InputField CurrentInputField { get; set; }

        public TMP_InputField CurrentTMPInputField { get; set; }

        public XRKeyboardData.Keyboard CurrentKeyboard
        {
            get => this.keyboardData.CurrentKeyboard;
        }

        public System.Action<char, string> KeyPressed;

        protected override void Awake()
        {
            base.Awake();
            this.PopulateKeyboard();
            this.UpdateKeyboardVisuals();
        }

        private void OnKeyPressed(char keyChar, string keyString)
        {
            this.KeyPressed?.Invoke(keyChar, keyString);

            if (this.CurrentInputField)
            {
                this.CurrentInputField.ProcessEvent(Event.KeyboardEvent(keyString));
            }
            else if (this.CurrentTMPInputField)
            {
                this.CurrentTMPInputField.ProcessEvent(Event.KeyboardEvent(keyString));
            }
        }

        private void PopulateKeyboard()
        {
            var keyboard = this.CurrentKeyboard;

            Populate(nameof(this.lowerCaseRows), this.lowerCaseRows, keyboard?.LowerCaseText);
            Populate(nameof(this.upperCaseRows), this.upperCaseRows, keyboard?.UpperCaseText);
            Populate(nameof(this.numbersRows), this.numbersRows, keyboard?.NumbersText);
            Populate(nameof(this.symbolsRows), this.symbolsRows, keyboard?.SymbolsText);
            Populate(nameof(this.keypadRows), this.keypadRows, keyboard?.KeypadText);

            void Populate(string rowName, Transform[] rows, string text)
            {
                var lines = text.IsNullOrWhitespace() == false ? text.Split('\n') : null;

                if (rows.Length != lines.Length)
                {
                    Debug.LogError($"{rowName} row/teext doesn't match!");
                    return;
                }

                for (int i = 0; i < rows.Length; i++)
                {
                    var row = rows[i];
                    var line = lines[i];

                    row.DestroyAllChildren();

                    for (int j = 0; j < line.Length; j++)
                    {
                        var keyboardKey = GameObject.Instantiate(this.keyPrefab, row);
                        keyboardKey.SetData(this, line[j], line.Substring(j, 1), this.OnKeyPressed);
                    }
                }
            }
        }

        private void UpdateKeyboardVisuals()
        {
            this.lowerCaseLayout.SafeSetActive(this.isShowingLetters && this.isUpperCase == false);
            this.upperCaseLayout.SafeSetActive(this.isShowingLetters && this.isUpperCase);
            this.numbersLayout.SafeSetActive(this.isShowingNumbers);
            this.symbolsLayout.SafeSetActive(this.isShowingSymbols);
        }
    }
}
