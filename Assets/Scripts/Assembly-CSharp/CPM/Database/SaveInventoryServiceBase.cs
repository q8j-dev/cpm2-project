using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using Persistence.Systems;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Other;

namespace CPM.Database
{
	public abstract class SaveInventoryServiceBase<T, THashes> : IReInitPreMainMenu, IDatabaseInventorySaveLoadService<T> where THashes : HashesData<T>, new()
	{
		private PlayerDataStorage _playerDataStorage;

		protected readonly FirebaseWrappedFunctionFactory Factory;

		private IAccountController _accountController;

		private IConnectionManager _connectionManager;

		public abstract string SendFunction { get; }

		public abstract string HashesPrefsKey { get; }

		protected PresetGrantSession PresetGrantSession { get; private set; }

		public THashes HashesData { get; set; }

		public event Action StartSaving;

		public event Action EndSaving;

		[Inject]
		public void Construct(IAccountController accountController, IConnectionManager connectionManager, PlayerDataStorage playerDataStorage, PresetGrantSession presetGrantSession)
		{
			_connectionManager = connectionManager;
			_accountController = accountController;
			_playerDataStorage = playerDataStorage;
			PresetGrantSession = presetGrantSession;
		}

		public void Receive(T receivedData)
		{
			PrintReceivedData(receivedData);
			HashesData = new THashes();
			HashesData.CalculateChangedValues(receivedData, out var _);
			HashesData.ConfirmSuccess();
			SaveHashesInPrefs();
		}

		public async UniTask<FunctionCallResult<FunctionResult>> TrySend(T data, bool fullData = false)
		{
			if (!_accountController.HasEmail)
			{
				return FunctionCallResult<FunctionResult>.Fail(FunctionStatus.LocalUnauthorized);
			}
			if ((int)_connectionManager.H == 1)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(5.0));
			}
			_connectionManager.CorrectHash.Get(out var value);
			if (!value)
			{
				return FunctionCallResult<FunctionResult>.Fail(FunctionStatus.LocalIncorrectHash);
			}
			if (SaveIntegrity.ViolatedThisSession)
			{
				return FunctionCallResult<FunctionResult>.Fail(FunctionStatus.LocalSaveIntegrityViolation);
			}
			return await Send(data, fullData);
		}

		public async UniTask<FunctionCallResult<FunctionResult>> Send(T data, bool fullData = false)
		{
			StartSaving?.Invoke();
			string serializedData = null;
			Dictionary<string, object> changedData;
			using (ExcludeTransientSendData(data))
			{
				HashesData.CalculateChangedValues(data, out changedData, fullData);
				PrintSendData("Send data", changedData);
				if (changedData.Count > 0)
				{
					serializedData = SerializeChangedData(changedData);
				}
			}
			FunctionCallResult<FunctionResult> result = await SendChangedPlayerData(changedData, serializedData);
			EndSaving?.Invoke();
			return result;
		}

		protected virtual IDisposable ExcludeTransientSendData(T data)
		{
			return null;
		}

		private string SerializeChangedData(Dictionary<string, object> changedData)
		{
			MemoryPackDictionaryBase memoryPackDictionary = MemoryPackSaveInventoryServiceHelper.GetMemoryPackDictionary(SendFunction);
			if (memoryPackDictionary != null)
			{
				return memoryPackDictionary.Serialize(changedData, _playerDataStorage.AccountID);
			}
			return JsonConvert.SerializeObject(changedData, JsonConvertCustomSettingsForDatabase.JsonSettingsForDataBase);
		}

		public void Init()
		{
			LoadCachedData();
		}

		public void ReInit()
		{
			Init();
		}

		protected async UniTask<FunctionCallResult<FunctionResult>> SendChangedPlayerData(Dictionary<string, object> changedData, string serializedData)
		{
			if (changedData.Count == 0)
			{
				return FunctionCallResult<FunctionResult>.Fail(FunctionStatus.LocalNoDataToSend);
			}
			switch (_connectionManager.H)
			{
			case 0:
				return FunctionCallResult<FunctionResult>.Fail(FunctionStatus.LocalIncorrectHash);
			case 1:
				return FunctionCallResult<FunctionResult>.Fail(FunctionStatus.LocalUnknownHash);
			default:
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					return FunctionCallResult<FunctionResult>.Fail(FunctionStatus.NoInternet);
				}
				FunctionCallResult<FunctionResult> result = await Factory.Create(SendFunction).CallAsyncResult<FunctionResult>(serializedData);
				if (result.IsSuccess)
				{
					HashesData.ConfirmSuccess();
					SaveHashesInPrefs();
					if (SendFunction == "SaveSlotsCollection24_1")
					{
						PlayerPrefs.SetInt("ForceSaveSlots", 0);
					}
				}
				return result;
			}
			}
		}

		public SaveInventoryServiceBase()
		{
			Factory = new FirebaseWrappedFunctionFactory();
		}

		protected void LoadCachedData()
		{
			string text = ObscuredPrefs.GetString(HashesPrefsKey);
			if (text != "")
			{
				HashesData = JsonConvert.DeserializeObject<THashes>(text);
			}
			else
			{
				HashesData = new THashes();
			}
		}

		protected void SaveHashesInPrefs()
		{
			string value = JsonConvert.SerializeObject(HashesData);
			ObscuredPrefs.SetString(HashesPrefsKey, value);
		}

		private void PrintSendData(string message, Dictionary<string, object> changedData)
		{
		}

		private void PrintReceivedData(T receivedData)
		{
		}
	}
}
