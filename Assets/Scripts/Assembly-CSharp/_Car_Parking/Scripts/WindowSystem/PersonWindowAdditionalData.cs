using UnityEngine.Scripting;
using _Car_Parking.Scripts.MenuUI.Person;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	public class PersonWindowAdditionalData
	{
		public enum ParentWindowType
		{
			None = 0,
			MainMenu = 1
		}

		public ParentWindowType ParentWindow;

		public PersonClothsEventType PersonClothsEventType;

		public int Gender = -1;

		public PersonWindowCategoryType? OpenCategory;

		public int? OpenPackId;
	}
}
