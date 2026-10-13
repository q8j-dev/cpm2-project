namespace _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload
{
	public class SteamPurchasePayloadCoin : SteamPurchasePayload
	{
		public SteamPurchasePayloadCoin(string steamDefId, string transactionId, string itemdefid, string name)
			: base(steamDefId, transactionId, itemdefid, name, PurchasePayloadType.Coins)
		{
		}
	}
}
