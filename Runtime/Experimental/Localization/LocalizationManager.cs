//-----------------------------------------------------------------------
// <copyright file="LocalizationManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public sealed class LocalizationManager : Manager<LocalizationManager>
    {
        public override void Initialize()
        {
            this.SetInstance(this);
        }
    }
}

#endif
