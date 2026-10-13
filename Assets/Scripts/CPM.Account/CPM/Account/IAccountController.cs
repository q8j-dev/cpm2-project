using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.AccountSystemNew;

namespace CPM.Account
{
	[Preserve]
	public interface IAccountController
	{
		string FirebaseUserId { get; }

		AccountType AccountType { get; }

		string Email { get; }

		ulong CreationTimestamp { get; }

		string LastSignInError { get; }

		bool Authorized { get; }

		bool HasEmail { get; }

		bool IsAnonymous { get; }

		bool IsSteamSession { get; }

		event Action OnForceLogout;

		event Action OnSignInAnon;

		void ClearDataAndRestart(bool loadingSceneSecond = false);

		UniTask<ServiceAvailability> GetServiceAvailability();

		UniTask<bool> SignInWithPrefs();

		UniTask<bool> SignIn(string email, string password, bool needSignOut = false, bool resetData = true);

		UniTask<bool> SignInGuest();

		UniTask<SignUpData> SignUp(string email, string password, bool needSignOut = false, bool resetData = true);

		UniTask<SignUpData> SignUpGuest();

		UniTask<SignUpData> ChangeEmailPassword(string email, string password);

		void SignOut(bool loadingSceneSecond = false, bool forceClearData = false);

		UniTask<bool> ResetPassword(string newEmail);

		UniTask DeleteAccount();

		UniTask<bool> VerifyEmail();

		bool IsUserVerified();

		UniTask ResetData(bool reloadScene);

		UniTask<bool> CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType type);

		void OnUserSignOut(bool forceClearData = false, bool loadingSceneSecond = false);

		UniTask HandleDeviceLogin();

		UniTask<bool> HaveGuestAccount();

		UniTask<bool> SignInWithSteamToken(string customToken, bool isNewAccount, string backendEmail = null, bool skipAuthComplete = false);

		void ResetSteamSession();

		void RegisterAuthorizationDependant(IAuthorizationDependant dependant);

		void UnregisterAuthorizationDependant(IAuthorizationDependant dependant);
	}
}
