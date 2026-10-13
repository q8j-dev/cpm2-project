using System;
using System.Collections.Generic;
using CPM.Connection;
using CPM.Database;
using CPM.PlayerData;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.Database
{
	[Preserve]
	public class FirebaseSaveLoadService : IDatabaseSaveLoadService
	{
		public const string GetSlotCollectionFn = "GetSlotsCollection24_1";

		private readonly FirebaseWrappedFunctionFactory _factory;

		private IConnectionManager _connectionManager;

		private EngineSaveLoadService _engineSaveLoadService;

		private PartsInventorySaveLoadService _partsInventorySaveLoadService;

		private PlatesSaveLoadService _platesSaveLoadService;

		private PlayerStorageSaveLoadService _playerStorageSaveLoadService;

		private PlayerAnonStorageSaveLoadService _playerAnonStorageSaveLoadService;

		private WalletSaveLoadService _walletSaveLoadService;

		private SlotsOnDatabaseSaveLoadService _slotsOnDatabaseSaveLoadService;

		private PlayerDataStorage _playerDataStorage;

		private List<int> _userGarages;

		public event Action StartSaving;

		public event Action EndSaving;

		public FirebaseSaveLoadService(IConnectionManager connectionManager, EngineSaveLoadService engineSaveLoadService, PlatesSaveLoadService platesSaveLoadService, PartsInventorySaveLoadService partsInventorySaveLoadService, PlayerStorageSaveLoadService playerStorageSaveLoadService, PlayerAnonStorageSaveLoadService playerAnonStorageSaveLoadService, WalletSaveLoadService walletSaveLoadService, SlotsOnDatabaseSaveLoadService slotsOnDatabaseSaveLoadService, PlayerDataStorage playerDataStorage)
		{
			_slotsOnDatabaseSaveLoadService = slotsOnDatabaseSaveLoadService;
			_playerAnonStorageSaveLoadService = playerAnonStorageSaveLoadService;
			_walletSaveLoadService = walletSaveLoadService;
			_playerStorageSaveLoadService = playerStorageSaveLoadService;
			_platesSaveLoadService = platesSaveLoadService;
			_partsInventorySaveLoadService = partsInventorySaveLoadService;
			_engineSaveLoadService = engineSaveLoadService;
			_connectionManager = connectionManager;
			_playerDataStorage = playerDataStorage;
			_factory = new FirebaseWrappedFunctionFactory();
		}

		public void Init()
		{
		}

		public async UniTask<SlotsOnDatabaseCollection> GetSlotsOnDatabaseCollection(string hash)
		{
			FirebaseWrappedFunction firebaseWrappedFunction = _factory.Create("GetSlotsCollection24_1");
			MemoryPackDecoder decoder = new MemoryPackDecoder(_playerDataStorage.AccountID, useBrotli: true);
			FunctionCallResult<SlotsOnDatabaseCollection> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<SlotsOnDatabaseCollection>(decoder, hash);
			if (functionCallResult.IsSuccess)
			{
				return functionCallResult.Data;
			}
			return null;
		}

		public void PrepareRecords(PlayerStorageFromDataBase records)
		{
			if (records != null)
			{
				records.CheckNulls();
				_playerStorageSaveLoadService.Receive(records.PlayerStorage);
				_partsInventorySaveLoadService.Receive(records.InventoryData);
				_engineSaveLoadService.Receive(records.EngineKits);
				_platesSaveLoadService.Receive(records.PlatesData);
				_walletSaveLoadService.Receive(records.WalletData);
				_slotsOnDatabaseSaveLoadService.Receive(records.SlotsOnDatabaseCollection);
			}
		}

		public async UniTask<FunctionCallResult<FunctionResult>> SendPlayerRecordsPartially(PlayerStorage playerStorage, bool fullData, bool isAnonPlayer)
		{
			if (isAnonPlayer)
			{
				return await _playerAnonStorageSaveLoadService.Send(playerStorage, fullData);
			}
			return await _playerStorageSaveLoadService.Send(playerStorage, fullData);
		}

		public async UniTask<FunctionCallResult<FunctionResult>> SendWalletData(WalletData walletData)
		{
			return await _walletSaveLoadService.Send(walletData);
		}
	}
}
