namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchasePayloadGarage : PurchasePayload
	{
		public PurchasePayloadGarage(string bundleID, string transactionId, string attemptId)
			: base(bundleID, transactionId, attemptId, PurchasePayloadType.Garage)
		{
		}
	}
}
