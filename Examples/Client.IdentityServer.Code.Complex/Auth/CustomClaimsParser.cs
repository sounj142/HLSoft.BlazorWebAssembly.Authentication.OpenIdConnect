using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Client.IdentityServer.Code.Complex.Auth
{
	public class CustomClaimsParser : IClaimsParser<User>
	{
		public IIdentity CreateIdentity(User user)
		{
			var claims = new List<Claim>();

			GenerateClaims(claims, user);
			GenerateClaims(claims, user?.profile);
			GenerateClaims(claims, user?.profile?.address);

			return claims.Count == 0
				? new ClaimsIdentity()
				: new ClaimsIdentity(claims, "Bearer");
		}

		private void GenerateClaims(List<Claim> claims, User user)
		{
			if (user == null) return;
			claims.Add(new Claim("access_token", user.access_token));
			claims.Add(new Claim("expires_at", user.expires_at.ToString()));
			claims.Add(new Claim("id_token", user.id_token));
			claims.Add(new Claim("session_state", user.session_state));
			claims.Add(new Claim("token_type", user.token_type));
			claims.AddRange(user.scope.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Select(scope => new Claim("scope", scope)));
		}

		private void GenerateClaims(List<Claim> claims, Profile profile)
		{
			if (profile == null) return;
			if (profile.api_role != null)
			{
				claims.AddRange(profile.api_role.Select(role => new Claim("api_role", role)));
			}
			claims.Add(new Claim("email", profile.email));
			claims.Add(new Claim("email_verified", profile.email_verified.ToString()));
			claims.Add(new Claim("idp", profile.idp));
			claims.Add(new Claim("name", profile.name));
			claims.Add(new Claim("sid", profile.sid));
			claims.Add(new Claim("sub", profile.sub));
			claims.Add(new Claim("website", profile.website));

			// add a special claim for User.Identity.Name
			claims.Add(new Claim(ClaimTypes.Name, profile.name));
		}

		private void GenerateClaims(List<Claim> claims, Address address)
		{
			if (address == null) return;
			claims.Add(new Claim("postal_code", address.postal_code.ToString()));
			claims.Add(new Claim("street_address", address.street_address));
		}
	}
}
