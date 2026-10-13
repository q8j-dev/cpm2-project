namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum PurchaseRealErrorReason
	{
		None = 0,
		ClientNoSlots = 101,
		ClientHashError = 102,
		ClientOfferIsOld = 103,
		ClientUnknown = 104,
		ClientProcessPurchaseAsync = 105,
		StoreNone = 200,
		BackendUnknown = 301,
		BackendItemWasBought = 302,
		BackendPaymentIsBad = 303,
		BackendError = 304
	}
}
