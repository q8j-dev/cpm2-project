using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using CPM.Balance;
using CPM.Connection;
using CPM.Database;
using CPM.PlayerData;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Persistence.Systems;
using UnityEngine;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	public class BootstrapHandler : IBootstrapHandler, IInitializable, IDisposable
	{
		private const int MaxAttempt = 3;

		private const int MasterMainStartupAttempt = 2;

		private const float DelayBeforeNextTry = 2f;

		public const string MasterMainStartupFn = "MasterMainStartup24_1";

		private const float SteamInitWaitSeconds = 3f;

		private const string MasterMainStartupFullData = "1";

		private const string MasterMainStartupPartData = "0";

		private const string StageInit = "Init";

		private const string StageSignIn = "SignIn";

		private const string StageStartup = "Startup";

		private const string StageDatabase = "Database";

		private readonly IAccountController _accountController;

		private readonly IOfflineModeSystem _offlineModeSystem;

		private readonly DataBase _dataBase;

		private readonly IDatabaseSaveLoadService _databaseSaveLoadService;

		private readonly IConnectionDataHandler _connectionDataHandler;

		private readonly BootStartData _bootStartData;

		private readonly IGameDataRepository _gameDataRepository;

		private readonly ISlotService _slotService;

		private readonly MigrationAnalytic _migrationAnalytic;

		private readonly PlayerDataStorage _playerDataStorage;

		private readonly IBalanceDocumentSynchronizer _balanceDocumentSynchronizer;

		private readonly WeightedProgress _weightedProgress = new WeightedProgress();

		private readonly ISteamAccountService _steamAccountService;

		private readonly ISteamPurchaseRestoreService _steamPurchaseRestoreService;

		private readonly ISteamPurchaseGate _steamPurchaseGate;

		public float Progress => _weightedProgress.Total;

		public BootstrapHandler(IAccountController accountController, IOfflineModeSystem offlineModeSystem, DataBase dataBase, IDatabaseSaveLoadService databaseSaveLoadService, IConnectionDataHandler connectionDataHandler, BootStartData bootStartData, IGameDataRepository gameDataRepository, ISlotService slotService, MigrationAnalytic migrationAnalytic, PlayerDataStorage playerDataStorage, ISteamAccountService steamAccountService, ISteamPurchaseRestoreService steamPurchaseRestoreService, ISteamPurchaseGate steamPurchaseGate, IBalanceDocumentSynchronizer balanceDocumentSynchronizer, IPlayerProfileService playerProfileService)
		{
			_accountController = accountController;
			_offlineModeSystem = offlineModeSystem;
			_dataBase = dataBase;
			_databaseSaveLoadService = databaseSaveLoadService;
			_connectionDataHandler = connectionDataHandler;
			_bootStartData = bootStartData;
			_gameDataRepository = gameDataRepository;
			_slotService = slotService;
			_migrationAnalytic = migrationAnalytic;
			_playerDataStorage = playerDataStorage;
			_steamAccountService = steamAccountService;
			_steamPurchaseRestoreService = steamPurchaseRestoreService;
			_steamPurchaseGate = steamPurchaseGate;
			_balanceDocumentSynchronizer = balanceDocumentSynchronizer;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}

		public async UniTask<BootStartResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			_balanceDocumentSynchronizer.EnsureFreshAsync(cancellationToken).Forget();
			_weightedProgress.Reset();
			_weightedProgress.CreateStage("Init", 0.05f);
			_weightedProgress.CreateStage("SignIn", 0.2f);
			_weightedProgress.CreateStage("Startup", 0.5f);
			_weightedProgress.CreateStage("Database", 0.25f);
			if (LoginInProgressContainer.IsLoginInProgress)
			{
				await _accountController.ResetData(reloadScene: false);
			}
			_gameDataRepository.Clear();
			_playerDataStorage.PlayerID = ManagerKeysForPrefs.LocalId;
			_weightedProgress.SetProgress("Init", 1f);
			if (SteamManager.IsInitialized() && ManagerKeysForPrefs.HasLinkedSteamId)
			{
				string userIdString = SteamManager.GetUserIdString();
				string linkedSteamId = ManagerKeysForPrefs.LinkedSteamId;
				if (linkedSteamId != "0" && userIdString != "0" && linkedSteamId != userIdString)
				{
					await _accountController.ResetData(reloadScene: false);
				}
			}
			BootStartResult result = await ExecuteAttemptAsync(3, cancellationToken);
			LoginInProgressContainer.IsLoginInProgress = false;
			if (result == BootStartResult.Success)
			{
				LoginInProgressContainer.PendingFullBootstrap = false;
				if (_bootStartData.LoadingType == BootStartLoadingType.Default)
				{
					await CheckDataBaseIsCorrectAsync();
				}
				await _balanceDocumentSynchronizer.EnsureFreshAsync(cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					result = BootStartResult.Canceled;
				}
				else if (_accountController.HasEmail)
				{
					GameDataRepositoryDataType dataType = GameDataRepositoryDataType.Full;
					if (_bootStartData.LoadingType == BootStartLoadingType.RegistrationSignUp)
					{
						dataType = GameDataRepositoryDataType.Part;
					}
					_gameDataRepository.ExecuteAsync(dataType).Forget();
				}
				if (_steamPurchaseGate != null)
				{
					await _steamPurchaseGate.FetchAsync();
				}
				if (_steamPurchaseRestoreService != null && SteamManager.IsInitialized() && ManagerKeysForPrefs.IsSteamLinked)
				{
					try
					{
						await _steamPurchaseRestoreService.RestoreIfNeeded();
						if (_dataBase != null)
						{
							_dataBase.DatabaseCarsService.DeleteData();
							await _dataBase.LoadCarSlots();
							await _dataBase.LoadCars();
						}
					}
					catch (Exception)
					{
					}
				}
			}
			_weightedProgress.SetProgress("Database", 1f);
			_bootStartData.LoadingType = BootStartLoadingType.Default;
			return result;
		}

		private async UniTask<BootStartResult> ExecuteAttemptAsync(int attempt, CancellationToken cancellationToken)
		{
			attempt--;
			_weightedProgress.ResetStage("SignIn");
			_weightedProgress.ResetStage("Startup");
			if (_offlineModeSystem.Enabled)
			{
				_weightedProgress.SetProgress("SignIn", 1f);
				_weightedProgress.SetProgress("Startup", 1f);
				return BootStartResult.SuccessOfflineMode;
			}
			if (_bootStartData.LoadingType == BootStartLoadingType.RegistrationSignIn)
			{
				_weightedProgress.SetProgress("SignIn", 1f);
				return await MasterMainStartupAsync("1", attempt, cancellationToken);
			}
			if (_bootStartData.LoadingType == BootStartLoadingType.RegistrationSignUp)
			{
				_weightedProgress.SetProgress("SignIn", 1f);
				return await MasterMainStartupAsync("0", attempt, cancellationToken);
			}
			bool flag = ManagerKeysForPrefs.LinkedSteamId != SteamManager.GetUserIdString();
			if (SteamManager.IsInitialized() & flag)
			{
				LoginInProgressContainer.PendingFullBootstrap = true;
			}
			string steamStartupData = (LoginInProgressContainer.PendingFullBootstrap ? "1" : "0");
			bool preferEmailLogin = ManagerKeysForPrefs.LastLoginMethod == LoginMethod.Email && _accountController.HasEmail && ManagerKeysForPrefs.HasPasswordAccount && !AccountSystemV2Extension.IsSteamOnlyAccount(_accountController.Email);
			bool kickedByAnotherDevice = ManagerKeysForPrefs.KickedByAnotherDevice;
			if (kickedByAnotherDevice)
			{
				ManagerKeysForPrefs.KickedByAnotherDevice = false;
				ObscuredPrefs.Save();
			}
			if (_steamAccountService != null && !preferEmailLogin && !kickedByAnotherDevice)
			{
				await SteamManager.WaitForInitAsync(3f);
			}
			if (_steamAccountService != null && SteamManager.IsInitialized() && !preferEmailLogin && !kickedByAnotherDevice)
			{
				SteamSignInOutcome steamSignInOutcome = await _steamAccountService.TrySignInWithSteamDetailedAsync(_accountController, withoutIsNew: true);
				if (cancellationToken.IsCancellationRequested)
				{
					return BootStartResult.Canceled;
				}
				_ = 2;
				if (steamSignInOutcome == SteamSignInOutcome.SignedIn)
				{
					EasySaveDataHandler.Initialize();
					ManagerKeysForPrefs.IsSteamLinked = true;
					ObscuredPrefs.Save();
					PlayerPrefs.Save();
					_weightedProgress.SetProgress("SignIn", 1f);
					return await MasterMainStartupAsync(steamStartupData, attempt, cancellationToken);
				}
				_ = 1;
			}
			if (_accountController.HasEmail)
			{
				bool flag2 = ManagerKeysForPrefs.IsSteamLinked && AccountSystemV2Extension.IsGuestAccount(_accountController.Email);
				if (AccountSystemV2Extension.IsSteamOnlyAccount(_accountController.Email) | flag2)
				{
					switch (await _steamAccountService.TrySignInWithSteamDetailedAsync(_accountController))
					{
					case SteamSignInOutcome.SignedIn:
						_weightedProgress.SetProgress("SignIn", 1f);
						return await MasterMainStartupAsync(steamStartupData, attempt, cancellationToken);
					case SteamSignInOutcome.SteamUnavailable:
						return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.ErrorSignInPrefs, cancellationToken);
					default:
						if (InternetHelper.HasInternetConnection())
						{
							await _accountController.ResetData(reloadScene: false);
							if (cancellationToken.IsCancellationRequested)
							{
								return BootStartResult.Canceled;
							}
							return await ExecuteAttemptAsync(attempt, cancellationToken);
						}
						return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.ErrorSignInPrefs, cancellationToken);
					}
				}
				if (!ManagerKeysForPrefs.HasPasswordAccount)
				{
					return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.ErrorSignInPrefs, cancellationToken);
				}
				bool flag3 = await _accountController.SignInWithPrefs();
				if (cancellationToken.IsCancellationRequested)
				{
					return BootStartResult.Canceled;
				}
				if (flag3)
				{
					_weightedProgress.SetProgress("SignIn", 1f);
					string masterMainStartupData = (LoginInProgressContainer.PendingFullBootstrap ? "1" : "0");
					return await MasterMainStartupAsync(masterMainStartupData, attempt, cancellationToken);
				}
				if (_accountController.LastSignInError == "EMAIL_NOT_FOUND" && InternetHelper.HasInternetConnection())
				{
					await _accountController.ResetData(reloadScene: false);
					if (cancellationToken.IsCancellationRequested)
					{
						return BootStartResult.Canceled;
					}
					return await ExecuteAttemptAsync(attempt, cancellationToken);
				}
				return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.ErrorSignInPrefs, cancellationToken);
			}
			_weightedProgress.SetProgress("SignIn", 0.3f);
			bool flag4 = await _accountController.HaveGuestAccount();
			if (cancellationToken.IsCancellationRequested)
			{
				return BootStartResult.Canceled;
			}
			if (!flag4)
			{
				if (ManagerKeysForPrefs.AccountId != "0")
				{
					if ((await new MigrationHandler().ExecuteAsync(_migrationAnalytic, _dataBase)).MigrationMessageType == MigrationMessageType.NetworkError)
					{
						return BootStartResult.CheckInternetConnection;
					}
					await _accountController.ResetData(reloadScene: false);
					return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.Canceled, cancellationToken);
				}
				await _accountController.ResetData(reloadScene: false);
				_weightedProgress.SetProgress("SignIn", 1f);
				_weightedProgress.SetProgress("Startup", 1f);
				return BootStartResult.Success;
			}
			bool flag5 = await _accountController.SignInGuest();
			if (cancellationToken.IsCancellationRequested)
			{
				return BootStartResult.Canceled;
			}
			if (!flag5)
			{
				await _accountController.ResetData(reloadScene: false);
				return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.ErrorSignInGuest, cancellationToken);
			}
			_weightedProgress.SetProgress("SignIn", 1f);
			return await MasterMainStartupAsync("1", attempt, cancellationToken);
		}

		private async UniTask<BootStartResult> MasterMainStartupAsync(string masterMainStartupData, int attempt, CancellationToken cancellationToken)
		{
			FirebaseWrappedFunction firebaseWrappedFunction = new FirebaseWrappedFunctionFactory().Create("MasterMainStartup24_1");
			_weightedProgress.SetProgress("Startup", 0.1f);
			FunctionCallResult<MasterMainStartupResult> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<MasterMainStartupResult>(masterMainStartupData, 2);
			if (cancellationToken.IsCancellationRequested)
			{
				return BootStartResult.Canceled;
			}
			_weightedProgress.SetProgress("Startup", 0.5f);
			if (!functionCallResult.IsSuccess || functionCallResult.Data == null || functionCallResult.Data.ConnectionData == null)
			{
				if (functionCallResult.Status == FunctionStatus.StartupGameVersionMissmath)
				{
					return AccountSystemV2Extension.IsGuestAccount(_accountController.Email) ? BootStartResult.CantPlayGuest : BootStartResult.CantPlayMain;
				}
				if (cancellationToken.IsCancellationRequested)
				{
					return BootStartResult.Canceled;
				}
				return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.ErrorMasterMainStartupResult, cancellationToken);
			}
			if (functionCallResult.Data.ConnectionData.canPlay == 0 || functionCallResult.Status == FunctionStatus.StartupGameVersionMissmath)
			{
				return AccountSystemV2Extension.IsGuestAccount(_accountController.Email) ? BootStartResult.CantPlayGuest : BootStartResult.CantPlayMain;
			}
			_connectionDataHandler.ProcessUserConnectionData(functionCallResult.Data.ConnectionData);
			if (!string.IsNullOrEmpty(functionCallResult.Data.PlayerRecords))
			{
				Result<PlayerStorageFromDataBase, string> result = MemoryPackHelper.TryDeserialize<PlayerStorageFromDataBase>(functionCallResult.Data.PlayerRecords, _accountController.FirebaseUserId, useBrotli: true);
				if (!result.IsSuccess)
				{
					return BootStartResult.DeserializePlayerRecordsError;
				}
				PlayerStorageFromDataBase value = result.Value;
				_databaseSaveLoadService.PrepareRecords(value);
				if (!_dataBase.ApplyPlayerRecords(value))
				{
					return await ExecuteAgainIfNeedAsync(attempt, BootStartResult.DeserializePlayerRecordsError, cancellationToken);
				}
				EasySaveDataHandler.ClearAllCache();
			}
			_weightedProgress.SetProgress("Startup", 0.8f);
			if (!string.IsNullOrEmpty(functionCallResult.Data.AllCars))
			{
				Result<List<CarInfoForDatabase>, string> result2 = MemoryPackHelper.TryDeserialize<List<CarInfoForDatabase>>(functionCallResult.Data.AllCars, _accountController.FirebaseUserId, useBrotli: true);
				if (!result2.IsSuccess)
				{
					return BootStartResult.DeserializeCarsError;
				}
				await _dataBase.ApplyCarsAsync(result2.Value);
				if (cancellationToken.IsCancellationRequested)
				{
					return BootStartResult.Canceled;
				}
				EasySaveDataHandler.ClearAllCache();
			}
			_weightedProgress.SetProgress("Startup", 1f);
			return BootStartResult.Success;
		}

		private async UniTask<BootStartResult> ExecuteAgainIfNeedAsync(int attempt, BootStartResult resultDefault, CancellationToken cancellationToken)
		{
			if (attempt <= 0)
			{
				return resultDefault;
			}
			await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return BootStartResult.Canceled;
			}
			return await ExecuteAttemptAsync(attempt, cancellationToken);
		}

		private async UniTask CheckDataBaseIsCorrectAsync()
		{
			if (_accountController.Authorized && await NewSaveCarSystemExtension.DataBaseNeedsToSyncAsync(_slotService, _dataBase.CarInfoService))
			{
				await _dataBase.SendSlotsCollection(fullData: true);
				await _dataBase.SendAllCars(sendNow: true, sendFull: false);
				_dataBase.DatabaseCarsService.DeleteData();
				await _dataBase.LoadCarSlots();
				await _dataBase.LoadCars();
			}
		}
	}
}
