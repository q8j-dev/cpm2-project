namespace _Car_Parking.Scripts.WindowSystem
{
	public interface ICarPurchaseHandler
	{
		bool TryHandle(CarPurchaseContext context);
	}
}
