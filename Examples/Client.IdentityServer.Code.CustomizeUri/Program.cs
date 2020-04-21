﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using System;
using System.Net.Http;

namespace Client.IdentityServer.Code.CustomizeUri
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

					options.ClientId = "Client.Code.CustomizeUri";
					options.ResponseType = "code";

					options.SignedInCallbackUri = "/fantastic-url-for-redirect";
					options.PopupSignInRedirectUri = "/wonderful-link-for-popup-login";
					options.PopupSignOutRedirectUri = "/sign-out-popup-here";

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
