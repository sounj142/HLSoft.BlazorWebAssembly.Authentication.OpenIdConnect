using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
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

					options.EndSessionEndpoint = "/oauth0-logout";
					options.EndSessionEndpointProcess = async (provider) =>
					{
						var logoutUrl = $"{options.Authority}/v2/logout?client_id={options.ClientId}";
						var client = provider.GetService<HttpClient>();
						await client.PostJsonAsync(logoutUrl, new object());
					};

					options.PopupSignInRedirectUri = "/signin-popup-redirect";
					options.PopupSignOutRedirectUri = "/signout-popup-redirect";

					options.Scope.Add("openid");
					options.Scope.Add("profile");
					options.Scope.Add("api");
				});
		}
	}
}