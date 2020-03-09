(function () {
	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect = {};
	let mgr = null;

	function getParameterByName(name, url) {
		if (!url) url = window.location.href;
		name = name.replace(/[\[\]]/g, '\\$&');
		let regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
			results = regex.exec(url);
		if (!results) return null;
		if (!results[2]) return '';
		return decodeURIComponent(results[2].replace(/\+/g, ' '));
	}

	function notifySilentRenewError(err) {
		DotNet.invokeMethodAsync('HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect', 'NotifySilentRefreshTokenFail', err);
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.configOidc = function (config) {
		if (!mgr) {
			if (config && config.client_id) {
				sessionStorage.setItem('_configOidc', JSON.stringify(config));
			}
			else {
				let str = sessionStorage.getItem('_configOidc');
				config = str ? JSON.parse(str) : null;
			}
			mgr = new Oidc.UserManager(config);
			mgr._events.addSilentRenewError(notifySilentRenewError);
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

	function createUserManager() {
		return getParameterByName('session_state') && window.location.href.indexOf('?') > 0
			? new Oidc.UserManager({ loadUserInfo: true, response_mode: "query" })
			: new Oidc.UserManager();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSigninCallback = function () {
		let mgr = createUserManager();
		return mgr.signinRedirectCallback().then();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSilentCallback = function () {
		let mgr = new Oidc.UserManager({});
		return mgr.signinSilentCallback(window.location.href);
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSigninPopup = function () {
		let mgr = createUserManager();
		return mgr.signinPopupCallback();
	}

	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.processSignoutPopup = function () {
		let mgr = createUserManager();
		mgr.signoutPopupCallback(false);
	}
})();