using System.Collections.Generic;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models
{
	public class OpenIdConnectOptions
	{
		public string Authority { get; set; }
		public string ClientId { get; set; }
		public string ResponseType { get; set; }
		public ICollection<string> Scope { get; } = new List<string>();
		public bool LoadUserInfo { get; set; } = true;
		public bool AutomaticSilentRenew { get; set; } = false;
		public bool MonitorAnonymousSession { get; set; } = false;
		public bool RevokeAccessTokenOnSignout { get; set; } = true;
		public bool FilterProtocolClaims { get; set; } = true;
		public string PopupWindowFeatures { get; set; } = "menubar=yes,location=yes,toolbar=yes,width=1200,height=800,left=100,top=100;resizable=yes";
		public bool WriteErrorToConsole { get; set; }
		public string SignedOutRedirectUri { get; set; } = "/";

		public string SignedInCallbackUri { get; set; } = "/signin-callback-oidc";
		public string SilentRedirectUri { get; set; }
		public string PopupSignInRedirectUri { get; set; }
		public string PopupSignOutRedirectUri { get; set; }
	}
}
