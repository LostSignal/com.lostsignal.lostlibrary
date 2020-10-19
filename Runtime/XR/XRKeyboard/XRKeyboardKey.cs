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
        [SerializeField] private bool inferFromName = true;
        [SerializeField] private string lowerCase;
        [SerializeField] private string upperCase;
        [SerializeField] private TMP_Text text;

        [SerializeField][HideInInspector] private Button button;
        [SerializeField][HideInInspector] private XRKeyboard xrKeyboard;
#pragma warning restore 0649

        private void OnValidate()
        {
            if (this.inferFromName && Application.isPlaying == false)
            {
                this.lowerCase = this.name.ToLower()[0].ToString();
                this.upperCase = this.name.ToUpper()[0].ToString();
                this.text.text = this.lowerCase.ToString();
            }

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
            this.xrKeyboard.KeyPressed(this.xrKeyboard.IsUpperCase ? this.upperCase : this.lowerCase);
        }
    }
}
