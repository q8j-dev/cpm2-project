namespace _Car_Parking.Scripts.PurchaseSystem
{
	public enum PurchaseRealErrorSdkReason
	{
		Unknown = 0,
		PurchasingUnavailable = 1,
		ExistingPurchasePending = 2,
		ProductUnavailable = 3,
		SignatureInvalid = 4,
		PaymentDeclined = 5,
		DuplicateTransaction = 6,
		ValidateFailed = 7,
		StoreNotConnected = 8,
		PurchaseMissing = 9
	}
}
