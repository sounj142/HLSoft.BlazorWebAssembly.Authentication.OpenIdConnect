(function () {
	const oidcManager = {};
	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect = oidcManager;
	let mgr = null;
	let userStorage = window.sessionStorage;

	function notifySilentRenewError(err) {
		DotNet.invokeMethodAsync('HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect', 'NotifySilentRefreshTokenFail', err);
	}

	function prepareOidcConfig(config) {
		if (!config) config = {};
		config.userStore = new Oidc.WebStorageStateStore({ store: userStorage });
		return config;
	}

	oidcManager.notifySilentRenewSuccess = function () {
		DotNet.invokeMethodAsync('HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect', 'NotifySilentRefreshTokenSuccess');
	}

	oidcManager.configOidc = function (config) {
		if (!mgr) {
			mgr = new Oidc.UserManager(prepareOidcConfig(config));
			// subscribe SilentRenewError event
			mgr.events.addSilentRenewError(notifySilentRenewError);
			// if there is a custom endSessionEndpoint, hack the Oidc.UserManager to use that url as the session endpoint
			if (config.endSessionEndpoint) {
				mgr.metadataService.getEndSessionEndpoint = function () {
					return Promise.resolve(config.endSessionEndpoint);
				}
			}
		}
	}

	oidcManager.signinRedirect = function () {
		return mgr.signinRedirect();
	}

	oidcManager.signoutRedirect = function () {
		return mgr.signoutRedirect();
	}

	oidcManager.getUser = function () {
		return mgr ? mgr.getUser() : null;
	}

	oidcManager.removeUser = function () {
		return mgr.removeUser();
	}

	oidcManager.signinPopup = function () {
		return mgr.signinPopup();
	}

	oidcManager.signoutPopup = function () {
		return mgr.signoutPopup();
	}

	oidcManager.signinSilent = function () {
		return mgr.signinSilent();
	}

	function createUserManager(config) {
		return config.isCode
			? new Oidc.UserManager(prepareOidcConfig({ ...config, response_mode: 'query' }))
			: new Oidc.UserManager(prepareOidcConfig(config));
	}

	oidcManager.processSigninCallback = function (config) {
		let mgr = createUserManager(config);
		return mgr.signinRedirectCallback();
	}

	oidcManager.processSilentCallback = function () {
		let mgr = new Oidc.UserManager(prepareOidcConfig());
		return mgr.signinSilentCallback(window.location.href).then(() => {
			// notify parent window that the silent refresh token succeeded
			if (window.parent !== window && window.parent.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect) {
				window.parent.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.notifySilentRenewSuccess();
			}
		});
	}

	oidcManager.processSigninPopup = function (config) {
		let mgr = createUserManager(config);
		return mgr.signinPopupCallback();
	}

	oidcManager.processSignoutPopup = function (config) {
		let mgr = createUserManager(config);
		mgr.signoutPopupCallback(false);
	}

	oidcManager.setPageDisplayStatus = function (show) {
		document.body.style.display = show ? 'block' : 'none';
	}

	oidcManager.silentOpenUrlInIframe = function (url, timeout) {
		return new Promise((resolve, reject) => {
			let iframe = document.createElement('iframe');
			iframe.style.display = 'none';
			iframe.setAttribute('src', url);
			document.body.appendChild(iframe);

			let timer = window.setTimeout(() => {
				reject(new Error('IFrame window timed out.'));
			}, timeout);
			iframe.onload = () => {
				document.body.removeChild(iframe);
				window.clearTimeout(timer);
				resolve();
			};
		});
	}
	// call this method if you want to change the default user store (default: sessionStorage)
	oidcManager.configUserStore = function (store) {
		userStorage = store;
	}
})();