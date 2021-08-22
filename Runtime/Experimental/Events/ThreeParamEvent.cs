//-----------------------------------------------------------------------
// <copyright file="ThreeParamEvent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public class ThreeParamEvent<TParam1, TParam2, TParam3>
    {
        private List<Action<TParam1, TParam2, TParam3>> actions = new List<Action<TParam1, TParam2, TParam3>>();

        public void Subscribe(Action<TParam1, TParam2, TParam3> action)
        {
            this.actions.AddIfNotNullAndUnique(action);
        }

        public void Unsubscribe(Action<TParam1, TParam2, TParam3> action)
        {
            this.actions.Remove(action);
        }

        public void Raise(TParam1 eventObject1, TParam2 eventObject2, TParam3 eventObject3)
        {
            for (int i = this.actions.Count - 1; i >= 0; i--)
            {
                this.actions[i].Invoke(eventObject1, eventObject2, eventObject3);
            }
        }
    }
}
