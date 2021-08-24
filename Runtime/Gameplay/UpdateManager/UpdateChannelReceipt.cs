//-----------------------------------------------------------------------
// <copyright file="UpdateChannelReceipt.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;

    public struct UpdateChannelReceipt
    {
        private Action<int> cancelAction;
        private int id;

        public static UpdateChannelReceipt New(int id, Action<int> cancelAction)
        {
            return new UpdateChannelReceipt { id = id, cancelAction = cancelAction };
        }

        public void Cancel()
        {
            if (this.id > 0 && this.cancelAction != null)
            {
                this.cancelAction?.Invoke(this.id);
            }

            this.id = -1;
            this.cancelAction = null;
        }
    }
}

#endif
