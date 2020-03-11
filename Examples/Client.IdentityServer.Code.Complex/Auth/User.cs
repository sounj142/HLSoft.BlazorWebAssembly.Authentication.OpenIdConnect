namespace Client.IdentityServer.Code.Complex.Auth
{
	public class User
	{
		public string id_token { get; set; }
		public string session_state { get; set; }
		public string access_token { get; set; }
		public string refresh_token { get; set; }
		public string token_type { get; set; }
		public string scope { get; set; }
		public long expires_at { get; set; }
		public Profile profile { get; set; }
	}
}
