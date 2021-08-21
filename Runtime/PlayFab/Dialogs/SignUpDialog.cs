﻿#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="SignUpDialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.PlayFab
{
    using System.Collections;
    using global::PlayFab.ClientModels;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SignUpDialog : DialogLogic
    {
#pragma warning disable 0649
        [SerializeField] private LostButton closeButton;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField confirmPasswordInputField;
        [SerializeField] private Toggle autoLoginToggle;
        [SerializeField] private LostButton alreadRegistedButton;
        [SerializeField] private LostButton signUpButton;
#pragma warning restore 0649

        private LoginManager loginManager;
        private GetPlayerCombinedInfoRequestParams infoRequestParams;
        private bool isLeaveGameCoroutineRunning;
        private bool isSignUpCoroutineRunning;

        public void Show(LoginManager loginManager, GetPlayerCombinedInfoRequestParams infoRequestParams = null, string email = "")
        {
            this.loginManager = loginManager;
            this.infoRequestParams = infoRequestParams;
            this.emailInputField.text = email;
            this.Dialog.Show();
        }

        private void OnValidate()
        {
            this.AssertNotNull(this.closeButton, nameof(this.closeButton));
            this.AssertNotNull(this.emailInputField, nameof(this.emailInputField));
            this.AssertNotNull(this.passwordInputField, nameof(this.passwordInputField));
            this.AssertNotNull(this.confirmPasswordInputField, nameof(this.confirmPasswordInputField));
            this.AssertNotNull(this.autoLoginToggle, nameof(this.autoLoginToggle));
            this.AssertNotNull(this.alreadRegistedButton, nameof(this.alreadRegistedButton));
            this.AssertNotNull(this.signUpButton, nameof(this.signUpButton));
        }

        protected override void Awake()
        {
            base.Awake();
            this.OnValidate();

            this.closeButton.onClick.AddListener(this.OnBackButtonPressed);

            this.emailInputField.onValueChanged.AddListener(this.UpdateSignUpButton);
            this.passwordInputField.onValueChanged.AddListener(this.UpdateSignUpButton);
            this.confirmPasswordInputField.onValueChanged.AddListener(this.UpdateSignUpButton);
            this.alreadRegistedButton.onClick.AddListener(this.ShowLogInDialog);
            this.signUpButton.interactable = false;
            this.signUpButton.onClick.AddListener(this.SignIn);

            PlayFab.PlayFabManager.OnInitialized += this.OnPlayFabManagerInitialized;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PlayFab.PlayFabManager.OnInitialized -= this.OnPlayFabManagerInitialized;
        }

        private void OnPlayFabManagerInitialized()
        {
            this.autoLoginToggle.isOn = PlayFabManager.Instance.Login.HasEverLoggedIn == false || PlayFabManager.Instance.Login.AutoLoginWithDeviceId;
        }

        protected override void OnBackButtonPressed()
        {
            CoroutineRunner.Instance.StartCoroutine(LeaveGameCoroutine());

            IEnumerator LeaveGameCoroutine()
            {
                if (this.isLeaveGameCoroutineRunning)
                {
                    yield break;
                }

                this.isLeaveGameCoroutineRunning = true;

                this.Dialog.Hide();

                yield return WaitForUtil.Seconds(0.25f);

                var leaveGamePrompt = PlayFabMessages.ShowExitAppPrompt();

                yield return leaveGamePrompt;

                if (leaveGamePrompt.Value == YesNoResult.Yes)
                {
                    Platform.QuitApplication();
                    yield break;
                }

                this.Dialog.Show();

                this.isLeaveGameCoroutineRunning = false;
            }
        }

        private void UpdateSignUpButton(string newValue)
        {
            var email = this.emailInputField.text;
            var password = this.passwordInputField.text;
            var confirmPassword = this.confirmPasswordInputField.text;

            this.signUpButton.interactable =
                email.IsNullOrWhitespace() == false &&
                password.IsNullOrWhitespace() == false &&
                password.Length >= 6 &&
                password.Length <= 100 &&
                password == confirmPassword;
        }

        private void SignIn()
        {
            CoroutineRunner.Instance.StartCoroutine(SignInCoroutine());

            IEnumerator SignInCoroutine()
            {
                if (this.isSignUpCoroutineRunning)
                {
                    yield break;
                }

                this.isSignUpCoroutineRunning = true;

                var register = this.loginManager.RegisterPlayFabUser(this.emailInputField.text, this.passwordInputField.text);

                yield return register;

                if (register.HasError)
                {
                    yield return PlayFabMessages.HandleError(register.Exception);
                }
                else
                {
                    var login = this.loginManager.LoginWithEmailAddress(this.emailInputField.text, this.passwordInputField.text);

                    yield return login;

                    if (login.HasError)
                    {
                        yield return PlayFabMessages.HandleError(login.Exception);

                        // TODO [bgish]: If email already take, then ask them to retry and continue showing the SignUp dialog
                    }
                    else
                    {
                        this.loginManager.HasEverLoggedIn = true;
                        this.loginManager.LastLoginEmail = this.emailInputField.text;
                        this.loginManager.AutoLoginWithDeviceId = this.autoLoginToggle.isOn;

                        if (this.loginManager.AutoLoginWithDeviceId)
                        {
                            var linkDevice = this.loginManager.LinkDeviceId(this.loginManager.GetEmailCustomId(this.emailInputField.text));

                            yield return linkDevice;

                            if (linkDevice.HasError)
                            {
                                yield return PlayFabMessages.HandleError(linkDevice.Exception);
                            }
                        }
                    }

                    //// TODO [bgish]: What Do I Do If the Linking is not successful!?!?!?!?! It will fail
                    ////               if someone logs out and tries to create another account.

                    this.Dialog.Hide();
                }

                this.isSignUpCoroutineRunning = false;
            }
        }

        private void ShowLogInDialog()
        {
            this.Dialog.Hide();

            this.ExecuteDelayed(0.25f, () =>
            {
                DialogManager.GetDialog<LogInDialog>().Show(this.loginManager, this.infoRequestParams, this.emailInputField.text);
            });
        }
    }
}

#endif
