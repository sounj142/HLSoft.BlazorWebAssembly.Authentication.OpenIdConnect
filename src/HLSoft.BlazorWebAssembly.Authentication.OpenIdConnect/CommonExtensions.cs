using Microsoft.AspNetCore.Components;
using System;

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
	}
}
