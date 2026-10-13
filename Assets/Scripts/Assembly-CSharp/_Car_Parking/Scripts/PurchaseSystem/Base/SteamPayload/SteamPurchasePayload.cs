namespace _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload
{
	public class SteamPurchasePayload
	{
		public string PaymentProductId;

		public string TransactionId;

		public string Itemdefid;

		public string Name;

		public string Platform;

		public int Type;

		public SteamPurchasePayload(string purchaseId, string transactionId, string itemdefid, string name, PurchasePayloadType type)
		{
			PaymentProductId = purchaseId;
			TransactionId = transactionId;
			Itemdefid = itemdefid;
			Name = name;
			Type = (int)type;
			Platform = "Steam";
		}
	}
}
