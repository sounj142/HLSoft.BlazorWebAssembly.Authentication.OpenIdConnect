using System.Security.Principal;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    /// <summary>
    /// implement this interface if you want to parse the claims by yourself
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
	public interface IClaimsParser<TUser> where TUser: class
    {
        /// <summary>
        /// create IIdentity from clain data userClaims
        /// </summary>
        /// <param name="userClaims">claim data, get using [oidc-client-js].getUser() method</param>
        IIdentity CreateIdentity(TUser userClaims);
    }
}
