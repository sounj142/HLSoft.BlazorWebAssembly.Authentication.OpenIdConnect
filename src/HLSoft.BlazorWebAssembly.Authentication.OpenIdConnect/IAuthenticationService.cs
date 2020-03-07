using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public interface IAuthenticationService
	{
		Task SignInAsync();
		Task SignOutAsync();
		Task SignInPopupAsync();
		Task SignOutPopupAsync();
		Task RequireAuthenticationAsync();
		/// <summary>
		/// return true if current URI is one of special URIs for sign-in, sign-out
		/// </summary>
		/// <returns></returns>
		bool CurrentUriIsAuthenticationUri();
	}
}
