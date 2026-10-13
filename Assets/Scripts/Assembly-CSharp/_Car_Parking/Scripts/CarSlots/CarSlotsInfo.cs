namespace _Car_Parking.Scripts.CarSlots
{
	public readonly struct CarSlotsInfo
	{
		public readonly int Occupied;

		public readonly int Purchased;

		public readonly bool IsBought;

		public CarSlotsInfo(int occupied, int purchased, bool isBought)
		{
			Occupied = occupied;
			Purchased = purchased;
			IsBought = isBought;
		}
	}
}
