using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class NewCarPurchaseActionBuySlotIfNeeds : IPurchaseActionsBeforeToBuyProduct
	{
		private bool _showMessageBeforeBuySlot;

		public NewCarPurchaseActionBuySlotIfNeeds(bool showMessageBeforeBuySlot)
		{
			_showMessageBeforeBuySlot = showMessageBeforeBuySlot;
		}

		public async UniTask<bool> ActionsBeforeToBuyProductAsync(string productID)
		{
			if (V.CoreGameManager.SlotService.AllGaragesFull)
			{
				return false;
			}
			if (V.CoreGameManager.SlotService.TryGetEmptySlot(out var _))
			{
				return true;
			}
			bool waitingToBuySlot = false;
			bool tryBuySlot = true;
			if (_showMessageBeforeBuySlot)
			{
				waitingToBuySlot = true;
				tryBuySlot = false;
				string content = new LocalizedString("DealerWindow.YouAreOutOfSlotsDoYouWantToBuy");
				ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new CombinedPrice(VehicleSlotPaymentService.Price, 0));
				additionalData.SetBackground = true;
				additionalData.Background = new Color(0f, 0f, 0f, 0.85f);
				_ = V.CoreGameManager.ManagerUI.ShowConfirmWindow(() =>
				{
					waitingToBuySlot = false;
					tryBuySlot = true;
				}, new LocalizedString("Newcarswindow.areyousureyouwanttobuythiscar?"), content, () =>
				{
					waitingToBuySlot = false;
				}, "Confirm", null, additionalData);
			}
			while (waitingToBuySlot)
			{
				await UniTask.Yield();
			}
			if (tryBuySlot)
			{
				return await V.CoreGameManager.SlotService.TryBuySlot(-1, checkAllGarages: true);
			}
			return false;
		}
	}
}
