//-----------------------------------------------------------------------
// <copyright file="TitleNewsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.PlayFab
{
    public class TitleNewsManager
    {
        private PlayFabManager playfabManager;

        public TitleNewsManager(PlayFabManager playfabManager)
        {
            this.playfabManager = playfabManager;
        }
    }
}

#endif
