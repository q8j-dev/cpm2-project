using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Connection;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CodeStage.AntiCheat.Storage;
using CommonUtility.Contracts;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Errors;
using Firebase.Functions;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Backend.Session;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.PrefsKeys;

namespace CPM.Account
{
	public class FirebaseAccountController : IAccountController
	{
		public const string CreateNewAccountFn = "CreateNewAccount24_1";

		private readonly HashSet<IAuthorizationDependant> authorizationDependants;

		private List<IAuthorizationFailureDependant> _authorizationFailureDependants;

		private readonly HashSet<IFirstSignInDependant> firstSignInDependants;

		private ServiceAvailability serviceAvailability = ServiceAvailability.Unknown;

		private FirebaseUser firebaseUser;

		private readonly IUserAccountService userAccountService;

		private readonly PlayerDataStorage playerDataStorage;

		private readonly PeriodicalActionsUtility periodicalActionsUtility;

		private readonly string recheckFn = "recheckDependencies";

		private readonly string checkAccountFn = "CheckAccount24_1";

		private readonly IRemoteConfigManager _remoteConfigManager;

		private readonly FirebaseWrappedFunctionFactory _factory = new FirebaseWrappedFunctionFactory();

		private readonly IPrefsSaversGetter _prefsSaversGetter;

		private readonly IDeleterUserDataGetter _deleterUserDataGetter;

		private readonly IConnectionManagerBase _connectionManagerBase;

		private readonly SessionBackend _sessionBackend;

		private float _startProgress;

		private bool _isSteamSession;

		public bool Started;

		public bool IsSteamSession => _isSteamSession;

		public string FirebaseUserId => firebaseUser?.UserId;

		public string Email
		{
			get
			{
				return ManagerKeysForPrefs.Email;
			}
			private set
			{
				ManagerKeysForPrefs.Email = value;
			}
		}

		public ulong CreationTimestamp
		{
			get
			{
				if (ManagerKeysForPrefs.CreationTimestamp == 0L && firebaseUser != null)
				{
					ManagerKeysForPrefs.CreationTimestamp = firebaseUser.Metadata.CreationTimestamp;
				}
				return ManagerKeysForPrefs.CreationTimestamp;
			}
			set
			{
				ManagerKeysForPrefs.CreationTimestamp = value;
			}
		}

		public bool HasEmail
		{
			get
			{
				if (Email != "0")
				{
					return Email != "";
				}
				return false;
			}
		}

		public bool IsAnonymous
		{
			get
			{
				firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
				if (firebaseUser == null)
				{
					return false;
				}
				return firebaseUser.IsAnonymous;
			}
		}

		private string Password
		{
			get
			{
				return ManagerKeysForPrefs.PasswordAccount;
			}
			set
			{
				ManagerKeysForPrefs.PasswordAccount = value;
			}
		}

		public AccountType AccountType
		{
			get
			{
				if (HasEmail)
				{
					if (!AccountSystemV2Extension.IsGuestAccount(Email))
					{
						return AccountType.Email;
					}
					return AccountType.NewSystemGuest;
				}
				if (!IsAnonymous)
				{
					return AccountType.OldGuest;
				}
				return AccountType.OldAnonymous;
			}
		}

		public bool Authorized { get; private set; }

		public string LastSignInError => userAccountService.LastSignInError;

		public event Action OnForceLogout;

		public event Action OnSignInAnon;

		public void RegisterAuthorizationDependant(IAuthorizationDependant dependant)
		{
			authorizationDependants.Add(dependant);
		}

		public void UnregisterAuthorizationDependant(IAuthorizationDependant dependant)
		{
			authorizationDependants.Remove(dependant);
		}

		[Inject]
		public FirebaseAccountController(IEnumerable<IAuthorizationDependant> authorizationDependants, IEnumerable<IAuthorizationFailureDependant> authorizationFailureDependants, IEnumerable<IFirstSignInDependant> firstSignInDependants, CoroutineHelper coroutineHelper, PlayerDataStorage playerDataStorage, IRemoteConfigManager remoteConfigManager, IPrefsSaversGetter prefsSaversGetter, IDeleterUserDataGetter deleterUserDataGetter, IConnectionManagerBase connectionManagerBase, SessionBackend sessionBackend)
		{
			_authorizationFailureDependants = authorizationFailureDependants.ToList();
			this.playerDataStorage = playerDataStorage;
			_remoteConfigManager = remoteConfigManager;
			this.firstSignInDependants = firstSignInDependants.ToHashSet();
			this.authorizationDependants = authorizationDependants.ToHashSet();
			userAccountService = new FirebaseUserAccountService(this);
			PeriodicalActionsUtility.RefreshData refreshData = new PeriodicalActionsUtility.RefreshData(recheckFn, 5f, () =>
			{
				RecheckDependencies().Forget();
			});
			periodicalActionsUtility = new PeriodicalActionsUtility(coroutineHelper, refreshData);
			_prefsSaversGetter = prefsSaversGetter;
			_deleterUserDataGetter = deleterUserDataGetter;
			_connectionManagerBase = connectionManagerBase;
			_sessionBackend = sessionBackend;
		}

