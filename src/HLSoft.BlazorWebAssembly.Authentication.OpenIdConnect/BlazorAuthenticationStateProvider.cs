﻿using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public class BlazorAuthenticationStateProvider<TUser> : AuthenticationStateProvider, IAuthenticationStateProvider where TUser: class
	{
		private readonly IJSRuntime _jsRuntime;
		private readonly Task<ClientOptions> _clientOptionsTask;
		private readonly Task<OpenIdConnectOptions> _openIdConnectOptionsTask;
		private readonly IServiceProvider _serviceProvider;
		private readonly NavigationManager _navigationManager;
		private readonly IClaimsParser<TUser> _claimsParser;
		private readonly AuthenticationEventHandler _authenticationEventHandler;
		private bool _initialized = false;

		public BlazorAuthenticationStateProvider(
			IJSRuntime jsRuntime, 
			NavigationManager navigationManager, 
			Task<ClientOptions> clientOptionsTask, 
			IClaimsParser<TUser> claimsParser, 
			AuthenticationEventHandler authenticationEventHandler,
			Task<OpenIdConnectOptions> openIdConnectOptionsTask,
			IServiceProvider serviceProvider)
		{
			_jsRuntime = jsRuntime;
			_navigationManager = navigationManager;
			_clientOptionsTask = clientOptionsTask;
			_claimsParser = claimsParser;
			_authenticationEventHandler = authenticationEventHandler;
			_openIdConnectOptionsTask = openIdConnectOptionsTask;
			_serviceProvider = serviceProvider;
		}

		public async Task InitializeAuthenticationData()
		{
			bool shouldHideUi = false;
			if (AuthConfig.AutomaticHideBodyWhenAuthenticating)
			{
				await _jsRuntime.InvokeVoidAsync(Constants.SetPageDisplayStatus, false);
			}
			try
			{
				await ProcessPreviousActionCode();
				if (await HandleKnownUri())
				{
					shouldHideUi = true;
				}
				else
				{
					var clientOptions = await _clientOptionsTask;
					await Utils.ConfigOidcAsync(_jsRuntime, clientOptions);
				}
			}
			finally
			{
				if (AuthConfig.AutomaticHideBodyWhenAuthenticating && !shouldHideUi)
				{
					await _jsRuntime.InvokeVoidAsync(Constants.SetPageDisplayStatus, true);
				}
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
			if (await HandleDoNothingUri()) return true;

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
			var clientOptions = await _clientOptionsTask;
			if (Utils.CurrentUriIs(clientOptions.redirect_uri, _navigationManager))
			{
				string returnUrl = null;
				try
				{
					returnUrl = await Utils.GetAndRemoveSessionStorageData(_jsRuntime, "_returnUrl");
					await _jsRuntime.InvokeVoidAsync(Constants.ProcessSigninCallback, clientOptions);
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySignInFail(err);
				}

				await Utils.SetSessionStorageData(_jsRuntime, "_previousActionCode", Constants.SignedInSuccess);
				_navigationManager.NavigateTo(returnUrl ?? clientOptions.post_logout_redirect_uri, true);

				return true;
			}
			return false;
		}

		private async Task<bool> HandleSilentCallbackUri()
		{
			var clientOptions = await _clientOptionsTask;
			if (Utils.CurrentUriIs(clientOptions.silent_redirect_uri, _navigationManager))
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
			var clientOptions = await _clientOptionsTask;
			if (Utils.CurrentUriIs(clientOptions.popup_redirect_uri, _navigationManager))
			{
				try
				{
					await _jsRuntime.InvokeVoidAsync(Constants.ProcessSigninPopup, clientOptions);
					RunAsyncTaskToClosePopup(1000);
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
			var clientOptions = await _clientOptionsTask;
			if (Utils.CurrentUriIs(clientOptions.popup_post_logout_redirect_uri, _navigationManager))
			{
				try
				{
					await _jsRuntime.InvokeVoidAsync(Constants.ProcessSignoutPopup, clientOptions);
					RunAsyncTaskToClosePopup(500);
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySignOutFail(err);
				}
				return true;
			}
			return false;
		}

		public async Task SetAuthorizationHeader(HttpClient httpClient, string tokenName = "access_token")
		{
			var authState = await GetAuthenticationStateAsync();
			var token = authState.GetClaim(tokenName);
			if (!string.IsNullOrEmpty(token))
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
		}

		private void RunAsyncTaskToClosePopup(int delay = 0)
		{
			Task.Run(async () =>
			{
				if (delay > 0)
				{
					await Task.Delay(delay);
				}
				await _jsRuntime.InvokeVoidAsync("window.close");
			});
		}

		private async Task<bool> HandleEndSessionEndpoint()
		{
			var openIdConnectOptions = await _openIdConnectOptionsTask;
			var clientOptions = await _clientOptionsTask;
			if (Utils.CurrentUriIs(clientOptions.endSessionEndpoint, _navigationManager))
			{
				try
				{
					if (openIdConnectOptions.EndSessionEndpointProcess != null)
					{
						await openIdConnectOptions.EndSessionEndpointProcess.Invoke(_serviceProvider);
					}

					var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
					var queryStrings = QueryHelpers.ParseQuery(uri.Query);
					if (queryStrings.TryGetValue("state", out var stateStr) && !string.IsNullOrEmpty(stateStr))
					{
						// the workflow is signout popup
						RunAsyncTaskToClosePopup(500);
					}
					else
					{
						_navigationManager.NavigateTo(clientOptions.post_logout_redirect_uri, true);
					}
				}
				catch (Exception err)
				{
					_authenticationEventHandler.NotifySignOutFail(err);
				}
				return true;
			}
			return false;
		}

		private async Task<bool> HandleDoNothingUri()
		{
			var clientOptions = await _clientOptionsTask;
			return Utils.CurrentUriIs(clientOptions.doNothingUri, _navigationManager);
		}
	}
}
