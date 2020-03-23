using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using Client.IdentityServer.Code.Complex.Auth;
using Microsoft.AspNetCore.Authorization;
using MatBlazor;
using System;

namespace Client.IdentityServer.Code.Complex
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
			services.AddAuthorizationCore(options =>
			{
				options.AddPolicy(
					Policies.CanManageWeatherForecast,
					new AuthorizationPolicyBuilder()
						.RequireAuthenticatedUser()
						.RequireClaim("api_role", "Admin")
						.Build());
			});
			AuthConfig.WriteErrorToConsole = true;

			//services.AddBlazoredOpenIdConnect(options => // switch to this line to use default ClaimsParser
			services.AddBlazoredOpenIdConnect<User, CustomClaimsParser>(options => // note: don't use this config with External Google/IdentityServer, the User class is not compatible with claims from these source
			{
				options.Authority = "https://localhost:5000/";

				options.ClientId = "Client.Code.Complex";
				options.ResponseType = "code";

				options.AutomaticSilentRenew = true;

				options.PopupSignInRedirectUri = "/signin-popup-oidc";
				options.SignedInCallbackUri = "/signin-callback-oidc";
				options.SilentRedirectUri = "/silent-callback-oidc";

				options.Scopes.Add("openid");
				options.Scopes.Add("profile");
				options.Scopes.Add("email");
				options.Scopes.Add("address");
				options.Scopes.Add("api_role");
				options.Scopes.Add("api");
			});

			services.AddMatToaster(config =>
			{
				config.Position = MatToastPosition.BottomRight;
				config.PreventDuplicates = true;
				config.NewestOnTop = true;
				config.ShowCloseButton = true;
				config.MaximumOpacity = 95;
				config.VisibleStateDuration = 3000;
			});

			services.AddHttpClient<WeatherForecastService>(client =>
			{
				client.BaseAddress = new Uri("http://localhost:5001/");
			});
		}
	}
}