		public async UniTask<bool> HaveGuestAccount()
		{
			return await userAccountService.HaveGuestAccount();
		}

		public async UniTask<bool> SignIn(string email, string password, bool needSignOut = false, bool resetData = true)
		{
			_ = 2;
			try
			{
				if (await GetServiceAvailability() != ServiceAvailability.Available)
				{
					return false;
				}
				(bool IsTimeout, FirebaseUser Result) result = await userAccountService.SignIn(email, password, resetData).TimeoutWithoutException(TimeSpan.FromSeconds(15.0));
				if (!result.IsTimeout && result.Result != null)
				{
					if (result.Result != firebaseUser)
					{
						if (needSignOut)
						{
							await ResetData(reloadScene: false);
						}
						UpdatePlayerAccountID(result.Result.UserId);
						Email = email;
						Password = password;
						OnAuthenticationComplete(email != "").Forget();
						_isSteamSession = false;
						if (!AccountSystemV2Extension.IsGuestAccount(email))
						{
							ManagerKeysForPrefs.LastLoginMethod = LoginMethod.Email;
						}
					}
				}
				else
				{
					OnAuthenticationFailure().Forget();
				}
				firebaseUser = result.Result;
				return firebaseUser != null;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return false;
			}
		}

		public async UniTask<bool> SignInGuest()
		{
			return await SignIn(AccountSystemV2Extension.GuestEmail, AccountSystemV2Extension.GuestPassword);
		}

		public async UniTask<bool> SignInWithPrefs()
		{
			if (_isSteamSession)
			{
				return true;
			}
			Debug.Log("Checking sign in");
			if (await GetServiceAvailability() != ServiceAvailability.Available)
			{
				return false;
			}
			Debug.Log("Sign in required");
			FirebaseUser firebaseUser = await userAccountService.SignIn(Email, Password, resetData: false);
			if (firebaseUser != null)
			{
				Debug.Log(" sign in result not null");
				if (firebaseUser != this.firebaseUser)
				{
					Debug.Log(" sign in result not firebase user");
					UpdatePlayerAccountID(firebaseUser.UserId);
					OnAuthenticationComplete().Forget();
				}
			}
			else
			{
				OnAuthenticationFailure().Forget();
			}
			this.firebaseUser = firebaseUser;
			return this.firebaseUser != null;
		}

