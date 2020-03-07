using System.Security.Principal;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public interface IClaimsParser<TUser> where TUser: class
    {
        IIdentity CreateIdentity(TUser userClaims);
    }
}
