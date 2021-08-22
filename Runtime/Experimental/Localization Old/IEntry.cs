//-----------------------------------------------------------------------
// <copyright file="IEntry.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;

    public interface IEntry
    {
        ushort Id { get; }

        string Text { get; set; }

        DateTime LastEdited { get; }
    }
}

#endif
