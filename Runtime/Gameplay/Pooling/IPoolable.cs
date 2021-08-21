#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="IPoolable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public interface IPoolable
    {
        void Recycle();
    }
}

#endif
