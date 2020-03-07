namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models
{
	public class ClientOptions
	{
		public string authority { get; set; }
		public string client_id { get; set; }
		public string redirect_uri { get; set; }
		public string silent_redirect_uri { get; set; }
		public string response_type { get; set; }
		public string scope { get; set; }
		public string post_logout_redirect_uri { get; set; }
		public string popup_redirect_uri { get; set; }
		public string popup_post_logout_redirect_uri { get; set; }
		public string popupWindowFeatures { get; set; }
		public bool loadUserInfo { get; set; }
		public bool automaticSilentRenew { get; set; }
		public bool monitorAnonymousSession { get; set; }
		public bool revokeAccessTokenOnSignout { get; set; }
		public bool filterProtocolClaims { get; set; }
	}
}
