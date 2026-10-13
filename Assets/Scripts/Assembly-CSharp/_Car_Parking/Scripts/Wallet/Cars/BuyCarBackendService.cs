using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Wallet.Cypher;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	public class BuyCarBackendService : IBuyCarBackendService
	{
		private readonly FirebaseWrappedFunctionFactory _factory;

		private IAccountController _accountController;

		private CurrencyManagerController _currencyManagerController;

		private IDatabaseSaveLoadService _databaseSaveLoadService;

		private IConnectionManager _connectionManager;

		public const string BuyCarFn = "BuyCar24_1";

		private const string PlatformHeaderKey = "X-Client-Platform";

		[Inject]
		public BuyCarBackendService(IAccountController accountController, CurrencyManagerController currencyManagerController, IDatabaseSaveLoadService databaseSaveLoadService, IConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
			_databaseSaveLoadService = databaseSaveLoadService;
			_currencyManagerController = currencyManagerController;
			_accountController = accountController;
			_factory = new FirebaseWrappedFunctionFactory();
		}

		public async UniTask<(AdvancedBoolean, string)> TryBuy(CarBackendRequest data)
		{
			_ = 3;
			try
			{
				int id = data.Id;
				if (id == 165 || id == 229 || id == 230 || id == 233 || id == 238 || id == 241 || id == 243 || id == 245 || id == 255 || id == 257 || id == 266 || id == 262 || id == 263)
				{
					ManagerUIHelper.SetMessage("Error while buying car: 0x33", 3f, NotificationType.Error);
					return (new AdvancedBoolean(v: false), string.Empty);
				}
				if (!InternetHelper.HasInternetConnection())
				{
					ManagerUIHelper.SetMessage(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f, NotificationType.Error);
					return (new AdvancedBoolean(v: false), string.Empty);
				}
				_connectionManager.CorrectHash.Get(out var value);
				if (!value)
				{
					Debug.Log("Buy car error: 0x1. Wait 3 sec.");
					await UniTask.Delay(TimeSpan.FromSeconds(3.0));
				}
				_connectionManager.CorrectHash.Get(out value);
				if (!value)
				{
					ManagerUIHelper.SetMessage("Buy car error: 0x1", 3f, NotificationType.Error);
					return (new AdvancedBoolean(v: false), string.Empty);
				}
				if (!_accountController.Authorized)
				{
					return (new AdvancedBoolean(v: false), string.Empty);
				}
				await _databaseSaveLoadService.SendWalletData(new WalletData(CurrencyManagerController.Money));
				ObscuredInt lastMoney = CurrencyManagerController.Money;
				FirebaseWrappedFunction firebaseWrappedFunction = _factory.Create("BuyCar24_1", 15);
				FunctionCallResult<CarBuyResponseRaw> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<CarBuyResponseRaw>(data.GetRequest(), 1, new Dictionary<string, string> { ["X-Client-Platform"] = "STEAM" });
				if (firebaseWrappedFunction.WasTimeouted)
				{
					ManagerUIHelper.SetMessage(new LocalizedString("DataBase.UnstableInternetConnectionTryR"), 3f, NotificationType.Error);
					return (new AdvancedBoolean(v: false), string.Empty);
				}
				if (functionCallResult.IsFail || functionCallResult.Data == null)
				{
					if (functionCallResult.Status == FunctionStatus.PurchaseInsufficientFundsMoney)
					{
						_currencyManagerController.ShowNotEnoughMoneyWindow();
					}
					else if (functionCallResult.Status == FunctionStatus.PurchaseInsufficientFundsCoins)
					{
						_currencyManagerController.ShowNotEnoughCoinsWindow();
					}
					else
					{
						ManagerUIHelper.SetMessage(new LocalizedString("DataBase.UnstableInternetConnectionTryR"), 3f, NotificationType.Error);
					}
					return (new AdvancedBoolean(v: false), string.Empty);
				}
				CarBuyResponse result = BuyCarCypher.GetBuyCarResult(functionCallResult.Data);
				if (result == null || string.IsNullOrEmpty(result.CarInstanceID))
				{
					ManagerUIHelper.SetMessage(new LocalizedString("DataBase.UnstableInternetConnectionTryR"), 3f, NotificationType.Error);
					return (new AdvancedBoolean(v: false), string.Empty);
				}
				if (result.NewMoney.HasValue)
				{
					int value2 = result.NewMoney.Value;
					if (value2 > (int)lastMoney)
					{
						return (new AdvancedBoolean(v: false), string.Empty);
					}
					_currencyManagerController.SetMoney(new AdvancedInt(value2));
					await _databaseSaveLoadService.SendWalletData(new WalletData(value2));
				}
				if (result.NewCoins.HasValue)
				{
					CurrencyManagerController.SetCoins(new AdvancedInt(result.NewCoins.Value));
					CurrencyManagerController.UpdateCoinsVisuals();
				}
				return (new AdvancedBoolean(v: true), result.CarInstanceID);
			}
			catch (Exception ex)
			{
				Debug.LogError("Buy car function:" + ex);
				ManagerUIHelper.SetMessage("Something went wrong: 0x4", 3f, NotificationType.Error);
				return (new AdvancedBoolean(v: false), string.Empty);
			}
		}
	}
}
