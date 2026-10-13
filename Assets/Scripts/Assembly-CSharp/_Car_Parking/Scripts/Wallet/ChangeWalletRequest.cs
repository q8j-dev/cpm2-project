namespace _Car_Parking.Scripts.Wallet
{
	public class ChangeWalletRequest
	{
		public int Value;

		public PurchaseData PurchaseData;

		public ChangeWalletRequest(int value, PurchaseData purchaseData)
		{
			Value = value;
			PurchaseData = purchaseData;
		}
	}
}
