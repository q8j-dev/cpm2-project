using UnityEngine.Scripting;
using _Car_Parking.Scripts.Database.PlayerData;

namespace CPM.Database
{
	[Preserve]
	public class PlatesSaveLoadService : SaveInventoryServiceBase<PlatesDataForDatabase, PlatesHashes>
	{
		public const string SendFunctionName = "SavePlateVinyls24_1";

		public override string SendFunction => "SavePlateVinyls24_1";

		public override string HashesPrefsKey => "PlatesHashes";

		[Preserve]
		public PlatesSaveLoadService()
		{
		}
	}
}
