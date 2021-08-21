#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="VoidEvent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public class VoidEvent
    {
        private List<Action> actions = new List<Action>();

        public void Subscribe(Action action)
        {
            this.actions.AddIfNotNullAndUnique(action);
        }

        public void Unsubscribe(Action action)
        {
            this.actions.Remove(action);
        }

        public void Raise()
        {
            for (int i = this.actions.Count - 1; i >= 0; i--)
            {
                this.actions[i].Invoke();
            }
        }
    }
}
