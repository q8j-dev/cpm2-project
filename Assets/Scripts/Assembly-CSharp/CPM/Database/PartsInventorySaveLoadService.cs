using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Inventory;

namespace CPM.Database
{
	public class PartsInventorySaveLoadService : SaveInventoryServiceBase<InventoryData, InventoryPartsHashes>
	{
		public const string SendFunctionName = "SavePartsInventory24_1";

		public override string SendFunction => "SavePartsInventory24_1";

		public override string HashesPrefsKey => "InventoryPartsHashes";
	}
}
