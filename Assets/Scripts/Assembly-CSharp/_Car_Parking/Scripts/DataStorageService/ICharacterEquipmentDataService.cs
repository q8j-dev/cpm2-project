using CPM.Database;

namespace _Car_Parking.Scripts.DataStorageService
{
	public interface ICharacterEquipmentDataService
	{
		EquipmentsData Male { get; set; }

		EquipmentsData Female { get; set; }

		int Gender { get; set; }

		void Save();
	}
}
