using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public class BlazorAuthenticationStateProvider<TUser> : AuthenticationStateProvider, IAuthenticationStateProvider where TUser: class
	{
		private readonly IJSRuntime _jsRuntime;
		private readonly ClientOptions _clientOptions;
		private readonly HttpClient _httpClient;
		private readonly NavigationManager _navigationManager;
		private readonly IClaimsParser<TUser> _claimsParser;
		private readonly AuthenticationEventHandler _authenticationEventHandler;
		private bool _initialized = false;

		public BlazorAuthenticationStateProvider(
			IJSRuntime jsRuntime, 
			NavigationManager navigationManager, 
			ClientOptions clientOptions, 
			IClaimsParser<TUser> claimsParser, 
			AuthenticationEventHandler authenticationEventHandler,
			HttpClient httpClient)
		{
			_jsRuntime = jsRuntime;
			_navigationManager = navigationManager;
			_clientOptions = clientOptions;
			_claimsParser = claimsParser;
			_authenticationEventHandler = authenticationEventHandler;
			_httpClient = httpClient;
		}

		public async Task InitializeAuthenticationData()
		{
			await ProcessPreviousActionCode();
			if (!await HandleKnownUri())
			{
				await Utils.ConfigOidcAsync(_jsRuntime, _clientOptions);
			}
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			if (!_initialized)
			{
				await InitializeAuthenticationData();
				_initialized = true;
			}

			var user = await _jsRuntime.InvokeAsync<TUser>(Constants.GetUser);
			//Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(user));
			var claimsIdentity = _claimsParser.CreateIdentity(user);
			return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
		}

		public void NotifyAuthenticationStateChanged()
		{
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		private async Task<bool> HandleKnownUri()
		{
			if (await HandleSigninCallbackUri()) return true;
			if (await HandleSilentCallbackUri()) return true;
			if (await HandleSigninPopupUri()) return true;
			if (await HandleSignoutPopupUri()) return true;
			if (await HandleEndSessionEndpoint()) return true;

			return false;
		}

		private async Task ProcessPreviousActionCode()
		{
			var previousActionCode = await Utils.GetAndRemoveSessionStorageData(_jsRuntime, "_previousActionCode");
			switch (previousActionCode)
			{
				case Constants.SignedInSuccess:
					_authenticationEventHandler.NotifySignInSuccess();
					break;
				case Constants.SignedOutSuccess:
					_authenticationEventHandler.NotifySignOutSuccess();
					break;
			}
		}

		private async Task<bool> HandleSigninCallbackUri()
		{
			if (Utils.CurrentUriIs(_clientOptions.redirect_uri, _navigationManager))
			{
				string returnUrl = null;
				try
				{
					returnUrl = await Utils.GetAndRemoveSessionStorageData(_jsRuntime, "_returnUrl");
					await _jsRuntime.InvokeVoidAsync(Constants.ProcessSigninCallback, _clientOptions.IsCode);
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySignInFail(err);
				}

				await Utils.SetSessionStorageData(_jsRuntime, "_previousActionCode", Constants.SignedInSuccess);
				_navigationManager.NavigateTo(returnUrl ?? _clientOptions.post_logout_redirect_uri, true);

				return true;
			}
			return false;
		}

		private async Task<bool> HandleSilentCallbackUri()
		{
			if (Utils.CurrentUriIs(_clientOptions.silent_redirect_uri, _navigationManager))
			{
				try
				{
					await _jsRuntime.InvokeVoidAsync(Constants.ProcessSilentCallback);
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySilentRefreshTokenFail(err);
				}
				return true;
			}
			return false;
		}

		private async Task<bool> HandleSigninPopupUri()
		{
			if (Utils.CurrentUriIs(_clientOptions.popup_redirect_uri, _navigationManager))
			{
				try
				{
					await _jsRuntime.InvokeVoidAsync(Constants.ProcessSigninPopup, _clientOptions.IsCode);
					await _jsRuntime.InvokeVoidAsync("window.close");
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySignInFail(err);
				}
				return true;
			}
			return false;
		}

		private async Task<bool> HandleSignoutPopupUri()
		{
			if (Utils.CurrentUriIs(_clientOptions.popup_post_logout_redirect_uri, _navigationManager))
			{
				try
				{
					await _jsRuntime.InvokeVoidAsync(Constants.ProcessSignoutPopup, _clientOptions.IsCode);
					await _jsRuntime.InvokeVoidAsync("window.close");
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySignOutFail(err);
				}
				return true;
			}
			return false;
		}

		public async Task<HttpClient> GetHttpClientAsync(string tokenName = "access_token")
		{
			var authState = await GetAuthenticationStateAsync();
			_httpClient.DefaultRequestHeaders.Authorization = null;
			if (authState.User.Identity.IsAuthenticated)
			{
				var token = authState.User.Claims.FirstOrDefault(x => x.Type == tokenName);
				if (!string.IsNullOrEmpty(token?.Value))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
				}
			}
			return _httpClient;
		}

		private async Task<bool> HandleEndSessionEndpoint()
		{
			if (Utils.CurrentUriIs(_clientOptions.endSessionEndpoint, _navigationManager))
			{
				try
				{
					Console.WriteLine("----- Url: " + _navigationManager.Uri);

					var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
					var queryStrings = QueryHelpers.ParseQuery(uri.Query);
					if (queryStrings.TryGetValue("state", out var stateStr) && !string.IsNullOrEmpty(stateStr))
					{
						// the workflow is signout popup
						await _jsRuntime.InvokeVoidAsync("window.close");
					}
					else
					{
						_navigationManager.NavigateTo(_clientOptions.post_logout_redirect_uri, true);
					}
					//await _jsRuntime.InvokeVoidAsync(Constants.ProcessSigninPopup, _clientOptions.IsCode);
					//await _jsRuntime.InvokeVoidAsync("window.close");
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySignOutFail(err);
				}
				return true;
			}
			return false;
		}
	}
}
