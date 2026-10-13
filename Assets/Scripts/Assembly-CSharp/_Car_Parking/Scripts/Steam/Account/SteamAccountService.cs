using System;
using CPM.Account;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using Steamworks;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam.Account
{
	public class SteamAccountService : ISteamAccountService
	{
		private const string CHECK_STEAM_ID = "CheckSteamId24_1";

		private const string START_WITH_STEAM_ID = "StartWithSteamId24_1";

		private const string ASSIGN_STEAM_ID = "AssignSteamId24_1";

		private const string REVOKE_STEAM_ID = "RevokeSteamId24_1";

		private ServerTimeService serverTimeService;

		private ProfileAnalytic profileAnalytic;

		private AccountLinkAnalytic accountLinkAnalytic;

		public SteamAccountService(ServerTimeService serverTimeService, ProfileAnalytic profileAnalytic, AccountLinkAnalytic accountLinkAnalytic)
		{
			this.serverTimeService = serverTimeService;
			this.profileAnalytic = profileAnalytic;
			this.accountLinkAnalytic = accountLinkAnalytic;
		}

		public async UniTask<SteamAuthResponse> CheckSteamIdAsync()
		{
			string text = await GetAuthSessionTicketAsync();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return await CheckSteamIdAsync(text);
		}

		private async UniTask<SteamAuthResponse> CheckSteamIdAsync(string ticketHex)
		{
			FirebaseWrappedFunction function = new FirebaseWrappedFunctionFactory().Create("CheckSteamId24_1");
			SteamAuthResponse steamAuthResponse = await CallSteamFunctionAsync(function, ticketHex);
			if (steamAuthResponse != null && string.IsNullOrEmpty(steamAuthResponse.CustomToken))
			{
				return null;
			}
			return steamAuthResponse;
		}

		public async UniTask<bool> TrySignInWithSteamAsync(IAccountController accountController, bool withoutIsNew = false, bool skipAuthComplete = false)
		{
			return await TrySignInWithSteamDetailedAsync(accountController, withoutIsNew, skipAuthComplete) == SteamSignInOutcome.SignedIn;
		}

		public async UniTask<SteamSignInOutcome> TrySignInWithSteamDetailedAsync(IAccountController accountController, bool withoutIsNew = false, bool skipAuthComplete = false)
		{
			if (!SteamManager.IsInitialized())
			{
				return SteamSignInOutcome.SteamUnavailable;
			}
			string text = await GetAuthSessionTicketAsync();
			if (string.IsNullOrEmpty(text))
			{
				return SteamSignInOutcome.SteamUnavailable;
			}
			SteamAuthResponse steamAuthResponse = await CheckSteamIdAsync(text);
			bool flag;
			if (withoutIsNew)
			{
				if (steamAuthResponse == null || steamAuthResponse.CustomToken == "0" || steamAuthResponse.IsNew == 1)
				{
					return SteamSignInOutcome.NotLinked;
				}
				flag = await accountController.SignInWithSteamToken(steamAuthResponse.CustomToken, isNewAccount: false, steamAuthResponse.Email, skipAuthComplete);
				if (flag)
				{
					LogSuccessfulLogin();
				}
			}
			else
			{
				if (steamAuthResponse == null || steamAuthResponse.CustomToken == "0")
				{
					return SteamSignInOutcome.NotLinked;
				}
				bool flag2 = !ManagerKeysForPrefs.HasAccountId || ManagerKeysForPrefs.AccountId == "0";
				bool isNewAccount = (steamAuthResponse.IsNew == 1) | flag2;
				flag = await accountController.SignInWithSteamToken(steamAuthResponse.CustomToken, isNewAccount, steamAuthResponse.Email, skipAuthComplete);
				if (flag)
				{
					LogSuccessfulLogin();
				}
			}
			return (!flag) ? SteamSignInOutcome.NotLinked : SteamSignInOutcome.SignedIn;
		}

		public async UniTask<bool> RefreshSteamLinkAsync(string currentEmail)
		{
			if (!SteamManager.IsInitialized())
			{
				return ManagerKeysForPrefs.IsSteamLinked;
			}
			SteamAuthResponse steamAuthResponse = await CheckSteamIdAsync();
			return ManagerKeysForPrefs.IsSteamLinked = ((!AccountSystemV2Extension.IsSteamOnlyAccount(currentEmail)) ? (steamAuthResponse != null && !string.IsNullOrEmpty(steamAuthResponse.Email) && string.Equals(steamAuthResponse.Email, currentEmail, StringComparison.OrdinalIgnoreCase)) : (steamAuthResponse != null && steamAuthResponse.CustomToken != "0"));
		}

		public async UniTask<SteamAuthResponse> StartWithSteamIDAsync()
		{
			string text = await GetAuthSessionTicketAsync();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			FunctionCallResult<string> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("StartWithSteamId24_1").CallAsyncResult<string>(text);
			switch (functionCallResult.Status)
			{
			case FunctionStatus.SteamIDAssignedAlready:
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.AlreadyAssignedToDifferentProfile"), 3f).Forget();
				return null;
			case FunctionStatus.SteamIDAssignedAlreadyDifferent:
			case FunctionStatus.SteamIDOwnedByOther:
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.DifferentProfileAlreadyHasBeenAssignedWithSteamID"), 3f).Forget();
				return null;
			default:
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("SteamError.VerificationFailed"), 3f, NotificationType.Error).Forget();
				return null;
			case FunctionStatus.Success:
			{
				if (string.IsNullOrEmpty(functionCallResult.Data))
				{
					return null;
				}
				SteamAuthResponse steamAuthResponse;
				try
				{
					steamAuthResponse = JsonConvert.DeserializeObject<SteamAuthResponse>(functionCallResult.Data);
				}
				catch (Exception)
				{
					return null;
				}
				if (steamAuthResponse != null && string.IsNullOrEmpty(steamAuthResponse.CustomToken))
				{
					return null;
				}
				return steamAuthResponse;
			}
			}
		}

		private async UniTask<SteamAuthResponse> CallSteamFunctionAsync(FirebaseWrappedFunction function, string ticketHex)
		{
			FunctionCallResult<string> functionCallResult = await function.CallAsyncResult<string>(ticketHex);
			if (!functionCallResult.IsSuccess || string.IsNullOrEmpty(functionCallResult.Data))
			{
				if (functionCallResult.Status != FunctionStatus.SteamAccountNotProvisioned)
				{
					_ = functionCallResult.Status;
					_ = 301;
				}
				return null;
			}
			try
			{
				return JsonConvert.DeserializeObject<SteamAuthResponse>(functionCallResult.Data);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void LogSuccessfulLogin()
		{
			ulong value = SteamClient.SteamId.Value;
			ManagerKeysForPrefs.LinkedSteamId = value.ToString();
			ManagerKeysForPrefs.LastLoginMethod = LoginMethod.Steam;
			SaveLoginTime();
			profileAnalytic.LogLogin(LoginStatus.LOGIN_COMPLETED, value.ToString(), "steam");
		}

		private void SaveLoginTime()
		{
			try
			{
				ES3.Save("loginTime", serverTimeService.CurrentServerUnixTimeSec);
			}
			catch (Exception)
			{
				try
				{
					ES3.DeleteFile();
					ES3.Save("loginTime", serverTimeService.CurrentServerUnixTimeSec);
				}
				catch (Exception)
				{
				}
			}
		}

		public async UniTask<string> GetAuthSessionTicketAsync()
		{
			if (!SteamManager.IsInitialized())
			{
				return null;
			}
			try
			{
				AuthTicket authTicket = await SteamUser.GetAuthTicketForWebApiAsync("CPM2");
				if (authTicket == null || authTicket.Data == null || authTicket.Data.Length == 0)
				{
					return null;
				}
				return BitConverter.ToString(authTicket.Data).Replace("-", "");
			}
			catch (Exception)
			{
				return null;
			}
		}

		public async UniTask AssignSteamId(string currentEmail, Action callback)
		{
			string steamId = SteamManager.GetUserIdString();
			string steamAccountId = SteamManager.GetUserAccountId().ToString();
			string steamLanguage = SteamManager.GetSteamLanguage();
			int steamLevel = SteamManager.GetSteamLevel();
			accountLinkAnalytic.LogAccountLink(AccountLinkStatus.LINK_STARTED, steamId, steamAccountId, steamLanguage, steamLevel);
			V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true, backButtonState: true).Forget();
			string text = await GetAuthSessionTicketAsync();
			if (string.IsNullOrEmpty(text))
			{
				accountLinkAnalytic.LogAccountLink(AccountLinkStatus.LINK_FAILED, steamId, steamAccountId, steamLanguage, steamLevel, "no_auth_ticket");
				return;
			}
			FunctionCallResult<int> response = await new FirebaseWrappedFunctionFactory().Create("AssignSteamId24_1").CallAsyncResult<int>(text);
			V.CoreGameManager.ManagerUI.CloseWaitingWindow();
			switch (response.Status)
			{
			case FunctionStatus.Success:
				ManagerKeysForPrefs.IsSteamLinked = true;
				ManagerKeysForPrefs.LinkedSteamId = SteamClient.SteamId.Value.ToString();
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.AccountLinkedToSteamProfile"), 3f).Forget();
				callback();
				break;
			case FunctionStatus.SteamIDAssignedAlready:
				if (await RefreshSteamLinkAsync(currentEmail))
				{
					callback();
				}
				else
				{
					V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.AlreadyAssignedToDifferentProfile"), 3f).Forget();
				}
				break;
			case FunctionStatus.SteamIDAssignedAlreadyDifferent:
			case FunctionStatus.SteamIDOwnedByOther:
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.DifferentProfileAlreadyHasBeenAssignedWithSteamID"), 3f).Forget();
				break;
			default:
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("SteamError.VerificationFailed"), 3f, NotificationType.Error).Forget();
				break;
			}
			if (response.Status == FunctionStatus.Success)
			{
				accountLinkAnalytic.LogAccountLink(AccountLinkStatus.LINK_COMPLETED, steamId, steamAccountId, steamLanguage, steamLevel);
			}
			else
			{
				accountLinkAnalytic.LogAccountLink(AccountLinkStatus.LINK_FAILED, steamId, steamAccountId, steamLanguage, steamLevel, response.Status.ToString());
			}
		}

		public async UniTask RevokeSteamId(Action callback)
		{
			V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true, backButtonState: true).Forget();
			string text = await GetAuthSessionTicketAsync();
			if (!string.IsNullOrEmpty(text))
			{
				FunctionCallResult<int> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("RevokeSteamId24_1").CallAsyncResult<int>(text);
				V.CoreGameManager.ManagerUI.CloseWaitingWindow();
				switch (functionCallResult.Status)
				{
				case FunctionStatus.Success:
					ManagerKeysForPrefs.IsSteamLinked = false;
					ManagerKeysForPrefs.LinkedSteamId = "0";
					ManagerKeysForPrefs.LastLoginMethod = LoginMethod.Email;
					V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.AccountUnlinkedFromSteam"), 3f).Forget();
					callback();
					break;
				case FunctionStatus.SteamIDNotLinked:
					ManagerKeysForPrefs.IsSteamLinked = false;
					ManagerKeysForPrefs.LinkedSteamId = "0";
					ManagerKeysForPrefs.LastLoginMethod = LoginMethod.Email;
					V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.NoAccountLinkedWasFound"), 3f).Forget();
					callback();
					break;
				default:
					V.CoreGameManager.ManagerUI.ShowNotificationWindow("Can not unlink Steam", 3f, NotificationType.Error).Forget();
					break;
				}
				if (functionCallResult.Status == FunctionStatus.Success || functionCallResult.Status == FunctionStatus.SteamIDNotLinked)
				{
					accountLinkAnalytic.LogAccountLink(AccountLinkStatus.UNLINKED, SteamManager.GetUserIdString(), SteamManager.GetUserAccountId().ToString(), SteamManager.GetSteamLanguage(), SteamManager.GetSteamLevel());
				}
			}
		}
	}
}
