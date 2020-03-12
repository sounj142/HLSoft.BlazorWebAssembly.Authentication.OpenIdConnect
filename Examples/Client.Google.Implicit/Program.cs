using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Client.Google.Implicit
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
					options.Authority = "https://accounts.google.com/";

					options.ClientId = "802236688604-pj1blf39tv42cn02c7bnajdinaf6f9p3.apps.googleusercontent.com";

					options.ResponseType = "token id_token";
					// google don't fully support authentication code flow and required client secret, so don't use this flow. Use implicit flow.
					////options.ResponseType = "code";
					////options.ClientSecret = "<client secret>";

					options.WriteErrorToConsole = true;
					options.RevokeAccessTokenOnSignout = true;

					options.EndSessionEndpoint = "/google-logout";

					options.PopupSignInRedirectUri = "/signin-popup-redirect";
					options.PopupSignOutRedirectUri = "/signout-popup-redirect";

					options.Scope.Add("openid");
					options.Scope.Add("profile");
				});
		}
	}
}