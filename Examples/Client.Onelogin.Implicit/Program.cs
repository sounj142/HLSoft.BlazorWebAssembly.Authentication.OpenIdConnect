using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Blazor.Hosting;
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

			await builder.Build().RunAsync();
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthorizationCore(options => { })
				.AddBlazoredOpenIdConnect(options =>
				{
					options.Authority = "https://hoang-luong-dev.onelogin.com/oidc/1/";

					options.ClientId = "9d38b830-45a3-0138-ebe5-020c7f144b4a165866";

					//options.ResponseType = "token id_token";
					options.ResponseType = "code";

					//options.WriteErrorToConsole = true;
					//options.RevokeAccessTokenOnSignout = true;

					options.PopupSignInRedirectUri = "/signin-popup-redirect";
					options.PopupSignOutRedirectUri = "/signout-popup-redirect";

					options.Scope.Add("openid");
					options.Scope.Add("profile");
				});
		}
	}
}