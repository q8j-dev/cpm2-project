using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.PlayerData;

namespace CPM.Database
{
	public class PlayerAnonStorageSaveLoadService : SaveInventoryServiceBase<PlayerStorage, PlayerAnonStorageHashes>
	{
		public const string SendFunctionName = "SavePlayerRecords24_1";

		public override string SendFunction => "SavePlayerRecords24_1";

		public override string HashesPrefsKey => "PlayerStorageHashes";
	}
}
