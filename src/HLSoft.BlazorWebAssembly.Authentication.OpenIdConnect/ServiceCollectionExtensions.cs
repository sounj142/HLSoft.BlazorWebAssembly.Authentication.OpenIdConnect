using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    public static class ServiceCollectionExtensions
	{
        /// <summary>
        /// Add authentication services for HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
        /// </summary>
        public static IServiceCollection AddBlazoredOpenIdConnect(this IServiceCollection services,
            Func<IServiceProvider, Task<OpenIdConnectOptions>> configureOptions)
        {
            return services.AddBlazoredOpenIdConnect<object, DefaultClaimsParser>(configureOptions);
        }

        /// <summary>
        /// Add authentication services for HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
        /// </summary>
        public static IServiceCollection AddBlazoredOpenIdConnect(this IServiceCollection services,
            Action<OpenIdConnectOptions> configureOptions)
        {
            return services.AddBlazoredOpenIdConnect<object, DefaultClaimsParser>(configureOptions);
        }

        /// <summary>
        /// Add authentication services for HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect using a custom ClaimsParser
        /// </summary>
        /// <typeparam name="TUser">The user type to store data get from [oidc-client].getUser() method</typeparam>
        /// <typeparam name="TClaimsParser">The Claims Parser</typeparam>
        public static IServiceCollection AddBlazoredOpenIdConnect<TUser, TClaimsParser>(this IServiceCollection services,
            Action<OpenIdConnectOptions> configureOptions)
            where TClaimsParser : class, IClaimsParser<TUser>
            where TUser : class
        {
            var options = new OpenIdConnectOptions();
            configureOptions(options);
            return services.AddBlazoredOpenIdConnect<TUser, TClaimsParser>(provider => Task.FromResult(options));
        }

        /// <summary>
        /// Add authentication services for HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect using a custom ClaimsParser
        /// </summary>
        /// <typeparam name="TUser">The user type to store data get from [oidc-client].getUser() method</typeparam>
        /// <typeparam name="TClaimsParser">The Claims Parser</typeparam>
        public static IServiceCollection AddBlazoredOpenIdConnect<TUser, TClaimsParser>(this IServiceCollection services,
            Func<IServiceProvider, Task<OpenIdConnectOptions>> configureOptions)
            where TClaimsParser : class, IClaimsParser<TUser>
            where TUser : class
        {
            return services
                .AddSharedServices(configureOptions)
                .AddSingleton<AuthenticationStateProvider, BlazorAuthenticationStateProvider<TUser>>()
                .AddSingleton<IClaimsParser<TUser>, TClaimsParser>();
        }

        private static IServiceCollection AddSharedServices(this IServiceCollection services,
            Func<IServiceProvider, Task<OpenIdConnectOptions>> configureOptions)
        {
            //services.Configure(configureOptions);
            services.AddSingleton(async provider => await configureOptions(provider));

            services.AddSingleton(async provider =>
            {
                var authOptionsTask = provider.GetRequiredService<Task<OpenIdConnectOptions>>();
                var authOptions = await authOptionsTask;
                var navigationManager = provider.GetRequiredService<NavigationManager>();
                var result = Utils.CreateClientOptionsConfigData(authOptions, navigationManager);
                return result;
            });
            return services.AddSingleton<IAuthenticationService, AuthenticationService>()
                .AddSingleton<AuthenticationEventHandler>()
                .AddSingleton(resolver => resolver.GetService<AuthenticationStateProvider>() as IAuthenticationStateProvider);
        }
    }
}