		public async UniTask<bool> SignInWithSteamToken(string customToken, bool isNewAccount, string backendEmail = null, bool skipAuthComplete = false)
		{
			try
			{
				if (await GetServiceAvailability() != ServiceAvailability.Available)
				{
					return false;
				}
				return await (await FirebaseAuth.DefaultInstance.SignInWithCustomToken(customToken)).Match((Func<FirebaseUser, UniTask<bool>>)(async (FirebaseUser user) =>
				{
					firebaseUser = user;
					UpdatePlayerAccountID(user.UserId);
					_isSteamSession = true;
					if (!string.IsNullOrEmpty(backendEmail) && !AccountSystemV2Extension.IsSteamOnlyAccount(backendEmail))
					{
						Email = backendEmail;
					}
					else
					{
						Email = "steam_authorized";
					}
					Started = true;
					try
					{
						if (skipAuthComplete)
						{
							Authorized = true;
						}
						else if (isNewAccount)
						{
							await OnAuthenticationComplete(firstEnter: true);
							HandleDeviceLogin().Forget();
						}
						else
						{
							await OnAuthenticationComplete();
							HandleDeviceLogin().Forget();
						}
					}
					catch (Exception)
					{
					}
					ManagerKeysForPrefs.IsSteamLinked = true;
					return true;
				}), (Func<SignInError, UniTask<bool>>)((SignInError error) => UniTask.FromResult(value: false)));
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void ResetSteamSession()
		{
			_isSteamSession = false;
		}

		private async UniTask OnAuthenticationFailure()
		{
			Debug.Log("Authentication failure");
			foreach (IAuthorizationFailureDependant authorizationFailureDependant in _authorizationFailureDependants)
			{
				authorizationFailureDependant.OnAuthorizationFailure();
			}
		}

		public async UniTask<SignUpData> SignUp(string email, string password, bool needSignOut = false, bool resetData = true)
		{
			FirebaseWrappedFunction firebaseWrappedFunction = _factory.Create("CreateNewAccount24_1");
			CreateAccountPayload data = new CreateAccountPayload
			{
				Email = email,
				Password = password
			};
			FunctionCallResult<CreateAccountResult> createAccountResult = await firebaseWrappedFunction.CallAsyncResult<CreateAccountResult>(data);
			if (!createAccountResult.IsSuccess)
			{
				return new SignUpData
				{
					result = SignUpResult.ErrorAccountDontCreated,
					firebaseUser = firebaseUser,
					localId = ""
				};
			}
			bool signInResult = false;
			int retryCount = 3;
			while (!signInResult && retryCount > 0)
			{
				retryCount--;
				signInResult = await SignIn(email, password, needSignOut, resetData);
				if (!signInResult)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(1.0));
				}
			}
			LoginInProgressContainer.IsLoginInProgress = false;
			if (!signInResult)
			{
				return new SignUpData
				{
					result = SignUpResult.ErrorCantSignIn,
					firebaseUser = firebaseUser,
					localId = ""
				};
			}
			if (createAccountResult.Data != null)
			{
				playerDataStorage.PlayerID = createAccountResult.Data.LocalId;
			}
			return new SignUpData
			{
				result = SignUpResult.Success,
				firebaseUser = firebaseUser,
				localId = createAccountResult.Data?.LocalId
			};
		}

		public async UniTask<SignUpData> SignUpGuest()
		{
			return await SignUp(AccountSystemV2Extension.GuestEmail, AccountSystemV2Extension.GuestPassword, needSignOut: false, resetData: false);
		}

		public async UniTask<SignUpData> ChangeEmailPassword(string email, string password)
		{
			SignUpData data = await userAccountService.ChangeEmailPassword(email, password);
			if (data.result != SignUpResult.Success)
			{
				return data;
			}
			Email = email;
			Password = password;
			firebaseUser = data.firebaseUser;
			await OnAuthenticationComplete();
			return data;
		}

		public void SignOut(bool loadingSceneSecond = false, bool forceClearData = false)
		{
			firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
			string text = ((firebaseUser == null) ? "IS NULL. NORMAL.  " : "IS NOT NULL. Be Patient!");
			Debug.LogWarning("firebaseUser: " + text);
			if (firebaseUser != null)
			{
				if (IsAnonymous)
				{
					Debug.LogError("HandleDeviceLogout for Anon user. TELL DEVS ABOUT IT!");
				}
				_sessionBackend.EndSessionAsync().Forget();
				userAccountService.SignOut();
			}
			OnUserSignOut(forceClearData, loadingSceneSecond);
		}

		public void OnUserSignOut(bool forceClearData = false, bool loadingSceneSecond = false)
		{
			_isSteamSession = false;
			ManagerKeysForPrefs.IsSteamLinked = false;
			Debug.Log("Firebase user: " + firebaseUser?.ToString() + " has email: " + HasEmail);
			if (firebaseUser != null)
			{
				ClearDataAndRestart(loadingSceneSecond);
				Debug.Log("Reload scene 0");
				foreach (IFirstSignInDependant firstSignInDependant in firstSignInDependants)
				{
					firstSignInDependant.OnSignOut();
				}
			}
			else if (forceClearData)
			{
				ClearDataAndRestart(loadingSceneSecond);
				foreach (IFirstSignInDependant firstSignInDependant2 in firstSignInDependants)
				{
					firstSignInDependant2.OnSignOut();
				}
				Debug.Log("Reload scene 0 forced");
			}
			IAuthorizationDependant[] array = authorizationDependants.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnSignOut();
			}
		}

		public void ClearDataAndRestart(bool loadingSceneSecond = false)
		{
			DataUtilities.DeleteAllUserData(_prefsSaversGetter.GetPrefsSavers(), _deleterUserDataGetter.GetDeleteUserDatas());
			playerDataStorage.ResetValues();
			firebaseUser = null;
			Authorized = false;
			ManagerKeysForPrefs.IsSteamLinked = false;
			SceneManager.LoadScene(loadingSceneSecond ? 34 : 0);
		}

