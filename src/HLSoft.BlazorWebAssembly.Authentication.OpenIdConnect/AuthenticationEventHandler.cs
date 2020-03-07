using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
{
	public class AuthenticationEventHandler
	{
		public event EventHandler<string> SignInFailEvent;
		public event EventHandler<string> SignOutFailEvent;
		public event EventHandler<string> SilentRefreshTokenFailEvent;
		public event EventHandler SignInSuccessEvent;
		public event EventHandler SignOutSuccessEvent;

		public readonly OpenIdConnectOptions _openIdConnectOptions;

		public AuthenticationEventHandler(OpenIdConnectOptions openIdConnectOptions)
		{
			_openIdConnectOptions = openIdConnectOptions;

			DotNetEndPoint.Initialize(this);
		}

		public void NotifySignInFail(Exception err)
		{
			ProcessFail(err, SignInFailEvent);
		}

		public void NotifySignOutFail(Exception err)
		{
			ProcessFail(err, SignOutFailEvent);
		}

		public void NotifySilentRefreshTokenFail(Exception err)
		{
			ProcessFail(err, SilentRefreshTokenFailEvent);
		}

		public void NotifySignInSuccess()
		{
			ProcessSuccess(SignInSuccessEvent);
		}

		public void NotifySignOutSuccess()
		{
			ProcessSuccess(SignOutSuccessEvent);
		}

		private void ProcessFail(Exception err, EventHandler<string> eventHandler)
		{
			var errorMsg = err.Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.FirstOrDefault()
					?.Trim();
			if (_openIdConnectOptions.WriteErrorToConsole)
			{
				Console.Error.WriteLine(err);
			}
			Task.Run(() =>
			{
				eventHandler?.Invoke(this, errorMsg);
			});
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
