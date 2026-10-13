using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.Basket
{
	public class BasketItemInfoBase
	{
		public ObscuredInt Price { get; set; }

		public bool IsLocked { get; set; }

		public BasketItemInfoBase(ObscuredInt price, bool isLocked)
		{
			Price = price;
			IsLocked = isLocked;
		}
	}
}
