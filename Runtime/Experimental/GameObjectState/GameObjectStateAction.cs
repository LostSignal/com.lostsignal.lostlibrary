//-----------------------------------------------------------------------
// <copyright file="GameObjectStateAction.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;

    [Serializable]
    public abstract class GameObjectStateAction
    {
        public virtual string EditorDisplayName => this.GetType().Name;

        public abstract void Apply();

        public abstract void Revert();
    }
}

#endif
