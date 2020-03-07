using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public interface IAuthenticationStateProvider
	{
		event AuthenticationStateChangedHandler AuthenticationStateChanged;
		Task<AuthenticationState> GetAuthenticationStateAsync();
		void NotifyAuthenticationStateChanged();
		Task<HttpClient> GetHttpClientAsync(string tokenName = "access_token");
	}
}