		public async UniTask ResetData(bool reloadScene)
		{
			Debug.Log("Reset player data before log in.");
			DataUtilities.DeleteAllUserData(_prefsSaversGetter.GetPrefsSavers(), _deleterUserDataGetter.GetDeleteUserDatas());
			playerDataStorage.ResetValues();
			firebaseUser = null;
			Authorized = false;
			foreach (IFirstSignInDependant firstSignInDependant in firstSignInDependants)
			{
				firstSignInDependant.OnSignOut();
			}
			if (reloadScene)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(5.0));
				Debug.LogWarning("Reload scene (R)1");
				SceneManager.LoadScene(0);
			}
		}

		public async UniTask<ServiceAvailability> GetServiceAvailability()
		{
			if (serviceAvailability == ServiceAvailability.Unknown)
			{
				await CheckDependencies();
				if (serviceAvailability == ServiceAvailability.Unavailable)
				{
					periodicalActionsUtility.StartRefresh(recheckFn);
				}
			}
			while (serviceAvailability == ServiceAvailability.Pending)
			{
				await UniTask.Delay(10);
			}
			Debug.Log($"Firebase availability: {serviceAvailability}");
			return serviceAvailability;
		}

		private async UniTaskVoid RecheckDependencies()
		{
			await CheckDependencies();
			if (serviceAvailability == ServiceAvailability.Available)
			{
				periodicalActionsUtility.StopAll();
			}
		}

		private async UniTask CheckDependencies()
		{
			serviceAvailability = ServiceAvailability.Pending;
			serviceAvailability = ((await userAccountService.CheckDependencies()) ? ServiceAvailability.Available : ServiceAvailability.Unavailable);
		}

		private void UpdatePlayerAccountID(string accountID = "")
		{
			if (accountID == "")
			{
				accountID = ManagerKeysForPrefs.AccountId;
				Debug.Log("Setting saved account ID:" + accountID);
				playerDataStorage.AccountID = accountID;
			}
			else
			{
				Debug.Log("Setting account ID: " + accountID);
				playerDataStorage.AccountID = accountID;
				ManagerKeysForPrefs.AccountId = accountID;
			}
		}

		private async UniTask OnAuthenticationComplete(bool firstEnter = false)
		{
			Debug.Log("OnAuthenticationComplete");
			if (firstEnter)
			{
				LoginInProgressContainer.IsLoginInProgress = true;
			}
			Authorized = true;
			IAuthorizationDependant[] array = authorizationDependants.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnSuccessfulAuthorization().Forget();
			}
			if (!firstEnter)
			{
				return;
			}
			List<UniTask> list = new List<UniTask>();
			foreach (IFirstSignInDependant firstSignInDependant in firstSignInDependants)
			{
				list.Add(firstSignInDependant.OnFirstSignIn());
			}
			await UniTask.WhenAll(list);
		}

		public async UniTask<bool> ResetPassword(string newEmail)
		{
			return await userAccountService.ResetPassword(newEmail);
		}

		public async UniTask DeleteAccount()
		{
			await userAccountService.DeleteAccount();
			if (firebaseUser != null)
			{
				Debug.Log("firebaseUser != null Signing out");
				userAccountService.SignOut();
			}
			OnUserSignOut(forceClearData: true);
			ObscuredPrefs.SetInt("RewardedShowed", 0);
		}

		public async UniTask<bool> VerifyEmail()
		{
			await SignInWithPrefs();
			firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
			if (firebaseUser == null)
			{
				return false;
			}
			return await firebaseUser.SendEmailVerificationAsync();
		}

		public bool IsUserVerified()
		{
			firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
			if (firebaseUser != null)
			{
				return firebaseUser.IsEmailVerified;
			}
			return false;
		}

		public async UniTask<bool> CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType type)
		{
			if (!Authorized || IsAnonymous)
			{
				return false;
			}
			int num = (int)type;
			_ = new string[2]
			{
				SystemInfo.deviceUniqueIdentifier,
				num.ToString()
			};
			Result<bool> result = await _sessionBackend.ExtendSessionAsync();
			if (result.IsFail || result.Value)
			{
				return false;
			}
			Debug.LogError("OTHER USER FOUND, FORCE LOGOUT");
			await AnotherDeviceFound();
			return true;
		}

		private async UniTask AnotherDeviceFound()
		{
			try
			{
				OnForceLogout?.Invoke();
				await UniTask.Delay(TimeSpan.FromSeconds(3.0));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			ManagerKeysForPrefs.KickedByAnotherDevice = true;
			ObscuredPrefs.Save();
			SignOut();
		}

		public UniTask HandleDeviceLogin()
		{
			return _sessionBackend.StartSessionAsync();
		}
	}
}
