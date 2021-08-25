// <auto-generated/>
#pragma warning disable

#if USING_PLAYFAB

namespace Lost.CloudFunctions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lost.CloudFunctions.Common;
    using Lost.CloudFunctions.Debug;
    using Lost.CloudFunctions.Login;

    public static class LoginCloudFunctionsExtensions
    {
        public static Task<Result> Login_RequestLogin(this CloudFunctionsManager cloudFunctionsManager, string request) => cloudFunctionsManager.Execute("Login_RequestLogin", request);

        public static Task<ResultT<AuthCodeLoginResult>> Login_AuthCodeLogin(this CloudFunctionsManager cloudFunctionsManager, AuthCodeLoginRequest request) => cloudFunctionsManager.Execute<AuthCodeLoginResult>("Login_AuthCodeLogin", request);
    }
}

#endif
