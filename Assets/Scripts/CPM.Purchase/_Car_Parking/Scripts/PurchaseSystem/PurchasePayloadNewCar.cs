namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchasePayloadNewCar : PurchasePayload
	{
		public int CarId;

		public int FreeSlotGarageIndex;

		public PurchasePayloadNewCar(string bundleID, string transactionId, string attemptId, int carId, int freeSlotGarageIndex)
			: base(bundleID, transactionId, attemptId, PurchasePayloadType.NewCar)
		{
			CarId = carId;
			FreeSlotGarageIndex = freeSlotGarageIndex;
		}
	}
}
