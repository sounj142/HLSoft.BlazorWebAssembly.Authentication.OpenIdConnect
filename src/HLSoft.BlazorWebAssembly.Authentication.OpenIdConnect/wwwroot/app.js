(function () {
	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect = {};
	let mgr = null;

	function notifySilentRenewError(err) {
		DotNet.invokeMethodAsync('HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect', 'NotifySilentRefreshTokenFail', err);
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.configOidc = function (config) {
		if (!mgr) {
			mgr = new Oidc.UserManager(config);
			mgr._events.addSilentRenewError(notifySilentRenewError);
			window.mmm = mgr;
		}
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.signinRedirect = function () {
		return mgr.signinRedirect();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.signoutRedirect = function () {
		return mgr.signoutRedirect();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.getUser = function () {
		return mgr ? mgr.getUser() : null;
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.removeUser = function () {
		return mgr.removeUser();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.signinPopup = function () {
		return mgr.signinPopup();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.signoutPopup = function () {
		return mgr.signoutPopup();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.signinSilent = function () {
		return mgr.signinSilent();
	}

	function createUserManager(isCode) {
		return isCode
			? new Oidc.UserManager({ loadUserInfo: true, response_mode: "query" })
			: new Oidc.UserManager();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSigninCallback = function (isCode) {
		let mgr = createUserManager(isCode);
		return mgr.signinRedirectCallback().then();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSilentCallback = function () {
		let mgr = new Oidc.UserManager({});
		return mgr.signinSilentCallback(window.location.href);
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSigninPopup = function (isCode) {
		let mgr = createUserManager(isCode);
		return mgr.signinPopupCallback();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSignoutPopup = function (isCode) {
		let mgr = createUserManager(isCode);
		mgr.signoutPopupCallback(false);
	}
})();