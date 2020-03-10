using Microsoft.AspNetCore.Components;
using System;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    public static class CommonExtensions
    {
		public static string GetAbsoluteUri(this NavigationManager navigationManager, string uri)
		{
			if (uri == null) return null;
			return uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
				? uri
				: navigationManager.ToAbsoluteUri(uri).AbsoluteUri;
		}
	}
}
