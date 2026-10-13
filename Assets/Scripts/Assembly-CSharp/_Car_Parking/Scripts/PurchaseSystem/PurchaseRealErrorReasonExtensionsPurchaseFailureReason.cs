using UnityEngine.Purchasing;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public static class PurchaseRealErrorReasonExtensionsPurchaseFailureReason
	{
		public static PurchaseRealErrorSdkReason ToPurchaseRealErrorSdkReason(this PurchaseFailureReason purchaseFailureReason)
		{
			return purchaseFailureReason switch
			{
				PurchaseFailureReason.PurchasingUnavailable => PurchaseRealErrorSdkReason.PurchasingUnavailable, 
				PurchaseFailureReason.ExistingPurchasePending => PurchaseRealErrorSdkReason.ExistingPurchasePending, 
				PurchaseFailureReason.ProductUnavailable => PurchaseRealErrorSdkReason.ProductUnavailable, 
				PurchaseFailureReason.SignatureInvalid => PurchaseRealErrorSdkReason.SignatureInvalid, 
				PurchaseFailureReason.PaymentDeclined => PurchaseRealErrorSdkReason.PaymentDeclined, 
				PurchaseFailureReason.DuplicateTransaction => PurchaseRealErrorSdkReason.DuplicateTransaction, 
				PurchaseFailureReason.ValidationFailure => PurchaseRealErrorSdkReason.ValidateFailed, 
				PurchaseFailureReason.StoreNotConnected => PurchaseRealErrorSdkReason.StoreNotConnected, 
				PurchaseFailureReason.PurchaseMissing => PurchaseRealErrorSdkReason.PurchaseMissing, 
				_ => PurchaseRealErrorSdkReason.Unknown, 
			};
		}
	}
}
