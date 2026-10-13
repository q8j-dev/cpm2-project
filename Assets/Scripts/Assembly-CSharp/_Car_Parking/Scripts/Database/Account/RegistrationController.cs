using System;
using System.Threading;
using CPM.Account;
using CPM.Balance;
using CPM.Connection;
using CPM.Connection.Entities;
using CPM.Database;
using CPM.Scripts.Garage;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.Database.Account
{
	public class RegistrationController : IDatabaseOutputHandler, IRegistrationController
	{
		private const int TryCheckHashCount = 3;

		private const float DelayToCheckNextHash = 5f;

		private const float AutoSaveWaitTimeoutSec = 20f;

		private IAccountController _accountController;

		private DataBase _dataBase;

		private IPlayerStorageService _playerStorageService;

		private ProgressContainer _progressContainer;

		private IAccountDataProvider _accountDataProvider;

		private readonly FirebaseWrappedFunctionFactory _factory = new FirebaseWrappedFunctionFactory();

		private IConnectionManager _connectionManager;

		private PlayerPrefsTimer _playerPrefsTimer;

		private ProfileAnalytic _profileAnalytic;

		private ServerTimeService _serverTimeService;

		private UserPreferenceAnalytic _userPreferenceAnalytic;

		private BootStartData _bootStartData;

		private IGameDataRepository _gameDataRepository;

		private IConnectionDataHandler _connectionDataHandler;

		private OptionalDependency<GarageController> _garageControllerDependency;

		private OptionalDependency<IAutoSaveService> _autoSaveService;

		private IBalanceDocumentSynchronizer _balanceDocumentSynchronizer;

		private ISteamAccountService _steamAccountService;

		public bool FirstSignIn { get; set; }

		public DataBase DataBase => _dataBase;

		[Inject]
		public void Construct(IAccountController accountController, DataBase dataBase, IPlayerStorageService playerStorageService, IAccountDataProvider accountDataProvider, ProgressContainer progressContainer, IConnectionManager connectionManager, PlayerPrefsTimer playerPrefsTimer, ProfileAnalytic profileAnalytic, ServerTimeService serverTimeService, UserPreferenceAnalytic userPreferenceAnalytic, BootStartData bootStartData, IGameDataRepository gameDataRepository, IConnectionDataHandler connectionDataHandler, OptionalDependency<GarageController> garageControllerDependency, OptionalDependency<IAutoSaveService> autoSaveService, IBalanceDocumentSynchronizer balanceDocumentSynchronizer, ISteamAccountService steamAccountService)
		{
			_playerPrefsTimer = playerPrefsTimer;
			_connectionManager = connectionManager;
			_accountDataProvider = accountDataProvider;
			_progressContainer = progressContainer;
			_playerStorageService = playerStorageService;
			_dataBase = dataBase;
			_accountController = accountController;
			_profileAnalytic = profileAnalytic;
			_serverTimeService = serverTimeService;
			_userPreferenceAnalytic = userPreferenceAnalytic;
			_bootStartData = bootStartData;
			_gameDataRepository = gameDataRepository;
			_connectionDataHandler = connectionDataHandler;
			_garageControllerDependency = garageControllerDependency;
			_autoSaveService = autoSaveService;
			_steamAccountService = steamAccountService;
			_balanceDocumentSynchronizer = balanceDocumentSynchronizer;
		}

		public async UniTask<(SignUpResult, bool)> CreateNewAccount(string email, string password)
		{
			if (!InternetHelper.HasInternetConnection())
			{
				_ = _progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.NoInternetConnection"), success: false);
				return (SignUpResult.NoInternet, false);
			}
			_connectionManager.CorrectHash.Get(out var value);
			if (!value && !_accountController.Authorized)
			{
				_connectionManager.RunCheckHashManually(_accountController.Authorized);
				for (int i = 0; i < 3; i++)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(5.0));
					_connectionManager.CorrectHash.Get(out value);
					if (value)
					{
						break;
					}
				}
			}
			if (!value)
			{
				_ = _progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.DownloadLatestVersionOfTheGame"), success: false);
				return (SignUpResult.DownloadLatestVersion, false);
			}
			if (ScenesHelper.IsNotificationActive())
			{
				await _accountController.ResetData(reloadScene: false);
			}
			PauseAutoSave();
			bool resumeNeeded = true;
			try
			{
				if (!(await TryWaitForAutoSaveBeforeAccountChange()))
				{
					return (SignUpResult.ErrorUnknown, false);
				}
				_progressContainer.SetProgress(0.05f, new LocalizedString("RegistrationController.TrySignUp"));
				bool flag = !ScenesHelper.IsNotificationActive();
				if (flag)
				{
					flag = !(await TrySaveRecords());
				}
				if (flag)
				{
					return (SignUpResult.ErrorUnknown, false);
				}
				bool isCreateNewAccount = false;
				SignUpData result;
				if (AccountSystemV2Extension.IsGuestAccount(_accountController.Email) || AccountSystemV2Extension.IsSteamOnlyAccount(_accountController.Email))
				{
					result = await _accountController.ChangeEmailPassword(email, password);
				}
				else if (_accountController.Authorized)
				{
					isCreateNewAccount = true;
					result = await _accountController.SignUp(email, password, needSignOut: true);
					if (result.result == SignUpResult.Success)
					{
						_dataBase.SlotService.DeleteAllSlots();
						ClearVehiclesSlotIds();
					}
				}
				else
				{
					result = await _accountController.SignUp(email, password, needSignOut: false, resetData: false);
				}
				if (result.result != SignUpResult.Success)
				{
					await _progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.ThisEmailIsAlreadyTakenTryAnot"), success: false);
					return (result.result, false);
				}
				bool resultSaveData = await OnAccountCreated();
				if (!string.IsNullOrEmpty(result.localId))
				{
					_playerStorageService.LocalID = result.localId;
				}
				_playerStorageService.Save();
				if ((isCreateNewAccount || ScenesHelper.IsNotificationActive()) & resultSaveData)
				{
					_ = _progressContainer.EndAfterDelay(new LocalizedString("DataBase.Complete"));
					resumeNeeded = false;
					_bootStartData.LoadingType = BootStartLoadingType.RegistrationSignUp;
					SceneManager.LoadScene(0);
				}
				else
				{
					_connectionDataHandler.ProcessUserConnectionData(UserConnectionData.Default);
					await _balanceDocumentSynchronizer.EnsureFreshAsync(CancellationToken.None);
					_ = _progressContainer.EndAfterDelay(new LocalizedString("DataBase.Complete"));
					_gameDataRepository.ExecuteAsync(GameDataRepositoryDataType.Part).Forget();
				}
				return (result.result, resultSaveData);
			}
			finally
			{
				if (resumeNeeded)
				{
					ResumeAutoSave();
				}
			}
		}

		public async UniTask<bool> SignInNew(string email, string password)
		{
			email = email.Replace("\u200b", "");
			password = password.Replace("\u200b", "");
			if (!InternetHelper.HasInternetConnection())
			{
				_ = _progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.NoInternetConnection"), success: false);
				return false;
			}
			if (ScenesHelper.IsNotificationActive())
			{
				await _accountController.ResetData(reloadScene: false);
			}
			bool flag = !ScenesHelper.IsNotificationActive();
			if (flag)
			{
				flag = !(await TrySaveRecords());
			}
			if (flag)
			{
				return false;
			}
			PauseAutoSave();
			bool resumeNeeded = true;
			try
			{
				if (!(await TryWaitForAutoSaveBeforeAccountChange()))
				{
					return false;
				}
				ObscuredPrefs.SetString("CarsToSave", "");
				_dataBase.ReInit();
				LoginInProgressContainer.PendingFullBootstrap = true;
				bool signedIn = await _accountController.SignIn(email, password);
				if (signedIn)
				{
					ClearVehiclesSlotIds();
					await _accountController.HandleDeviceLogin();
					await SyncSteamLinkFlag(email);
					EasySaveDataHandler.DeleteAll();
					EasySaveDataHandler.Initialize();
				}
				if (signedIn)
				{
					await UniTask.Delay(100);
					_bootStartData.LoadingType = BootStartLoadingType.RegistrationSignIn;
					Debug.Log("get all cars");
					LoginInProgressContainer.IsLoginInProgress = false;
					ES3.Save("loginTime", _serverTimeService.CurrentServerUnixTimeSec);
					_profileAnalytic.LogLogin(LoginStatus.LOGIN_COMPLETED, email);
					resumeNeeded = false;
					SceneManager.LoadScene(0);
				}
				else
				{
					LoginInProgressContainer.PendingFullBootstrap = false;
					_ = _progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.CanTSignInWithThisEmailAndPass"), success: false);
					_profileAnalytic.LogLogin(LoginStatus.LOGIN_FAILED, email);
				}
				return signedIn;
			}
			finally
			{
				if (resumeNeeded)
				{
					ResumeAutoSave();
				}
			}
		}

		private async UniTask SyncSteamLinkFlag(string email)
		{
			if (_steamAccountService != null)
			{
				SteamAuthResponse steamAuthResponse = await _steamAccountService.CheckSteamIdAsync();
				ManagerKeysForPrefs.IsSteamLinked = steamAuthResponse != null && !string.IsNullOrEmpty(steamAuthResponse.Email) && string.Equals(steamAuthResponse.Email, email, StringComparison.OrdinalIgnoreCase);
			}
		}

		public async UniTask<bool> SignInWithSteamNew()
		{
			if (!InternetHelper.HasInternetConnection())
			{
				_ = _progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.NoInternetConnection"), success: false);
				return false;
			}
			if (!SteamManager.IsInitialized())
			{
				_ = _progressContainer.EndAfterDelay(new LocalizedString("SteamError.SteamUnavailable"), success: false);
				return false;
			}
			if (!(await TrySaveRecords()))
			{
				return false;
			}
			ObscuredPrefs.SetString("CarsToSave", "");
			_dataBase.ReInit();
			EasySaveDataHandler.DeleteAll();
			LevelProgressionHelper.GetLevelProgressionData();
			bool signedIn = false;
			SteamSignInOutcome steamSignInOutcome = SteamSignInOutcome.NotLinked;
			if (_steamAccountService != null)
			{
				steamSignInOutcome = await _steamAccountService.TrySignInWithSteamDetailedAsync(_accountController, withoutIsNew: true);
				signedIn = steamSignInOutcome == SteamSignInOutcome.SignedIn;
			}
			if (signedIn)
			{
				EasySaveDataHandler.Initialize();
				await _accountController.HandleDeviceLogin();
				await UniTask.Delay(100);
				_bootStartData.LoadingType = BootStartLoadingType.RegistrationSignIn;
				LoginInProgressContainer.IsLoginInProgress = false;
				ES3.Save("loginTime", _serverTimeService.CurrentServerUnixTimeSec);
				_profileAnalytic.LogLogin(LoginStatus.LOGIN_COMPLETED, _accountController.Email, "steam");
				SceneManager.LoadScene(0);
			}
			else
			{
				_ = _progressContainer.EndAfterDelay(new LocalizedString((steamSignInOutcome == SteamSignInOutcome.SteamUnavailable) ? "SteamError.SteamUnavailable" : "Steamauthorization.NoAccountLinkedWasFound"), success: false);
				_profileAnalytic.LogLogin(LoginStatus.LOGIN_FAILED, _accountController.Email, "steam");
			}
			return signedIn;
		}

		public async UniTask SignOut()
		{
			if (!InternetHelper.HasInternetConnection())
			{
				_ = _progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.NoInternetConnection"), success: false);
				return;
			}
			await _dataBase.SendAllRecords();
			_accountController.SignOut();
			long num = ES3.Load<long>("loginTime");
			_profileAnalytic.LogLogout(_serverTimeService.CurrentServerUnixTimeSec - num);
		}

		private async UniTask<bool> OnAccountCreated()
		{
			_progressContainer.SetProgress(0.1f, new LocalizedString("DataBase.SavingData"));
			FirstSignIn = true;
			LoginInProgressContainer.IsLoginInProgress = false;
			PlayerPrefs.Save();
			ObscuredPrefs.Save();
			_progressContainer.SetProgress(0.12f, new LocalizedString("RegistrationController.SavingAccount"));
			_accountController.HandleDeviceLogin().Forget();
			_progressContainer.SetProgress(0.13f, new LocalizedString("RegistrationController.SavingDataAndCars"));
			bool result = await SendAllData();
			_progressContainer.SetProgress(0.99f, new LocalizedString("RegistrationController.Saving"));
			return result;
		}

		private async UniTask<bool> SendAllData()
		{
			LoadingStarted();
			bool num = await _dataBase.SendAllRecords();
			if (num)
			{
				FirstSignIn = true;
			}
			SendDataCompleted();
			return num;
		}

		private async UniTask<bool> TrySaveRecords()
		{
			if (_accountController.Authorized && !(await _dataBase.SendAllRecords(saveCars: true, fastSave: false, useEndProgressContainer: false, 0.3f)))
			{
				await _progressContainer.EndAfterDelay(new LocalizedString("DataBase.ErrorWhileSaving"), success: false);
				return false;
			}
			return true;
		}

		private void LoadingStarted()
		{
			Debug.Log("Loading started");
		}

		private void SendDataCompleted()
		{
			Debug.Log("SendDataCompleted");
		}

		public void OnProgress(float progress)
		{
			Debug.Log($"progress:{progress}");
		}

		public void OnMessage(string message)
		{
			Debug.Log("message:" + message);
		}

		public string PlayerNameChanged(string newName)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.name_change, ObscuredPrefs.GetString("PlayerName"), newName);
			newName = PlayerDatas.CheckNameValid(newName);
			ObscuredPrefs.SetString("PlayerName", newName);
			_accountDataProvider.Name = newName;
			_playerStorageService.Save();
			return newName;
		}

		public async UniTask<(bool, string)> ResetPassword(string email)
		{
			if (!_playerPrefsTimer.IsReadyToReset("LastTimeResetPressed"))
			{
				LocalizedString localizedString = new LocalizedString("SignInRequiredWindow.YouCanresetpassword");
				string item = string.Format(arg0: Mathf.RoundToInt(15f), format: localizedString);
				return (false, item);
			}
			if (await _accountController.ResetPassword(email))
			{
				return (true, new LocalizedString("SignInRequiredWindow.Pleasecheckyouremail"));
			}
			return (false, new LocalizedString("SignInRequiredWindow.unkownerrorpleaseretry"));
		}

		private async UniTask<bool> TryWaitForAutoSaveBeforeAccountChange()
		{
			IAutoSaveService value = _autoSaveService.Value;
			bool flag = value == null;
			if (!flag)
			{
				flag = await value.WaitForOngoingSaveAsync(20f);
			}
			if (flag)
			{
				return true;
			}
			_progressContainer.EndAfterDelay(new LocalizedString("RegistrationController.SomethingWasWrongTryLater"), success: false).Forget();
			return false;
		}

		private void ClearVehiclesSlotIds()
		{
			if (_garageControllerDependency == null || !_garageControllerDependency.Value || _garageControllerDependency.Value.CurrentGarage == null)
			{
				return;
			}
			VehicleData[] allCars = _garageControllerDependency.Value.CurrentGarage.GetAllCars();
			foreach (VehicleData vehicleData in allCars)
			{
				if ((bool)vehicleData)
				{
					vehicleData.slotId = -1;
				}
			}
		}

		private void PauseAutoSave()
		{
			_autoSaveService.Value?.Pause();
		}

		private void ResumeAutoSave()
		{
			_autoSaveService.Value?.Resume();
		}
	}
}
