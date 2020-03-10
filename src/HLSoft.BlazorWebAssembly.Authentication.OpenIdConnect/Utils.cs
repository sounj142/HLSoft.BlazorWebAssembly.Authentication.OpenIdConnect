using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	internal static class Utils
	{
		public static ValueTask ConfigOidcAsync(IJSRuntime jsRuntime, ClientOptions clientOptions)
		{
			return jsRuntime.InvokeVoidAsync(Constants.ConfigOidc, clientOptions);
		}

		public static ClientOptions CreateClientOptionsConfigData(OpenIdConnectOptions authOption, NavigationManager navigationManager)
		{
			return new ClientOptions
			{
				authority = authOption.Authority,
				client_id = authOption.ClientId,
				redirect_uri = navigationManager.GetAbsoluteUri(authOption.SignedInCallbackUri),
				silent_redirect_uri = navigationManager.GetAbsoluteUri(authOption.SilentRedirectUri),
				response_type = authOption.ResponseType,
				scope = string.Join(" ", authOption.Scope.Distinct()),
				post_logout_redirect_uri = navigationManager.GetAbsoluteUri(authOption.SignedOutRedirectUri),
				popup_redirect_uri = navigationManager.GetAbsoluteUri(authOption.PopupSignInRedirectUri),
				popup_post_logout_redirect_uri = navigationManager.GetAbsoluteUri(authOption.PopupSignOutRedirectUri),
				loadUserInfo = authOption.LoadUserInfo,
				automaticSilentRenew = authOption.AutomaticSilentRenew,
				revokeAccessTokenOnSignout = authOption.RevokeAccessTokenOnSignout,
				filterProtocolClaims = authOption.FilterProtocolClaims,
				popupWindowFeatures = authOption.PopupWindowFeatures,
				clockSkew = authOption.ClockSkew,
				popupWindowTarget = authOption.PopupWindowTarget,
				silentRequestTimeout = authOption.SilentRequestTimeout,
				accessTokenExpiringNotificationTime = authOption.AccessTokenExpiringNotificationTime,
				monitorSession = authOption.MonitorSession,
				checkSessionInterval = authOption.CheckSessionInterval,
				includeIdTokenInSilentRenew = authOption.IncludeIdTokenInSilentRenew,
				staleStateAge = authOption.StaleStateAge,
				extraQueryParams = authOption.ExtraQueryParams,
				extraTokenParams = authOption.ExtraTokenParams,
				endSessionEndpoint = navigationManager.GetAbsoluteUri(authOption.EndSessionEndpoint),
				doNothingUri = navigationManager.GetAbsoluteUri(authOption.DoNothingUri),
			};
		}

		public static async Task<string> GetAndRemoveSessionStorageData(IJSRuntime jsRuntime, string name)
		{
			var returnUrl = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", name);
			await RemoveSessionStorageData(jsRuntime, name);
			return returnUrl;
		}

		public static async Task SetSessionStorageData(IJSRuntime jsRuntime, string name, string value)
		{
			await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", name, value);
		}

		public static async Task RemoveSessionStorageData(IJSRuntime jsRuntime, string name)
		{
			await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", name);
		}

		public static bool CurrentUriIs(string url, NavigationManager navigationManager)
		{
			return !string.IsNullOrWhiteSpace(url) && navigationManager.Uri.StartsWith(url, StringComparison.OrdinalIgnoreCase);
		}
	}
}
