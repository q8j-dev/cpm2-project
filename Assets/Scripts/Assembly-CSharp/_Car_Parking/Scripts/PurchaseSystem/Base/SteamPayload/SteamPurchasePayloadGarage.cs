namespace _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload
{
	public class SteamPurchasePayloadGarage : SteamPurchasePayload
	{
		public SteamPurchasePayloadGarage(string steamDefId, string transactionId, string itemdefid, string name)
			: base(steamDefId, transactionId, itemdefid, name, PurchasePayloadType.Garage)
		{
		}
	}
}
