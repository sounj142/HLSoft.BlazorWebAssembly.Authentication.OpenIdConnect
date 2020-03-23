namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
    public static class AuthConfig
    {
        /// <summary>
		/// automatically hide the body element when displaying the authentication pages
		/// </summary>
		public static bool AutomaticHideBodyWhenAuthenticating { get; set; } = true;
		/// <summary>
		/// Whether write the processing error to Console, should enable only in Devlopment
		/// Default: true
		/// </summary>
		public static bool WriteErrorToConsole { get; set; } = true;
	}
}
