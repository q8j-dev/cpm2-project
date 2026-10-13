namespace _Car_Parking.Scripts.Offers.Reward
{
	public interface ISuitInventory
	{
		bool IsBought(int bodyType, int typeId, int itemId);

		void Buy(int bodyType, int typeId, int itemId);
	}
}
