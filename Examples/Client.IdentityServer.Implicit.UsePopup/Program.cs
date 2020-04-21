using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using System;
using System.Net.Http;

namespace Client.IdentityServer.Implicit.UsePopup
{
	public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder.Services);

            builder.RootComponents.Add<App>("app");
			builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
			await builder.Build().RunAsync();
        }

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions()
				.AddAuthorizationCore()
				.AddBlazoredOpenIdConnect(options =>
				{
					options.Authority = "https://localhost:5000/";

					options.ClientId = "Client.Implicit.UsePopup";
					options.ResponseType = "token id_token";

					options.SignedInCallbackUri = null;
					options.PopupSignInRedirectUri = "/signin-popup-oidc";
					options.PopupSignOutRedirectUri = "/signout-popup-oidc";

					options.Scopes.Add("openid");
					options.Scopes.Add("profile");
					options.Scopes.Add("api");
				});

			services.AddHttpClient<WeatherForecastService>(client =>
			{
				client.BaseAddress = new Uri("http://localhost:5001/");
			});
		}
	}
}
