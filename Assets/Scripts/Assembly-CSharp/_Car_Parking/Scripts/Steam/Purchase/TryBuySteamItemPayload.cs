namespace _Car_Parking.Scripts.Steam.Purchase
{
	public class TryBuySteamItemPayload
	{
		public string TransactionId;

		public string Itemdefid;

		public TryBuySteamItemPayload(string transactionId, string itemdefid)
		{
			TransactionId = transactionId;
			Itemdefid = itemdefid;
		}
	}
}
