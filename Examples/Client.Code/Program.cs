using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using System;

namespace Client.Code
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
					options.Authority = "http://localhost:5000/";

					options.ClientId = "Client.Code";
					options.ResponseType = "code";

					options.Scope.Add("openid");
					options.Scope.Add("profile");
					options.Scope.Add("api");
				});
			services.AddHttpClient<WeatherForecastService>(client =>
				{
					client.BaseAddress = new Uri("http://localhost:5001/");
				});
		}
	}
}
