using UnityEngine.Purchasing;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public static class PurchaseRealErrorReasonExtensions
	{
		public static PurchaseRealErrorReason ToPurchaseRealErrorReason(this PurchaseFailureReason purchaseFailureReason)
		{
			return PurchaseRealErrorReason.StoreNone;
		}

		public static PurchaseRealErrorReason ToPurchaseRealErrorReason(this PreBuyActionResult preBuyActionResult)
		{
			return preBuyActionResult switch
			{
				PreBuyActionResult.Success => PurchaseRealErrorReason.None, 
				PreBuyActionResult.NoSlots => PurchaseRealErrorReason.ClientNoSlots, 
				PreBuyActionResult.HashError => PurchaseRealErrorReason.ClientHashError, 
				PreBuyActionResult.OfferIsOld => PurchaseRealErrorReason.ClientOfferIsOld, 
				_ => PurchaseRealErrorReason.ClientUnknown, 
			};
		}

		public static PurchaseRealErrorReason ToPurchaseRealErrorReason(this PurchaseItemResultStatus purchaseItemResultStatus)
		{
			return purchaseItemResultStatus switch
			{
				PurchaseItemResultStatus.Success => PurchaseRealErrorReason.None, 
				PurchaseItemResultStatus.Error => PurchaseRealErrorReason.BackendError, 
				PurchaseItemResultStatus.ItemWasBought => PurchaseRealErrorReason.BackendItemWasBought, 
				PurchaseItemResultStatus.PaymentIsBad => PurchaseRealErrorReason.BackendPaymentIsBad, 
				_ => PurchaseRealErrorReason.BackendUnknown, 
			};
		}
	}
}
