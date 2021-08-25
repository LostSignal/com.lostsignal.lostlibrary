//-----------------------------------------------------------------------
// <copyright file="RealtimeMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || USING_PLAYFAB

namespace Lost
{
    public abstract class RealtimeMessage
    {
        public abstract string Type { get; }
    }
}

#endif
