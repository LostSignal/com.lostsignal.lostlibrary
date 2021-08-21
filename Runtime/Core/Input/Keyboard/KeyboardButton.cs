#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="KeyboardButton.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using UnityEngine;

    public enum ButtonState
    {
        Idle,
        Pressed,
        Held,
        Released
    }

    public class KeyboardButton
    {
        private double pressedTime;
        private double releasedTime;

        public ButtonState State
        {
            get; private set;
        }

        public bool IsDown
        {
            get { return this.State == ButtonState.Held || this.State == ButtonState.Pressed; }
        }

        public bool IsUp
        {
            get { return this.State == ButtonState.Idle || this.State == ButtonState.Released; }
        }

        public bool IsPressed
        {
            get { return this.State == ButtonState.Pressed; }
        }

        public double PressedTime
        {
            get
            {
                if (this.State == ButtonState.Idle)
                {
                    return 0;
                }
                else if (this.State == ButtonState.Held || this.State == ButtonState.Pressed)
                {
                    return Time.realtimeSinceStartupAsDouble - this.pressedTime;
                }
                else if (this.State == ButtonState.Released)
                {
                    return Time.realtimeSinceStartupAsDouble - this.pressedTime;
                }
                else
                {
                    throw new NotImplementedException("Found unknown ButtonState!");
                }
            }
        }

        public void Update(bool isPressed)
        {
            if (this.State == ButtonState.Idle)
            {
                if (isPressed)
                {
                    this.State = ButtonState.Pressed;
                    this.pressedTime = Time.realtimeSinceStartupAsDouble;
                }
            }
            else if (this.State == ButtonState.Pressed)
            {
                if (isPressed)
                {
                    this.State = ButtonState.Held;
                }
                else
                {
                    this.State = ButtonState.Released;
                    this.releasedTime = Time.realtimeSinceStartupAsDouble;
                }
            }
            else if (this.State == ButtonState.Held)
            {
                if (isPressed == false)
                {
                    this.State = ButtonState.Released;
                    this.releasedTime = Time.realtimeSinceStartupAsDouble;
                }
            }
            else if (this.State == ButtonState.Released)
            {
                if (isPressed)
                {
                    this.State = ButtonState.Pressed;
                    this.pressedTime = Time.realtimeSinceStartupAsDouble;
                }
                else
                {
                    this.State = ButtonState.Idle;
                }
            }
        }
    }
}

#endif
