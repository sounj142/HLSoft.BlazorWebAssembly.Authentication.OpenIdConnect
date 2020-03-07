using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    public static class ServiceCollectionExtensions
	{
        public static IServiceCollection AddBlazoredOpenIdConnect(this IServiceCollection services,
            Action<OpenIdConnectOptions> configureOptions)
        {
            return services
                .AddSharedServices(configureOptions)
                .AddSingleton<AuthenticationStateProvider, BlazorAuthenticationStateProvider<object>>()
                .AddSingleton<IClaimsParser<object>, DefaultClaimsParser>();
        }

        public static IServiceCollection AddBlazoredOpenIdConnect<TUser, TClaimsParser>(this IServiceCollection services,
            Action<OpenIdConnectOptions> configureOptions)
            where TClaimsParser : class, IClaimsParser<TUser>
            where TUser : class
        {
            return services
                .AddSharedServices(configureOptions)
                .AddSingleton<AuthenticationStateProvider, BlazorAuthenticationStateProvider<TUser>>()
                .AddSingleton<IClaimsParser<TUser>, TClaimsParser>();
        }

        private static IServiceCollection AddSharedServices(this IServiceCollection services,
            Action<OpenIdConnectOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OpenIdConnectOptions>>().Value);

            services.AddSingleton(resolver =>
            {
                var authOptions = resolver.GetRequiredService<OpenIdConnectOptions>();
                var navigationManager = resolver.GetRequiredService<NavigationManager>();
                return Utils.CreateClientOptionsConfigData(authOptions, navigationManager);
            });
            return services.AddSingleton<IAuthenticationService, AuthenticationService>()
                .AddSingleton<AuthenticationEventHandler>()
                .AddSingleton(resolver => resolver.GetService<AuthenticationStateProvider>() as IAuthenticationStateProvider);
        }
    }
}
