//-----------------------------------------------------------------------
// <copyright file="InputFieldTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;

    public static class InputFieldTracker
    {
        private static int currentSelectedGameObjectInstanceId = int.MinValue;
        private static TMP_InputField currentInputField;

        public static TMP_InputField GetCurrentInputField()
        {
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            int instanceId = selected != null ? selected.GetInstanceID() : int.MinValue;

            if (instanceId != currentSelectedGameObjectInstanceId)
            {
                currentSelectedGameObjectInstanceId = instanceId;
                currentInputField = selected.GetComponent<TMP_InputField>();
            }

            return currentInputField;
        }
    }
}
