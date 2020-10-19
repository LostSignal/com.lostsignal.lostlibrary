//-----------------------------------------------------------------------
// <copyright file="XRKeyboard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class XRKeyboard : MonoBehaviour
    {
        public bool IsUpperCase { get; private set; }

        public System.Action<string> KeyPressed;

        public void KeyboardKeyPressed(string key)
        {
            this.InternalKeyPressed(key);
        }

        private void Update()
        {
            foreach (char c in UnityEngine.Input.inputString)
            {
                this.InternalKeyPressed(c);
            }
        }

        private void InternalKeyPressed(char c)
        {
            this.InternalKeyPressed(c.ToString());
        }

        private void InternalKeyPressed(string key)
        {
            this.KeyPressed?.Invoke(key);
        }
    }
}
