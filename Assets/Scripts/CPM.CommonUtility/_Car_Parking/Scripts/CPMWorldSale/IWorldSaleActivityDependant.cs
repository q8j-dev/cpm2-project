namespace _Car_Parking.Scripts.CPMWorldSale
{
	public interface IWorldSaleActivityDependant
	{
		void SetWorldSaleActivity(int coinLimit);

		void ProcessWorldSaleSubscription(int reward);
	}
}
