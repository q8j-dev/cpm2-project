using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.CarSlots
{
	public class VehicleSlotPaymentService
	{
		public static ObscuredInt Price = 100;

		public const string BuyCarSlotFn = "BuyCarSlot17";

		private CurrencyManagerController _currencyManagerController;

		private PurchaseItemService _purchaseItemService;

		[Inject]
		public VehicleSlotPaymentService(PurchaseItemService purchaseItemService, CurrencyManagerController currencyManagerController)
		{
			_purchaseItemService = purchaseItemService;
			_currencyManagerController = currencyManagerController;
		}

		public async UniTask<AdvancedBoolean> TryPurchase(int GarageId, Action<int> onPurchaseResult = null)
		{
			PurchaseItemResult purchaseItemResult = await _purchaseItemService.TryPurchase(PurchaseItemType.GarageSlot, new { GarageId });
			onPurchaseResult?.Invoke(purchaseItemResult.StatusCode);
			switch (purchaseItemResult.StatusCode)
			{
			case 0:
				MessageShow(new LocalizedString("DebugAccountData.SomethingWentWrong"));
				break;
			case 1:
				MessageShow(new LocalizedString("CarSlotsPurchaseService.SlotBoughtSuccessfully"));
				break;
			case 2:
				MessageShow(new LocalizedString("CarSlotsPurchaseService.YouAlreadyHaveMaxSlotCount"));
				break;
			case 3:
				_currencyManagerController.ShowNotEnoughCoinsWindow();
				break;
			case 4:
				MessageShow(new LocalizedString("CarSlotsPurchaseService.GarageNotFound"));
				break;
			}
			return purchaseItemResult.Status;
		}

		private void MessageShow(string message)
		{
			if ((bool)V.CoreGameManager)
			{
				V.CoreGameManager.SetMessage(message, 5f);
			}
		}
	}
}
