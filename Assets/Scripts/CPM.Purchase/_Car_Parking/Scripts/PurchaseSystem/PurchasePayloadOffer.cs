namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchasePayloadOffer : PurchasePayload
	{
		public int OfferId;

		public int FreeSlotGarageIndex;

		public PurchasePayloadOffer(string bundleID, string transactionId, string attemptId, int offerId, int freeSlotGarageIndex)
			: base(bundleID, transactionId, attemptId, PurchasePayloadType.Offer)
		{
			OfferId = offerId;
			FreeSlotGarageIndex = freeSlotGarageIndex;
		}
	}
}
