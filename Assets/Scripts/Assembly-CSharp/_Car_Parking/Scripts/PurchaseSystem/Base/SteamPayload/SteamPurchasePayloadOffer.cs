namespace _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload
{
	public class SteamPurchasePayloadOffer : SteamPurchasePayload
	{
		public int OfferId;

		public int FreeSlotGarageIndex;

		public SteamPurchasePayloadOffer(string bundleId, string transactionId, int offerId, int freeSlotGarageIndex, string itemdefid, string name)
			: base(bundleId, transactionId, itemdefid, name, PurchasePayloadType.Offer)
		{
			OfferId = offerId;
			FreeSlotGarageIndex = freeSlotGarageIndex;
		}
	}
}
