namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchasePayloadMoney : PurchasePayload
	{
		public PurchasePayloadMoney(string bundleID, string transactionId, string attemptId)
			: base(bundleID, transactionId, attemptId, PurchasePayloadType.Money)
		{
		}
	}
}
