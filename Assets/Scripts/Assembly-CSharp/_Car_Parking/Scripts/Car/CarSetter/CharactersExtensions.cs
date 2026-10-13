using System.Collections.Generic;
using CPM.Database;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	public static class CharactersExtensions
	{
		public static List<int> GetBoughtList(this EquipmentsData equipmentsData, int typeId)
		{
			switch (typeId)
			{
			case 0:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.hair ?? (equipmentsData2.hair = new List<int>());
			}
			case 1:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.face ?? (equipmentsData2.face = new List<int>());
			}
			case 2:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.beard ?? (equipmentsData2.beard = new List<int>());
			}
			case 3:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.cap ?? (equipmentsData2.cap = new List<int>());
			}
			case 4:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.mask ?? (equipmentsData2.mask = new List<int>());
			}
			case 5:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.top ?? (equipmentsData2.top = new List<int>());
			}
			case 6:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.gloves ?? (equipmentsData2.gloves = new List<int>());
			}
			case 7:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.bag ?? (equipmentsData2.bag = new List<int>());
			}
			case 8:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.pants ?? (equipmentsData2.pants = new List<int>());
			}
			case 9:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.shoes ?? (equipmentsData2.shoes = new List<int>());
			}
			case 10:
			{
				EquipmentsData equipmentsData2 = equipmentsData;
				return equipmentsData2.glasses ?? (equipmentsData2.glasses = new List<int>());
			}
			default:
				return null;
			}
		}

		public static bool IsItemBought(this EquipmentsData equipmentsData, int typeID, int itemId)
		{
			return equipmentsData.GetBoughtList(typeID).Contains(itemId);
		}

		public static void SetSelectedItem(this EquipmentsData equipmentsData, int typeID, int itemId)
		{
			while (typeID >= equipmentsData.SelectedEquipments.Count)
			{
				equipmentsData.SelectedEquipments.Add(-1);
			}
			equipmentsData.SelectedEquipments[typeID] = itemId;
		}

		public static void BuyItem(this EquipmentsData equipmentsData, int typeID, int itemId)
		{
			List<int> boughtList = equipmentsData.GetBoughtList(typeID);
			if (!boughtList.Contains(itemId))
			{
				boughtList.Add(itemId);
			}
		}
	}
}
