using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public class DefaultClaimsParser : IClaimsParser<object>
    {
		private const int MAXIMUM_READ_CLAIM_LEVEL = 3;

		public virtual IIdentity CreateIdentity(object userClaims)
		{
			var claims = ParseClaims(userClaims);
			DetectClaimForIdentityName(claims);

			var tokenType = claims.FirstOrDefault(s => s.Type == "token_type");
			var claimsIdentity = claims.Count == 0
				? new ClaimsIdentity()
				: new ClaimsIdentity(claims, string.IsNullOrEmpty(tokenType?.Value) ? "Bearer" : tokenType.Value);
			return claimsIdentity;
		}

		protected virtual IList<Claim> ParseClaims(object userClaims)
		{
			var result = new List<Claim>();
			if (userClaims == null)
				return result;
			var claimsObj = (JsonElement)userClaims;
			if (claimsObj.ValueKind != JsonValueKind.Object)
				return result;

			ParseClaims(claimsObj, result, 1);

			return result;
		}

		private void ParseClaims(JsonElement jsonElem, IList<Claim> claims, int level)
		{
			foreach (var item in jsonElem.EnumerateObject())
			{
				switch (item.Value.ValueKind)
				{
					case JsonValueKind.Null:
					case JsonValueKind.Undefined:
						break;
					case JsonValueKind.Array:
						if (level < MAXIMUM_READ_CLAIM_LEVEL)
						{
							ParseArrayClaims(item, claims);
						}
						break;
					case JsonValueKind.Object:
						if (level < MAXIMUM_READ_CLAIM_LEVEL)
						{
							ParseClaims(item.Value, claims, level + 1);
						}
						break;
					default:
						claims.Add(new Claim(item.Name, item.Value.ToString()));
						break;
				}
			}
		}

		private void ParseArrayClaims(JsonProperty jsonElem, IList<Claim> claims)
		{
			foreach (var item in jsonElem.Value.EnumerateArray())
			{
				if (item.ValueKind == JsonValueKind.False || item.ValueKind == JsonValueKind.Number
					|| item.ValueKind == JsonValueKind.String || item.ValueKind == JsonValueKind.True)
				{
					claims.Add(new Claim(jsonElem.Name, item.ToString()));
				}
			}
		}

		protected virtual void DetectClaimForIdentityName(IList<Claim> claims)
		{
			if (claims == null || claims.Count == 0)
				return;
			if (claims.All(c => c.Type != ClaimTypes.Name))
			{
				var nameCandidate = claims.FirstOrDefault(c => c.Type == "name")
					?? claims.FirstOrDefault(c => c.Type == "given_name")
					?? claims.FirstOrDefault(c => c.Type == "email");
				if (nameCandidate != null)
				{
					claims.Add(new Claim(ClaimTypes.Name, nameCandidate.Value));
				}
			}
		}
	}
}
