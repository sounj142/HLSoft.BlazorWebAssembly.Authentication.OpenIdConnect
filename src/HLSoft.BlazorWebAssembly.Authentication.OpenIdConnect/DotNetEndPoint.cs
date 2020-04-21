using Microsoft.JSInterop;
using System;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    public static class DotNetEndPoint
    {
        private static AuthenticationEventHandler _authenticationEventHandler;

        public static void Initialize(AuthenticationEventHandler authenticationEventHandler)
        {
            _authenticationEventHandler = authenticationEventHandler;
        }

        [JSInvokable]
        public static void NotifySilentRefreshTokenFail(Exception ex)
        {
            _authenticationEventHandler?.NotifySilentRefreshTokenFail(ex);
        }

        [JSInvokable]
        public static void NotifySilentRefreshTokenSuccess()
        {
            _authenticationEventHandler?.NotifySilentRefreshTokenSuccess();
        }
    }
}
