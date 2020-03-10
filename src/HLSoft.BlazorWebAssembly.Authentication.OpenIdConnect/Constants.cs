namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public static class Constants
	{
		private const string JavascriptPrefix = "HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.";
		/// <summary>
		/// Returns promise to trigger a redirect of the current window to the authorization endpoint
		/// </summary>
		public const string SigninRedirect = JavascriptPrefix + "signinRedirect";
		/// <summary>
		/// Returns promise to trigger a redirect of the current window to the end session endpoint.
		/// </summary>
		public const string SignoutRedirect = JavascriptPrefix + "signoutRedirect";
		/// <summary>
		/// Returns promise to load the User object for the currently authenticated user.
		/// </summary>
		public const string GetUser = JavascriptPrefix + "getUser";
		/// <summary>
		/// Returns promise to remove from any storage the currently authenticated user.
		/// </summary>
		public const string RemoveUser = JavascriptPrefix + "removeUser";
		/// <summary>
		/// Use the OpenIdConnectOptions object to create object Oidc.UserManager
		/// </summary>
		public const string ConfigOidc = JavascriptPrefix + "configOidc";
		/// <summary>
		/// Returns promise to trigger a silent request (via an iframe) to the authorization endpoint. The result of the promise is the authenticated User.
		/// </summary>
		public const string SigninSilent = JavascriptPrefix + "signinSilent";
		/// <summary>
		/// Returns promise to notify the parent window of response from the authorization endpoint(signinSilentCallback)
		/// </summary>
		public const string ProcessSilentCallback = JavascriptPrefix + "processSilentCallback";
		/// <summary>
		/// Returns promise to process response from the authorization endpoint. The result of the promise is the authenticated User (signinRedirectCallback)
		/// </summary>
		public const string ProcessSigninCallback = JavascriptPrefix + "processSigninCallback";
		/// <summary>
		/// Returns promise to notify the opening window of response from the authorization endpoint(signinPopupCallback)
		/// </summary>
		public const string ProcessSigninPopup = JavascriptPrefix + "processSigninPopup";
		/// <summary>
		/// Returns promise to process response from the end session endpoint from a popup window. (signoutPopupCallback)
		/// </summary>
		public const string ProcessSignoutPopup = JavascriptPrefix + "processSignoutPopup";
		/// <summary>
		/// Returns promise to trigger a request (via a popup window) to the authorization endpoint. The result of the promise is the authenticated User
		/// </summary>
		public const string SigninPopup = JavascriptPrefix + "signinPopup";
		/// <summary>
		/// Returns promise to trigger a redirect of a popup window window to the end session endpoint.
		/// </summary>
		public const string SignoutPopup = JavascriptPrefix + "signoutPopup";
		/// <summary>
		/// load an url in a hidden iframe
		/// </summary>
		public const string SilentOpenUrlInIframe = JavascriptPrefix + "silentOpenUrlInIframe";
		/// <summary>
		/// set body.display = none to hide all content in page
		/// </summary>
		public const string HideAllPage = JavascriptPrefix + "hideAllPage";
		public const string SignedInSuccess = "1";
		public const string SignedOutSuccess = "2";
	}
}
