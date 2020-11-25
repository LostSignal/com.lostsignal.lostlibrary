//-----------------------------------------------------------------------
// <copyright file="LogInDialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using System.Collections;
    using global::PlayFab.ClientModels;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LogInDialog : DialogLogic
    {
#pragma warning disable 0649
        [SerializeField] private LostButton closeButton;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private Toggle autoLoginToggle;
        [SerializeField] private LostButton logInButton;
        [SerializeField] private LostButton forgotPasswordButton;
        [SerializeField] private LostButton createNewAccountButton;
        [SerializeField] private string forgotEmailTemplateId;
#pragma warning restore 0649

        private GetPlayerCombinedInfoRequestParams infoRequestParams;
        private LoginManager loginManager;
        private bool isLeaveGameCoroutineRunning;
        private bool isLoginCoroutineRunning;
        private bool isForgotPasswordCoroutineRunning;

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
            this.AssertNotNull(this.autoLoginToggle, nameof(this.autoLoginToggle));
            this.AssertNotNull(this.logInButton, nameof(this.logInButton));
            this.AssertNotNull(this.forgotPasswordButton, nameof(this.forgotPasswordButton));
            this.AssertNotNull(this.createNewAccountButton, nameof(this.createNewAccountButton));
        }

        protected override void Awake()
        {
            base.Awake();
            this.OnValidate();

            this.closeButton.onClick.AddListener(this.OnBackButtonPressed);
            this.emailInputField.onValueChanged.AddListener(this.UpdateLogInButton);
            this.passwordInputField.onValueChanged.AddListener(this.UpdateLogInButton);
            this.forgotPasswordButton.onClick.AddListener(this.ForgotPassword);
            this.createNewAccountButton.onClick.AddListener(this.ShowSignUpDialog);
            this.logInButton.interactable = false;
            this.logInButton.onClick.AddListener(this.LogIn);

            PlayFabManager.OnInitialized += this.OnPlayFabManagerInitialized;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PlayFabManager.OnInitialized -= this.OnPlayFabManagerInitialized;
        }

        private void OnPlayFabManagerInitialized()
        {
            this.emailInputField.text = PlayFabManager.Instance.Login.LastLoginEmail;
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

        private void UpdateLogInButton(string newValue)
        {
            var email = this.emailInputField.text;
            var password = this.passwordInputField.text;

            this.logInButton.interactable =
                email.IsNullOrWhitespace() == false &&
                password.IsNullOrWhitespace() == false &&
                password.Length >= 6 &&
                password.Length <= 100;
        }

        private void ShowSignUpDialog()
        {
            this.Dialog.Hide();

            this.ExecuteDelayed(0.25f, () =>
            {
                DialogManager.GetDialog<SignUpDialog>().Show(this.loginManager, this.infoRequestParams, this.emailInputField.text);
            });
        }

        private void LogIn()
        {
            CoroutineRunner.Instance.StartCoroutine(LogInCoroutine());

            IEnumerator LogInCoroutine()
            {
                if (this.isLoginCoroutineRunning)
                {
                    yield break;
                }

                this.isLoginCoroutineRunning = true;

                var login = this.loginManager.LoginWithEmailAddress(this.emailInputField.text, this.passwordInputField.text, this.infoRequestParams);

                yield return login;

                if (login.HasError)
                {
                    yield return PlayFabMessages.HandleError(login.Exception);
                }
                else
                {
                    this.loginManager.LastLoginEmail = this.emailInputField.text;
                    this.loginManager.AutoLoginWithDeviceId = this.autoLoginToggle.isOn;
                    string customId = this.loginManager.GetEmailCustomId(this.emailInputField.text);

                    if (this.loginManager.AutoLoginWithDeviceId && this.loginManager.IsCustomIdLinked(customId) == false)
                    {
                        this.loginManager.LinkDeviceId(this.loginManager.DeviceId);
                    }

                    this.Dialog.Hide();
                }

                this.isLoginCoroutineRunning = false;
            }
        }

        private void ForgotPassword()
        {
            CoroutineRunner.Instance.StartCoroutine(ForgotPasswordCoroutine());

            IEnumerator ForgotPasswordCoroutine()
            {
                if (this.isForgotPasswordCoroutineRunning)
                {
                    yield break;
                }

                this.isForgotPasswordCoroutineRunning = true;

                this.Dialog.Hide();
                yield return WaitForUtil.Seconds(.25f);

                var forgot = PlayFabMessages.ShowForgotPasswordPrompt(this.emailInputField.text);

                if (forgot.Value == YesNoResult.Yes)
                {
                    var accountRecovery = loginManager.SendAccountRecoveryEmail(this.emailInputField.text, this.forgotEmailTemplateId);

                    yield return accountRecovery;

                    if (accountRecovery.HasError)
                    {
                        yield return PlayFabMessages.HandleError(accountRecovery.Exception);
                    }
                }

                this.Dialog.Show();

                this.isForgotPasswordCoroutineRunning = false;
            }
        }
    }
}
