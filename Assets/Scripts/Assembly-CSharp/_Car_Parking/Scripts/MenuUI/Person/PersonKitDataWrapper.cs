using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.MenuUI.Person
{
	public struct PersonKitDataWrapper
	{
		public PersonElements.PersonKitData PersonKitData;

		public KitType KitType;

		public PersonClothsEventType PersonClothsEventType;

		public int PackId;

		public PersonKitDataWrapper(PersonElements.PersonKitData personKitData, KitType kitType, PersonClothsEventType personClothsEventType)
		{
			PersonKitData = personKitData;
			KitType = kitType;
			PersonClothsEventType = personClothsEventType;
			PackId = -1;
		}

		public PersonKitDataWrapper(PersonElements.PersonKitData personKitData, KitType kitType, PersonClothsEventType personClothsEventType, int packId)
		{
			PersonKitData = personKitData;
			KitType = kitType;
			PersonClothsEventType = personClothsEventType;
			PackId = packId;
		}
	}
}
