﻿using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.IdentityServer.Code.RemotelyConfig
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
			//AuthConfig.AutomaticHideBodyWhenAuthenticating = false;

			services.AddOptions()
				.AddAuthorizationCore()
				.AddBlazoredOpenIdConnect(async (provider) =>
				{
					var httpClient = provider.GetRequiredService<HttpClient>();
					var result = await httpClient.GetFromJsonAsync<OpenIdConnectOptions>("http://localhost:5001/Configs");
					return result;
				});

			services.AddHttpClient<WeatherForecastService>(client =>
				{
					client.BaseAddress = new Uri("http://localhost:5001/");
				});
		}
	}
}