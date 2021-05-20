//-----------------------------------------------------------------------
// <copyright file="AuthCodeLoginRequest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions.Login
{
    public class AuthCodeLoginRequest
    {
        public string Email { get; set; }

        public string AuthCode { get; set; }
    }
}
