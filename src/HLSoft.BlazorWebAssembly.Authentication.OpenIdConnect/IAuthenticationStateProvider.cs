using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public interface IAuthenticationStateProvider
	{

		/// <summary>
		/// An event that provides notification when the Microsoft.AspNetCore.Components.Authorization.AuthenticationState
		/// has changed. For example, this event may be raised if a user logs in or out.
		/// </summary>
		event AuthenticationStateChangedHandler AuthenticationStateChanged;

		/// <summary>
		/// Asynchronously gets an Microsoft.AspNetCore.Components.Authorization.AuthenticationState that describes the current user.
		/// </summary>
		/// <returns> 
		/// A task that, when resolved, gives an Microsoft.AspNetCore.Components.Authorization.AuthenticationState 
		/// instance that describes the current user.
		/// </returns>
		Task<AuthenticationState> GetAuthenticationStateAsync();

		/// <summary>
		/// Raises the Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider.AuthenticationStateChanged event.
		/// </summary>
		void NotifyAuthenticationStateChanged();

		/// <summary>
		/// set the "Bearer" header for httpClient
		/// </summary>
		Task SetAuthorizationHeader(HttpClient httpClient, string tokenName = "access_token");
	}
}
