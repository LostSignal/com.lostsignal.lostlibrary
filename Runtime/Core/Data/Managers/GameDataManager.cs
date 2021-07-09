//-----------------------------------------------------------------------
// <copyright file="GameDataManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public sealed class GameDataManager : DataManager<GameDataManager>
    {
        public override string Name => nameof(GameDataManager);

        public override void Initialize()
        {
            this.InitializeDataStroreFromPlayerPrefs();
            this.SetInstance(this);
        }
    }
}
