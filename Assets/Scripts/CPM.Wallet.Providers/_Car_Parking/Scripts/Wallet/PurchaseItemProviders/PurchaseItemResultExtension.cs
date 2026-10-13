namespace _Car_Parking.Scripts.Wallet.PurchaseItemProviders
{
	public static class PurchaseItemResultExtension
	{
		public static bool IsConfirmPendingPurchase(int result)
		{
			return IsConfirmPendingPurchase((PurchaseItemResultStatus)result);
		}

		public static bool IsConfirmPendingPurchase(PurchaseItemResultStatus result)
		{
			return result != PurchaseItemResultStatus.Error;
		}
	}
}
