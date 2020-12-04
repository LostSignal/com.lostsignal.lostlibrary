//-----------------------------------------------------------------------
// <copyright file="DialogManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.PlayFab;
    using UnityEngine;

    public class DialogManager : Manager<DialogManager>
    {
        //// TODO [bgish]: Update GetDialog to look out for these types and create them on the fly if needed
        //// #pragma warning disable 0649
        //// [Header("Required Dialogs")]
        //// [SerializeField] private BootloaderDialog bootloaderDialog;
        //// [SerializeField] private MessageBox messageBox;
        //// [SerializeField] private DebugMenu debugMenu;
        //// [SerializeField] private PurchaseItem purchaseItem;
        //// [SerializeField] private StringInputBox stringInputBox;
        //// [SerializeField] private LogInDialog loginDialog;
        //// [SerializeField] private SignUpDialog signUpDialog;
        //// [SerializeField] private XRKeyboard xrKeyboard;
        //// TODO [bgish]: Add 2FA Request Login
        //// TODO [bgish]: Add 2FA Login
        //// #pragma warning restore 0649

        private Dictionary<System.Type, DialogLogic> dialogTypes = new Dictionary<System.Type, DialogLogic>();
        private LinkedList<Dialog> dialogs = new LinkedList<Dialog>();

        public static void RegisterDialog(DialogLogic dialogLogic)
        {
            if (DialogManager.IsInitialized)
            {
                DialogManager.Instance.dialogTypes.Add(dialogLogic.GetType(), dialogLogic);
            }
            else
            {
                DialogManager.OnInitialized += () =>
                {
                    DialogManager.Instance.dialogTypes.Add(dialogLogic.GetType(), dialogLogic);
                };
            }
        }

        public static void UnregisterDialog(DialogLogic dialogLogic)
        {
            if (DialogManager.Instance)
            {
                DialogManager.Instance.dialogTypes.Remove(dialogLogic.GetType());
            }
        }

        public static void UpdateAllDialogCameras()
        {
            foreach (var dialogLogic in DialogManager.Instance.dialogTypes.Values)
            {
                dialogLogic.Dialog.UpdateCamera();
            }
        }

        public static T GetDialog<T>() where T : DialogLogic
        {
            if (DialogManager.Instance.dialogTypes.TryGetValue(typeof(T), out DialogLogic dialogLogic))
            {
                return (T)dialogLogic;
            }

            return null;
        }

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        public bool IsTopMostDialog(Dialog dialog)
        {
            return this.dialogs.Last != null && this.dialogs.Last.Value == dialog;
        }

        public void AddDialog(Dialog dialog)
        {
            if (dialog != null && dialog.RegisterForBackButton && this.dialogs.Contains(dialog) == false)
            {
                this.dialogs.AddLast(dialog);
            }
        }

        public void RemoveDialog(Dialog dialog)
        {
            if (dialog != null && dialog.RegisterForBackButton && this.dialogs.Contains(dialog))
            {
                this.dialogs.Remove(dialog);
            }
        }

        public void BackButtonPressed()
        {
            if (this.dialogs.Count > 0)
            {
                this.dialogs.Last.Value.BackButtonPressed();
            }
        }

#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
        private void Update()
        {
            // NOTE [bgish]: this catches the Android Back Button
#if USING_UNITY_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
#else
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
#endif
            {
                this.BackButtonPressed();
            }
        }

#endif
    }
}
