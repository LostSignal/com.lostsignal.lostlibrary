#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="AudioBlockExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public static class AudioBlockExtensions
    {
        public static void PlayIfNotNull(this AudioBlock audioBlock)
        {
            if (audioBlock)
            {
                audioBlock.Play();
            }
        }
    }
}

#endif
