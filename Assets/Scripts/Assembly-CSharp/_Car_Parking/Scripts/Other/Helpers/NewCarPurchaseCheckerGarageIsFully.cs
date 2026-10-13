using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class NewCarPurchaseCheckerGarageIsFully : IPurchaseCheckProductToBuy
	{
		private bool _showMessageBeforeBuySlot;

		public NewCarPurchaseCheckerGarageIsFully(bool showMessageBeforeBuySlot)
		{
			_showMessageBeforeBuySlot = showMessageBeforeBuySlot;
		}

		public async UniTask<PreBuyActionResult> CheckProductToBuyAsync(string productID)
		{
			if (V.CoreGameManager.SlotService.SlotCollection.OccupiedSlotCount >= V.CoreGameManager.SlotService.SlotCollection.MaxCarSpotCount)
			{
				if (_showMessageBeforeBuySlot)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("DealerWindow.YourGarageHasNoEmptySpot"), 3f);
				}
				return PreBuyActionResult.NoSlots;
			}
			return PreBuyActionResult.Success;
		}
	}
}
