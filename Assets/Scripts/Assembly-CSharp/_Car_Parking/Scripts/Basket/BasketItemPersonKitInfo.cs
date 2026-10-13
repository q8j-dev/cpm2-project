using CodeStage.AntiCheat.ObscuredTypes;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.Basket
{
	public class BasketItemPersonKitInfo : BasketItemInfoBase
	{
		public int Id { get; set; }

		public KitType KitType { get; set; }

		public int PackId { get; set; }

		public BasketItemPersonKitInfo(ObscuredInt price, int id, KitType kitType, bool isLocked, int packId = -1)
			: base(price, isLocked)
		{
			Id = id;
			KitType = kitType;
			PackId = packId;
		}
	}
}
