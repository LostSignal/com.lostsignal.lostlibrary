//-----------------------------------------------------------------------
// <copyright file="PlayerDataManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public sealed class PlayerDataManager : DataManager<PlayerDataManager>
    {
        public override string Name => nameof(PlayerDataManager);

        public override void Initialize()
        {
            this.InitializeDataStroreFromPlayerPrefs();
            this.SetInstance(this);
        }
    }
}
