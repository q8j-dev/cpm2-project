using System;
using CPM.Account;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Steam.Base
{
	public interface ISteamAccountService
	{
		UniTask<string> GetAuthSessionTicketAsync();

		UniTask<SteamAuthResponse> CheckSteamIdAsync();

		UniTask<SteamAuthResponse> StartWithSteamIDAsync();

		void LogSuccessfulLogin();

		UniTask AssignSteamId(string currentEmail, Action callback);

		UniTask<bool> RefreshSteamLinkAsync(string currentEmail);

		UniTask RevokeSteamId(Action callback);

		UniTask<bool> TrySignInWithSteamAsync(IAccountController accountController, bool withoutIsNew = false, bool skipAuthComplete = false);

		UniTask<SteamSignInOutcome> TrySignInWithSteamDetailedAsync(IAccountController accountController, bool withoutIsNew = false, bool skipAuthComplete = false);
	}
}
