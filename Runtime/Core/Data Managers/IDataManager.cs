//-----------------------------------------------------------------------
// <copyright file="IDataManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public interface IDataManager
    {
        System.Action DataUpdated { get; }

        DataStore DataStore { get; }

        bool IsDirty { get; }

        void Save();
    }
}
