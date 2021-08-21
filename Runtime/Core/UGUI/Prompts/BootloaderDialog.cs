#pragma warning disable

//--------------------------------------------------------------------s---
// <copyright file="BootloaderDialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class BootloaderDialog : DialogLogic
    {
        #pragma warning disable 0649
        [SerializeField] private AspectRatioFitter aspectRatioFitter;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Image progressBar;
        #pragma warning disable 0649

        protected override void OnShow()
        {
            base.OnShow();

            Bootloader.ProgressTextUpdate += this.OnProgressTextUpdate;
            Bootloader.ProgressUpdated += this.OnProgressUpdated;
            this.ExecuteDelayed(0.5f, this.DisableAspectRatioFitter);
        }

        protected override void OnHide()
        {
            base.OnHide();

            Bootloader.ProgressTextUpdate -= this.OnProgressTextUpdate;
            Bootloader.ProgressUpdated -= this.OnProgressUpdated;
        }

        private void OnProgressTextUpdate(string newText)
        {
            if (this.progressText != null)
            {
                this.progressText.text = newText;
            }
        }

        private void OnProgressUpdated(float newProgress)
        {
            if (this.progressBar != null)
            {
                this.progressBar.fillAmount = newProgress;
            }
        }

        private void DisableAspectRatioFitter()
        {
            if (this.aspectRatioFitter != null && Application.isEditor == false)
            {
                this.aspectRatioFitter.enabled = false;
            }
        }
    }
}

#endif
