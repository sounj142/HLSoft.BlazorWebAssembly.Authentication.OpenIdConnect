using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models
{
	public class OpenIdConnectOptions
	{
		/// <summary>
		/// [Required] The URL of the OIDC/OAuth2 provider (oidc-client-js:authority)
		/// </summary>
		public string Authority { get; set; }

		/// <summary>
		/// [Required] Your client application's identifier as registered with the OIDC/OAuth2 provider (oidc-client-js:client_id)
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// [Danger!!!] I added ClientSecret here for completeness but please don't use it if possible.
		/// </summary>
		public string ClientSecret { get; set; }

		/// <summary>
		/// [Required] The type of response desired from the OIDC/OAuth2 provider (oidc-client-js:response_type)
		/// </summary>
		public string ResponseType { get; set; }
		/// <summary>
		/// [Required] The scopes being requested from the OIDC/OAuth2 provider (oidc-client-js:scope)
		/// </summary>
		public ICollection<string> Scopes { get; } = new List<string>();
		/// <summary>
		/// Flag to control if additional identity data is loaded from the user info endpoint in order to populate the user's profile (oidc-client-js:loadUserInfo) 
		/// Default: true
		/// </summary>
		public bool LoadUserInfo { get; set; } = true;
		/// <summary>
		/// Flag to indicate if there should be an automatic attempt to renew the access token prior to its expiration. The attempt is made as a result of the accessTokenExpiring event being raised (oidc-client-js:automaticSilentRenew) 
		/// Default: false
		/// </summary>
		public bool AutomaticSilentRenew { get; set; } = false;
		/// <summary>
		/// Will invoke the revocation endpoint on signout if there is an access token for the user (oidc-client-js:revokeAccessTokenOnSignout) 
		/// Default: false
		/// </summary>
		public bool RevokeAccessTokenOnSignout { get; set; } = false;
		/// <summary>
		/// Should OIDC protocol claims be removed from profile (oidc-client-js:filterProtocolClaims) 
		/// Default: true
		/// </summary>
		public bool FilterProtocolClaims { get; set; } = true;
		/// <summary>
		/// The features parameter to window.open for the popup signin window. (oidc-client-js:popupWindowFeatures) 
		/// Default: "location=no,toolbar=no,width=500,height=500,left=100,top=100;"
		/// </summary>
		public string PopupWindowFeatures { get; set; } = "location=no,toolbar=no,width=500,height=500,left=100,top=100;";
		/// <summary>
		/// Whether write the processing error to Console, should enable only in Devlopment
		/// Default: true
		/// </summary>
		public bool WriteErrorToConsole { get; set; } = true;
		/// <summary>
		/// The OIDC/OAuth2 post-logout redirect URI (oidc-client-js:post_logout_redirect_uri) 
		/// Default: "/"
		/// </summary>
		public string SignedOutRedirectUri { get; set; } = "/";
		/// <summary>
		/// [Required] The redirect URI of your client application to receive a response from the OIDC/OAuth2 provider (oidc-client-js:redirect_uri) 
		/// Default: "/signin-callback-oidc"
		/// </summary>
		public string SignedInCallbackUri { get; set; } = "/signin-callback-oidc";
		/// <summary>
		/// The URL for the page containing the code handling the silent renew (oidc-client-js:silent_redirect_uri) 
		/// </summary>
		public string SilentRedirectUri { get; set; }
		/// <summary>
		/// The URL for the page containing the call to signinPopupCallback to handle the callback from the OIDC/OAuth2 (oidc-client-js:popup_redirect_uri) 
		/// </summary>
		public string PopupSignInRedirectUri { get; set; }
		/// <summary>
		/// The URL for the page that should be callback in the Popup sign-out scenarios (oidc-client-js:popup_post_logout_redirect_uri) 
		/// </summary>
		public string PopupSignOutRedirectUri { get; set; }
		/// <summary>
		/// The window of time (in seconds) to allow the current time to deviate when validating id_token's iat, nbf, and exp values. (oidc-client-js:clockSkew) 
		/// Default: 300
		/// </summary>
		public int ClockSkew { get; set; } = 60 * 5;
		/// <summary>
		/// The target parameter to window.open for the popup signin window (oidc-client-js:popupWindowTarget) 
		/// Default: "_blank"
		/// </summary>
		public string PopupWindowTarget { get; set; }
		/// <summary>
		/// Number of milliseconds to wait for the silent renew to return before assuming it has failed or timed out (oidc-client-js:silentRequestTimeout) 
		/// Default: 10000
		/// </summary>
		public int? SilentRequestTimeout { get; set; }
		/// <summary>
		/// The number of seconds before an access token is to expire to raise the accessTokenExpiring event. (oidc-client-js:accessTokenExpiringNotificationTime) 
		/// Default: 60
		/// </summary>
		public int AccessTokenExpiringNotificationTime { get; set; } = 60;
		/// <summary>
		/// Will raise events for when user has performed a signout at the OP (oidc-client-js:monitorSession) 
		/// Default: true
		/// </summary>
		public bool MonitorSession { get; set; } = true;
		/// <summary>
		/// Interval, in ms, to check the user's session. (oidc-client-js:checkSessionInterval) 
		/// Default: 2000
		/// </summary>
		public int CheckSessionInterval { get; set; } = 2000;
		/// <summary>
		/// Flag to control if id_token is included as id_token_hint in silent renew calls (oidc-client-js:includeIdTokenInSilentRenew) 
		/// Default: true
		/// </summary>
		public bool IncludeIdTokenInSilentRenew { get; set; } = true;
		/// <summary>
		/// Number (in seconds) indicating the age of state entries in storage for authorize requests that are considered abandoned and thus can be cleaned up (oidc-client-js:staleStateAge) 
		/// Default: 900
		/// </summary>
		public int StaleStateAge { get; set; } = 60 * 15;
		/// <summary>
		/// An object containing additional query string parameters to be including in the authorization request. E.g, when using Azure AD to obtain an access token an additional resource parameter is required. extraQueryParams: {resource:"some_identifier"} (oidc-client-js:extraQueryParams) 
		/// Default: {}
		/// </summary>
		public object ExtraQueryParams { get; set; } = new object();
		/// <summary>
		/// (oidc-client-js:extraTokenParams) 
		/// Default: {}
		/// </summary>
		public object ExtraTokenParams { get; set; } = new object();

		public string EndSessionEndpoint { get; set; } = null;

		public delegate Task EndSessionEndpointHandler(IServiceProvider provider);

		public EndSessionEndpointHandler EndSessionEndpointProcess { get; set; } = null;

		/// <summary>
		/// Some special Identity Providers need to redirect to some kind of URL, but we don't want to process anything at that URL,
		/// so we use this "empty url" for these situations.
		/// Default: "oidc-nothing"
		/// </summary>
		public string DoNothingUri { get; set; } = "/oidc-nothing";

		/// <summary>
		/// manually configure OIDC/OAuth2 provider metadata endpoints
		/// </summary>
		public OidcMetadata Metadata { get; set; }
	}

	public class OidcMetadata
	{
		/// <summary>
		/// The fully qualified issuer URL of the server
		/// </summary>
		public string Issuer { get; set; }

		/// <summary>
		/// The fully qualified URL of the server’s authorization endpoint defined by RFC 6749
		/// </summary>
		public string AuthorizationEndpoint { get; set; }

		/// <summary>
		/// The OpenID Connect UserInfo endpoint URL.
		/// </summary>
		public string UserinfoEndpoint { get; set; }

		/// <summary>
		///  The fully qualified URL of the server’s token endpoint defined by RFC 6749
		/// </summary>
		public string TokenEndpoint { get; set; }

		/// <summary>
		/// The OpenID Connect check session iframe URL, omitted if disabled.
		/// </summary>
		public string CheckSessionIframe { get; set; }

		/// <summary>
		/// The OpenID Connect logout endpoint URL
		/// </summary>
		public string EndSessionEndpoint { get; set; }

		/// <summary>
		/// The fully qualified URL of the server’s revocation endpoint defined by OAuth 2.0 Authorization Server Metadata (and sort of in OAuth 2.0 Token Revocation)
		/// </summary>
		public string RevocationEndpoint { get; set; }

		/// <summary>
		/// The fully qualified URI of the server’s public key in JSON Web Key Set (JWKS) format
		/// </summary>
		public string JwksUri { get; set; }
	}
}
