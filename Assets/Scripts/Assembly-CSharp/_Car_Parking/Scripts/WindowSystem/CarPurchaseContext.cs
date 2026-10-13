using System;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class CarPurchaseContext
	{
		public int CarId { get; }

		public Action HideShop { get; }

		public CarPurchaseContext(int carId, Action hideShop)
		{
			CarId = carId;
			HideShop = hideShop;
		}
	}
}
