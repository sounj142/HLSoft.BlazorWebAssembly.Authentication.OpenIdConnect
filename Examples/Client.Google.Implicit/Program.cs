using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Blazor.Hosting;
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

			await builder.Build().RunAsync();
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthorizationCore(options => { })
				.AddBlazoredOpenIdConnect(options =>
				{
					options.Authority = "https://accounts.google.com/";

					options.ClientId = "802236688604-pj1blf39tv42cn02c7bnajdinaf6f9p3.apps.googleusercontent.com";
					options.ResponseType = "token id_token";

					// There are some crazy probem with google authentication code flow
					// 1. It requires ClientSecret, so foolhardy!
					// 2. Don't work if you try to login using popup.
					// => use implicit flow above, don't use this flow
					////options.ResponseType = "code";
					////options.ClientSecret = "gA-yaUJT7PWTEzpog_NfOp44";

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