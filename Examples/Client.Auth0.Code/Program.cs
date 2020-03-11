using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Client.Auth0.Code
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			ConfigureServices(builder.Services);

			builder.RootComponents.Add<App>("app");

			await builder.Build().RunAsync();
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthorizationCore(options => { })
				.AddBlazoredOpenIdConnect(options =>
				{
					options.Authority = "https://hoang-luong.auth0.com";

					options.ClientId = "mbjoV5gM7AcRpslDFQyc6Qs6GjXPyPWa";

					options.ResponseType = "code";
					//options.ResponseType = "token id_token";

					options.WriteErrorToConsole = true;
					options.RevokeAccessTokenOnSignout = false;

					// because Auth0 don't offer End Session Endpoint in its well-known documentary, we need to
					// implement a custom url to process this feature
					options.EndSessionEndpoint = "/oauth0-logout";
					options.EndSessionEndpointProcess = async provider =>
					{
						var config = provider.GetService<ClientOptions>();
						var logoutUrl = $"{config.authority}/v2/logout";
						logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "client_id", config.client_id);
						logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "returnTo", config.doNothingUri);
						var authenticationService = provider.GetService<IAuthenticationService>();
						await authenticationService.SilentOpenUrlInIframe(logoutUrl);
					};

					// Note: you need to add urls bellow and "/oidc-nothing" to the redirect urls configuration in Auth0
					options.PopupSignInRedirectUri = "/signin-popup-redirect";
					options.PopupSignOutRedirectUri = "/signout-popup-redirect";

					options.Scope.Add("openid");
					options.Scope.Add("profile");
					//options.Scope.Add("api");
				});
		}
	}
}