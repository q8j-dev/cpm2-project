namespace _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload
{
	public class SteamPurchasePayloadNewCar : SteamPurchasePayload
	{
		public int CarId;

		public int FreeSlotGarageIndex;

		public SteamPurchasePayloadNewCar(string steamDefId, string transactionId, int carId, int freeSlotGarageIndex, string itemdefid, string name)
			: base(steamDefId, transactionId, itemdefid, name, PurchasePayloadType.NewCar)
		{
			CarId = carId;
			FreeSlotGarageIndex = freeSlotGarageIndex;
		}
	}
}
