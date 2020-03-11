using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Client.Azure.Implicit
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
					options.Authority = "https://login.microsoftonline.com/hlsoftblazortest.onmicrosoft.com/v2.0/";

					options.ClientId = "d9793137-c826-4456-98af-771afee326ee";

					options.ResponseType = "token id_token";

					// due to CORS error on /token endpoint, we can't use authentication code flow on Azure
					// https://stackoverflow.com/questions/52839055/enabling-cors-on-azure-active-directory
					////options.ResponseType = "code";

					//options.WriteErrorToConsole = true;
					options.RevokeAccessTokenOnSignout = true;


					options.PopupSignInRedirectUri = "/signin-popup-redirect";
					options.PopupSignOutRedirectUri = "/signout-popup-redirect";

					options.Scope.Add("openid");
					options.Scope.Add("profile");
				});
		}
	}
}