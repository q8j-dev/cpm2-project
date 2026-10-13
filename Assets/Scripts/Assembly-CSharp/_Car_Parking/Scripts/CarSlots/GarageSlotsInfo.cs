namespace _Car_Parking.Scripts.CarSlots
{
	public readonly struct GarageSlotsInfo
	{
		public readonly CarSlotsInfo[] CarSlotsInfoArray;

		public GarageSlotsInfo(CarSlotsInfo[] carSlotsInfoArray)
		{
			CarSlotsInfoArray = carSlotsInfoArray;
		}
	}
}
