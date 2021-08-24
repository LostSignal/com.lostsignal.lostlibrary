//-----------------------------------------------------------------------
// <copyright file="UnityTaskTimeoutException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    public class UnityTaskTimeoutException : Exception
    {
        public UnityTaskTimeoutException()
               : base()
        {
        }

        public UnityTaskTimeoutException(string message)
            : base(message)
        {
        }

        public UnityTaskTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
