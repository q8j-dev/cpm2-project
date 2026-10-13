#nullable enable annotations
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	[Preserve]
	public class CarBuyResponseRaw
	{
		public string? CI;

		public string? M;

		public string? C;

		[Preserve]
		public CarBuyResponseRaw()
		{
		}

		public override string ToString()
		{
			return "Money: " + M + " Coins: " + C + " CI: " + CI;
		}
	}
}
