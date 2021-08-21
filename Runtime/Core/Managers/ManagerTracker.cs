#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ManagerTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    // NOTE [bgish]: Not a huge fan on how I'm tracking managers, but not 100% sure the right route to go
    public static class ManagerTracker
    {
        public static readonly ObjectTracker<IManager> Managers = new ObjectTracker<IManager>(50);
    }
}

#endif
