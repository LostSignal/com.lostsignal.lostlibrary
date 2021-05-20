//-----------------------------------------------------------------------
// <copyright file="LocalizationManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class LocalizationManager : Manager<LocalizationManager>
    {
        public override void Initialize()
        {
            this.SetInstance(this);
        }
    }
}
