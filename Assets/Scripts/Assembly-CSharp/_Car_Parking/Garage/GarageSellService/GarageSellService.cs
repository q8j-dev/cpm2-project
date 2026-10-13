using System;
using CPM.Database;
using CPM.RemoteConfig;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using UnityEngine.Scripting;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Database;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Garage.GarageSellService
{
	[Preserve]
	public class GarageSellService : IGarageSellService
	{
		private const string SellGarageFunctionName = "SellGarage24_1";

		private const string CheckGarageFunctionName = "CheckGarage24_1";

		private const int ConsumableCheckSuccessCode = 1;

		private const int ConsumableCheckTimeoutSeconds = 20;

		private readonly IGarageSelectorService _garageSelectorService;

		private readonly IGarageBuyService _garageBuyService;

		private readonly ISlotService _slotService;

		private readonly OptionalDependency<DataBase> _dataBase;

		private readonly IRemoteConfigManager _remoteConfigManager;

		private readonly GaragePropertyAnalytic _garagePropertyAnalytic;

		private bool _fetched;

		public bool IsConsumable { get; private set; } = true;

		public GarageSellService(IGarageSelectorService garageSelectorService, IGarageBuyService garageBuyService, ISlotService slotService, GaragePropertyAnalytic garagePropertyAnalytic, IRemoteConfigManager remoteConfigManager, OptionalDependency<DataBase> dataBase)
		{
			_garageSelectorService = garageSelectorService;
			_garageBuyService = garageBuyService;
			_slotService = slotService;
			_dataBase = dataBase;
			_remoteConfigManager = remoteConfigManager;
			_garagePropertyAnalytic = garagePropertyAnalytic;
		}

		public async UniTask SellOrRemove(int garageIndex, GaragePropertyActionType actionType)
		{
			if (!_dataBase.HasValue)
			{
				return;
			}
			await _dataBase.Value.SendSlotsCollection();
			if (!_garageBuyService.BuyingGaragesStatus[garageIndex])
			{
				V.CoreGameManager.ManagerUI.ShowLocalizedErrorNotification("Errors.GarageForSaleNotFound").Forget();
				V.CoreGameManager.ManagerUI.ShowMainMenuWindow();
				return;
			}
			if (_slotService.GetSlotCollection(garageIndex).HaveOccupiedSlot)
			{
				V.CoreGameManager.ManagerUI.ShowLocalizedErrorNotification("Errors.ClearYourGarage").Forget();
				V.CoreGameManager.ManagerUI.ShowMainMenuWindow();
				return;
			}
			int moneyBeforeSell = CurrencyManagerController.Money;
			int coinsBeforeSell = CurrencyManagerController.Coins;
			DataBaseResponse<GarageSellModel> response = await CallSellFunction(garageIndex);
			if (response?.Data != null)
			{
				ResponseCode status = response.Status;
				if (response.Status == ResponseCode.UnknownClient)
				{
					response.Status = ResponseCode.Unknown;
				}
				ConfigValuesStorage configValuesStorage = await _remoteConfigManager.GetConfigValuesStorage();
				int rewardSoft = response.Data.NewMoney - moneyBeforeSell;
				int rewardHard = response.Data.NewCoins - coinsBeforeSell;
				_garagePropertyAnalytic.Log(actionType, response.Status, garageIndex, IsConsumable, rewardSoft, rewardHard, configValuesStorage.legacyGarage);
				if (BackendErrorsProvider.ValidateResponse(status))
				{
					ApplyWalletReward(response.Data);
					ResetGarageOwnership(garageIndex);
				}
			}
		}

		public async UniTask<PurchaseItemType> GetGaragePurchaseItemType()
		{
			bool num = await FetchConsumableStatus();
			PurchaseItemType result = PurchaseItemType.Garage1;
			if (!num)
			{
				return result;
			}
			if (!IsConsumable)
			{
				result = PurchaseItemType.GarageOld1;
			}
			return result;
		}

		public async UniTask<bool> FetchConsumableStatus()
		{
			if (_fetched)
			{
				return true;
			}
			FunctionCallResult<int> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("CheckGarage24_1", 20).CallAsyncResult<int>();
			if (functionCallResult.IsFail)
			{
				return false;
			}
			IsConsumable = functionCallResult.Data == 1;
			_fetched = true;
			return true;
		}

		private async UniTask<DataBaseResponse<GarageSellModel>> CallSellFunction(int garageIndex)
		{
			V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true).Forget();
			try
			{
				FirebaseWrappedFunction firebaseWrappedFunction = new FirebaseWrappedFunctionFactory().Create("SellGarage24_1");
				IDecoder decoder = new AesDecoder();
				FunctionCallResult<DataBaseResponse<GarageSellModel>> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<DataBaseResponse<GarageSellModel>>(decoder, garageIndex, 1);
				if (functionCallResult.IsFail)
				{
					return new DataBaseResponse<GarageSellModel>
					{
						Status = ResponseCode.UnknownClient,
						Data = new GarageSellModel()
					};
				}
				return functionCallResult.Data;
			}
			catch (Exception)
			{
				return new DataBaseResponse<GarageSellModel>
				{
					Status = ResponseCode.UnknownClient,
					Data = new GarageSellModel()
				};
			}
			finally
			{
				V.CoreGameManager.ManagerUI.CloseWaitingWindow();
				V.CoreGameManager.ManagerUI.ShowMainMenuWindow();
			}
		}

		private void ResetGarageOwnership(int garageIndex)
		{
			_garageBuyService.BuyingGaragesStatus[garageIndex] = false;
			_slotService.GetSlotCollection(garageIndex)?.ClearToDefault();
			_garageBuyService.SaveBuyingProgress();
			if (_garageSelectorService.CurrentGarageIndex == garageIndex)
			{
				_garageSelectorService.ChangeMenuGarage(0, withScreen: true).Forget();
			}
		}

		private static void ApplyWalletReward(GarageSellModel model)
		{
			CurrencyManagerController.Instance.SetMoney(new AdvancedInt(model.NewMoney));
			CurrencyManagerController.SetCoins(new AdvancedInt(model.NewCoins));
			CurrencyManagerController.UpdateCoinsVisuals();
		}
	}
}
