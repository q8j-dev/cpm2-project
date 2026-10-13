namespace _Car_Parking.Scripts.Offers.Reward
{
	public readonly struct PackClothItem
	{
		public readonly int PackId;

		public readonly int TypeId;

		public readonly int ItemId;

		public PackClothItem(int packId, int typeId, int itemId)
		{
			PackId = packId;
			TypeId = typeId;
			ItemId = itemId;
		}
	}
}
