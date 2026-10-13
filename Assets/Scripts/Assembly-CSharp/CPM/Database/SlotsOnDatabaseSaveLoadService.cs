using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Database.PlayerData;

namespace CPM.Database
{
	[Preserve]
	public class SlotsOnDatabaseSaveLoadService : SaveInventoryServiceBase<SlotsOnDatabaseCollection, SlotsOnDatabaseHashes>
	{
		public const string SendFunctionName = "SaveSlotsCollection24_1";

		public override string SendFunction => "SaveSlotsCollection24_1";

		public override string HashesPrefsKey => "SlotsDatabaseHashes1";

		[Preserve]
		public SlotsOnDatabaseSaveLoadService()
		{
		}
	}
}
