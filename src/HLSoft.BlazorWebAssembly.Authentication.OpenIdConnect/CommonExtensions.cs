using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    public static class CommonExtensions
    {
		/// <summary>
		/// check if the uri is reletive, convert it to absolute otherwise keep it as-is
		/// </summary>
		public static string GetAbsoluteUri(this NavigationManager navigationManager, string uri)
		{
			if (uri == null) return null;
			return uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
				? uri
				: navigationManager.ToAbsoluteUri(uri).AbsoluteUri;
		}

		/// <summary>
		/// Get user identifier (claim name sub)
		/// </summary>
		public static string GetUserIdentifier(this AuthenticationState state)
		{
			return state.GetClaim("sub");
		}

		/// <summary>
		/// Get user display name
		/// </summary>
		public static string GetUserDisplayName(this AuthenticationState state)
		{
			return state.GetClaim(ClaimTypes.Name);
		}

		/// <summary>
		/// Get user email
		/// </summary>
		public static string GetUserEmail(this AuthenticationState state)
		{
			return state.GetClaim("email");
		}

		/// <summary>
		/// Get claim by name
		/// </summary>
		public static string GetClaim(this AuthenticationState state, string name)
		{
			return state?.User?.FindFirst(name)?.Value;
		}
	}
}
