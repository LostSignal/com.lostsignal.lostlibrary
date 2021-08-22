//-----------------------------------------------------------------------
// <copyright file="LostConn.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE

namespace Lost.DissonanceIntegration
{
    using System;

    public struct LostConn : IEquatable<LostConn>
    {
        public long PlayerId;

        public bool Equals(LostConn other)
        {
            throw new NotImplementedException();
        }
    }
}

#endif
