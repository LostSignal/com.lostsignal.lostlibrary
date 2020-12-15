//-----------------------------------------------------------------------
// <copyright file="InputFieldTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;
    using UnityEngine.UI;

    public static class InputFieldTracker
    {
        private static int currentSelectedGameObjectInstanceId = int.MinValue;

        private static InputField currentInputField;
        private static TMP_InputField currentTMPInputField;

        public static InputField GetCurrentInputField()
        {
            Update();
            return currentInputField;
        }

        public static TMP_InputField GetCurrentTMPInputField()
        {
            Update();
            return currentTMPInputField;
        }

        private static void Update()
        {
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (selected)
            {
                var instanceId = selected.GetInstanceID();

                if (instanceId != currentSelectedGameObjectInstanceId)
                {
                    currentSelectedGameObjectInstanceId = instanceId;
                    currentInputField = selected.GetComponent<InputField>();
                    currentTMPInputField = selected.GetComponent<TMP_InputField>();
                }
            }
            else
            {
                currentSelectedGameObjectInstanceId = int.MinValue;
                currentInputField = null;
                currentTMPInputField = null;
            }
        }
    }
}
