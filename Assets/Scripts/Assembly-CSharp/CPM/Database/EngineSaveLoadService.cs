using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Inventory;

namespace CPM.Database
{
	public class EngineSaveLoadService : SaveInventoryServiceBase<EngineKitsInventoryData, InventoryEnginesPartsHashes>
	{
		public const string SendFunctionName = "SaveEngineInventory24_1";

		public override string SendFunction => "SaveEngineInventory24_1";

		public override string HashesPrefsKey => "InventoryEnginesHashes";
	}
}
