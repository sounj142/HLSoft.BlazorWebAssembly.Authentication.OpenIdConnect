using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
			builder.Services.AddBaseAddressHttpClient();
			await builder.Build().RunAsync();
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions()
				.AddAuthorizationCore()

				//.AddBlazoredOpenIdConnect(options =>
				//{
				//	options.Authority = "https://hoang-luong.auth0.com";

				//	options.ClientId = "mbjoV5gM7AcRpslDFQyc6Qs6GjXPyPWa";

				//	options.ResponseType = "code";
				//	//options.ResponseType = "token id_token";

				//	options.WriteErrorToConsole = true;
				//	options.RevokeAccessTokenOnSignout = false;

				//	// because Auth0 don't offer End Session Endpoint in its well-known documentary, we need to
				//	// implement a custom url to process this feature
				//	options.EndSessionEndpoint = "/oauth0-logout";
				//	options.EndSessionEndpointProcess = async provider =>
				//	{
				//		var config = provider.GetService<ClientOptions>();
				//		var logoutUrl = $"{config.authority}/v2/logout";
				//		logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "client_id", config.client_id);
				//		logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "returnTo", config.doNothingUri);
				//		var authenticationService = provider.GetService<IAuthenticationService>();
				//		await authenticationService.SilentOpenUrlInIframe(logoutUrl);
				//	};

				//	// request api resource
				//	//options.ExtraQueryParams = new
				//	//{
				//	//	audience = "weather_forecast"
				//	//};

				//	// Note: you need to add urls bellow and "/oidc-nothing" to the redirect urls configuration in Auth0
				//	options.PopupSignInRedirectUri = "/signin-popup-redirect";
				//	options.PopupSignOutRedirectUri = "/signout-popup-redirect";

				//	options.Scope.Add("openid");
				//	options.Scope.Add("profile");
				//});

				.AddBlazoredOpenIdConnect(provider =>
				{
					System.Console.WriteLine("==== Tao OpenIdConnectOptionsYy");

					var authorityUrl = "https://hoang-luong.auth0.com";
					var options = new OpenIdConnectOptions();
					options.Authority = authorityUrl;

					options.ClientId = "mbjoV5gM7AcRpslDFQyc6Qs6GjXPyPWa";

					options.ResponseType = "code";

					options.WriteErrorToConsole = true;
					options.RevokeAccessTokenOnSignout = false;


					var navigationManager = provider.GetService<NavigationManager>();
					var logoutRedirectUrl = navigationManager.GetAbsoluteUri(options.SignedOutRedirectUri);
					var logoutUrl = $"{authorityUrl}/v2/logout";
					logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "client_id", options.ClientId);
					logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "returnTo", logoutRedirectUrl);
					options.Metadata = new OidcMetadata
					{
						Issuer = $"{authorityUrl}/",
						AuthorizationEndpoint = $"{authorityUrl}/authorize?audience=weather_forecast",
						JwksUri = $"{authorityUrl}/.well-known/jwks.json",
						TokenEndpoint = $"{authorityUrl}/oauth/token",
						UserinfoEndpoint = $"{authorityUrl}/userinfo",
						EndSessionEndpoint = logoutUrl,
						RevocationEndpoint = $"{authorityUrl}/oauth/revoke",
					};

					// Note: you need to add urls bellow and "/oidc-nothing" to the redirect urls configuration in Auth0
					options.PopupSignInRedirectUri = "/signin-popup-redirect";
					options.PopupSignOutRedirectUri = "/signout-popup-redirect";

					options.Scope.Add("openid");
					options.Scope.Add("profile");
					return Task.FromResult(options);
				});
		}
	}
}