using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    internal class AuthenticationService : IAuthenticationService
	{
		private readonly IJSRuntime _jsRuntime;
		private readonly IAuthenticationStateProvider _authenticationStateProvider;
		private readonly AuthenticationEventHandler _authenticationEventHandler;
		private readonly NavigationManager _navigationManager;
		private readonly ClientOptions _clientOptions;

		public AuthenticationService(
			IJSRuntime jsRuntime,
			IAuthenticationStateProvider authenticationStateProvider,
			AuthenticationEventHandler authenticationEventHandler, 
			NavigationManager navigationManager, 
			ClientOptions clientOptions)
		{
			_jsRuntime = jsRuntime;
			_authenticationStateProvider = authenticationStateProvider;
			_authenticationEventHandler = authenticationEventHandler;
			_navigationManager = navigationManager;
			_clientOptions = clientOptions;
		}

		/// <summary>
		/// trigger a redirect from the current window to the authorization endpoint.
		/// </summary>
		/// <returns></returns>
		public async Task SignInAsync()
		{
			try
			{
				await Utils.SetSessionStorageData(_jsRuntime, "_returnUrl", _navigationManager.Uri);
				await _jsRuntime.InvokeVoidAsync(Constants.SigninRedirect);
			}
			catch (Exception err)
			{
				_authenticationEventHandler.NotifySignInFail(err);
			}
		}
		
		public async Task SignInPopupAsync()
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync(Constants.SigninPopup);
				_authenticationEventHandler.NotifySignInSuccess();
			}
			catch (Exception err)
			{
				_authenticationEventHandler.NotifySignInFail(err);
			}
			_authenticationStateProvider.NotifyAuthenticationStateChanged();
		}

		public async Task SignOutAsync()
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync(Constants.SignoutRedirect);
				await Utils.SetSessionStorageData(_jsRuntime, "_previousActionCode", Constants.SignedOutSuccess);
			}
			catch (Exception err)
			{
				_authenticationEventHandler.NotifySignOutFail(err);
			}
		}

		public async Task SignOutPopupAsync()
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync(Constants.SignoutPopup);
				await _jsRuntime.InvokeVoidAsync(Constants.RemoveUser);
				_authenticationEventHandler.NotifySignOutSuccess();
			}
			catch (Exception err)
			{
				_authenticationEventHandler.NotifySignOutFail(err);
			}
			_authenticationStateProvider.NotifyAuthenticationStateChanged();
		}

		public async Task RequireAuthenticationAsync()
		{
			if (!CurrentUriIsAuthenticationUri())
			{
				var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
				if (!authenticationState.User.Identity.IsAuthenticated)
				{
					await SignInAsync();
				}
			}
		}

		public bool CurrentUriIsAuthenticationUri()
		{
			return Utils.CurrentUriIs(_clientOptions.redirect_uri, _navigationManager) ||
				Utils.CurrentUriIs(_clientOptions.silent_redirect_uri, _navigationManager) ||
				Utils.CurrentUriIs(_clientOptions.popup_redirect_uri, _navigationManager) ||
				Utils.CurrentUriIs(_clientOptions.popup_post_logout_redirect_uri, _navigationManager);
		}
	}
}
