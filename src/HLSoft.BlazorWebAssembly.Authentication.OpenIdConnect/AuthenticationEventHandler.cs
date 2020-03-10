using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public class AuthenticationEventHandler
	{
		/// <summary>
		/// Raise when an error happened when process the login workflow (both redirect and popup).
		/// </summary>
		public event EventHandler<string> SignInFailEvent;
		/// <summary>
		/// Raise when an error happened when process the logout workflow (both redirect and popup).
		/// </summary>
		public event EventHandler<string> SignOutFailEvent;
		/// <summary>
		/// Raised when the automatic silent renew has failed.
		/// </summary>
		public event EventHandler<string> SilentRefreshTokenFailEvent;
		/// <summary>
		/// Raise when the login workflow succeeded (both redirect and popup).
		/// </summary>
		public event EventHandler SignInSuccessEvent;
		/// <summary>
		/// Raise when the logout workflow succeeded (both redirect and popup).
		/// </summary>
		public event EventHandler SignOutSuccessEvent;

		private readonly OpenIdConnectOptions _openIdConnectOptions;

		public AuthenticationEventHandler(OpenIdConnectOptions openIdConnectOptions)
		{
			_openIdConnectOptions = openIdConnectOptions;

			DotNetEndPoint.Initialize(this);
		}
		/// <summary>
		/// Notify that the login process failed
		/// </summary>
		public void NotifySignInFail(Exception err)
		{
			ProcessFail(err, SignInFailEvent);
		}
		/// <summary>
		/// Notify that the logout process failed
		/// </summary>
		public void NotifySignOutFail(Exception err)
		{
			ProcessFail(err, SignOutFailEvent);
		}
		/// <summary>
		/// Notify that the silent refresh process failed
		/// </summary>
		public void NotifySilentRefreshTokenFail(Exception err)
		{
			ProcessFail(err, SilentRefreshTokenFailEvent);
		}
		/// <summary>
		/// Notify that the login process succeeded
		/// </summary>
		public void NotifySignInSuccess()
		{
			ProcessSuccess(SignInSuccessEvent);
		}
		/// <summary>
		/// Notify that the logout process succeeded
		/// </summary>
		public void NotifySignOutSuccess()
		{
			ProcessSuccess(SignOutSuccessEvent);
		}

		private bool IsConcernError(string errorMsg)
		{
			return errorMsg != "Popup window closed";
		}

		private void ProcessFail(Exception err, EventHandler<string> eventHandler)
		{
			var errorMsg = err.Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.FirstOrDefault()
					?.Trim();
			if (IsConcernError(errorMsg))
			{
				if (_openIdConnectOptions.WriteErrorToConsole)
				{
					Console.Error.WriteLine(err);
				}
				Task.Run(() =>
				{
					eventHandler?.Invoke(this, errorMsg);
				});
			}
		}

		private void ProcessSuccess(EventHandler eventHandler)
		{
			Task.Run(() =>
			{
				eventHandler?.Invoke(this, null);
			});
		}
	}
}
