using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public interface IAuthenticationService
	{
		/// <summary>
		/// Request to start the redirect login process, will trigger a redirect of the current window to the authorization endpoint
		/// </summary>
		Task SignInAsync();
		/// <summary>
		/// Request to start the redirect logout process, will trigger a redirect of the current window to the end session endpoint.
		/// </summary>
		Task SignOutAsync();
		/// <summary>
		/// Request to start the popup login process, will show s popup to do the authorization
		/// </summary>
		Task SignInPopupAsync();
		/// <summary>
		/// Request to start the popup logout process, will show s popup to logout
		/// </summary>
		Task SignOutPopupAsync();
		/// <summary>
		/// Usually use in the scenario when we need to enforce the user login before he can access any resources in website.
		/// At the first time user enters website, he will be redirected to authorization endpoint
		/// </summary>
		/// <returns></returns>
		Task RequireAuthenticationAsync();
		/// <summary>
		/// return true if current URI is one of special URIs for sign-in/sign-out features
		/// </summary>
		/// <returns></returns>
		bool CurrentUriIsAuthenticationUri();
	}
}
