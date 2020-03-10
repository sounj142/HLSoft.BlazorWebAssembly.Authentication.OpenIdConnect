using System;
using System.Linq;

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
		public bool revokeAccessTokenOnSignout { get; set; }
		public bool filterProtocolClaims { get; set; }
		public int clockSkew { get; set; }
		public string popupWindowTarget { get; set; }
		public int? silentRequestTimeout { get; set; }
		public int accessTokenExpiringNotificationTime { get; set; }
		public bool monitorSession { get; set; }
		public int checkSessionInterval { get; set; }
		public bool includeIdTokenInSilentRenew { get; set; }
		public int staleStateAge { get; set; }
		public object extraQueryParams { get; set; }
		public object extraTokenParams { get; set; }

		public string endSessionEndpoint { get; set; }
		public string doNothingUri { get; set; }

		public bool IsCode
		{
			get
			{
				return (response_type ?? string.Empty)
					.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
					.Any(c => c == "code");
			}
		}
	}
}
