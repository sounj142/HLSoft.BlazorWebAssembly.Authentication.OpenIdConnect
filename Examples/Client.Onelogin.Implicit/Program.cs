using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Client.Onelogin.Implicit
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
				.AddBlazoredOpenIdConnect(options =>
				{
					options.Authority = "https://hoang-luong-dev.onelogin.com/oidc/2/";

					options.ClientId = "9d38b830-45a3-0138-ebe5-020c7f144b4a165866";

					options.ResponseType = "token id_token";
					// due to CORS error on /token endpoint, we can't use authentication code flow on onelogin
					////options.ResponseType = "code";

					options.RevokeAccessTokenOnSignout = true;

					// onelogin use PUT method to logout => CORS error will happen, we just ignore logout step on onelogin.
					options.EndSessionEndpoint = "/onelogin-logout";

					options.PopupSignInRedirectUri = "/signin-popup-redirect";
					options.PopupSignOutRedirectUri = "/signout-popup-redirect";

					options.Scopes.Add("openid");
					options.Scopes.Add("profile");
				});
		}
	}
}