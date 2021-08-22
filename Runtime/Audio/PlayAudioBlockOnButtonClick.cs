//-----------------------------------------------------------------------
// <copyright file="PlayAudioBlockOnButtonClick.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class PlayAudioBlockOnButtonClick : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private AudioBlock audioBlock;
        [SerializeField] private bool playSoundFromButtonPosition;

        [HideInInspector]
        [SerializeField] private Button button;
#pragma warning restore 0649

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.button);
        }

        private void Awake()
        {
            this.OnValidate();

            if (this.audioBlock == null)
            {
                Debug.LogError($"Button {this.gameObject.name} does not have an Audio Block!", this);
            }

            this.button.onClick.AddListener(this.OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (this.audioBlock == null)
            {
                return;
            }

            if (this.playSoundFromButtonPosition)
            {
                this.audioBlock.Play(this.button.transform.position);
            }
            else
            {
                this.audioBlock.Play();
            }
        }
    }
}

#endif
