#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="DeviceDataManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public sealed class DeviceDataManager : DataManager<DeviceDataManager>
    {
        public override string Name => nameof(DeviceDataManager);

        public override void Initialize()
        {
            this.InitializeDataStroreFromPlayerPrefs();
            this.SetInstance(this);
        }
    }
}

#endif
