//-----------------------------------------------------------------------
// <copyright file="ManagerTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    
    public static class ManagerTracker
    {
        static ManagerTracker()
        {
            Bootloader.OnReset += ClearManagers;

            managers = new List<IManager>();
            Managers = new ReadOnlyCollection<IManager>(managers);
        }

        private static List<IManager> managers = new List<IManager>();

        public static ReadOnlyCollection<IManager> Managers { get; private set; }

        public static void AddManager(IManager manager)
        {
            managers.Add(manager);
        }

        public static void RemoveManager(IManager manager)
        {
            managers.Remove(manager);
        }

        private static void ClearManagers()
        {
            managers.Clear();
        }
    }
}

#endif
