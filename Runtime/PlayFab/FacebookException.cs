//-----------------------------------------------------------------------
// <copyright file="FacebookException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlayFab
{
    using System;

    public class FacebookException : Exception
    {
        public FacebookException(string error)
        {
            this.Error = error;
        }

        public string Error { get; private set; }
    }
}
