using System.Collections.Generic;

namespace Client.IdentityServer.Code.Complex.Auth
{
	public class Profile
	{
		public string sid { get; set; }
		public string sub { get; set; }
		public string idp { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public bool email_verified { get; set; }
		public string website { get; set; }
		public IList<string> api_role { get; set; }
		public Address address { get; set; }
	}
}
