using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Offers.Reward;

namespace _Car_Parking.Scripts.Characters
{
	public sealed class CharactersSuitInventory : ISuitInventory
	{
		private readonly CommonProvider _commonProvider;

		public CharactersSuitInventory(CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
		}

		public bool IsBought(int bodyType, int typeId, int itemId)
		{
			return _commonProvider.CharactersService.GetEquipmentsData(bodyType).IsItemBought(typeId, itemId);
		}

		public void Buy(int bodyType, int typeId, int itemId)
		{
			_commonProvider.CharactersService.GetEquipmentsData(bodyType).BuyItem(typeId, itemId);
		}
	}
}
