namespace _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload
{
	public class SteamPurchasePayloadMoney : SteamPurchasePayload
	{
		public SteamPurchasePayloadMoney(string steamDefId, string transactionId, string itemdefid, string name)
			: base(steamDefId, transactionId, itemdefid, name, PurchasePayloadType.Money)
		{
		}
	}
}
