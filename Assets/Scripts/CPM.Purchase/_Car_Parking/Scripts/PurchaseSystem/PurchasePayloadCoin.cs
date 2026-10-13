namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchasePayloadCoin : PurchasePayload
	{
		public PurchasePayloadCoin(string bundleID, string transactionId, string attemptId)
			: base(bundleID, transactionId, attemptId, PurchasePayloadType.Coins)
		{
		}
	}
}
