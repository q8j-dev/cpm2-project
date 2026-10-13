namespace _Car_Parking.Scripts.EngineTuning
{
	public class CarPrices
	{
		private const int Car7 = 10000;

		private const int Car10 = 10000;

		private const int Car27 = 10000;

		private const int Car32 = 10000;

		private const int Car41 = 10000;

		private const int Car58 = 10000;

		private const int Car162 = 10000;

		private const int Car178 = 10000;

		private const int Car198 = 10000;

		private const int Car202 = 10000;

		private const int Car203 = 10000;

		public static bool IsConstGetPrice(int carId, out int price)
		{
			switch (carId)
			{
			case 7:
				price = 10000;
				return true;
			case 10:
				price = 10000;
				return true;
			case 27:
				price = 10000;
				return true;
			case 32:
				price = 10000;
				return true;
			case 41:
				price = 10000;
				return true;
			case 58:
				price = 10000;
				return true;
			case 162:
				price = 10000;
				return true;
			case 178:
				price = 10000;
				return true;
			case 198:
				price = 10000;
				return true;
			case 202:
				price = 10000;
				return true;
			case 203:
				price = 10000;
				return true;
			default:
				price = 10000;
				return false;
			}
		}
	}
}
