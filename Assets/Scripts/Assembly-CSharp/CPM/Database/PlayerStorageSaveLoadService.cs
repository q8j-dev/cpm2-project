using System;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.PlayerData;

namespace CPM.Database
{
	[Preserve]
	public class PlayerStorageSaveLoadService : SaveInventoryServiceBase<PlayerStorage, PlayerStorageHashes>
	{
		public const string SendFunctionName = "SavePlayerRecords24_1";

		public override string SendFunction => "SavePlayerRecords24_1";

		public override string HashesPrefsKey => "PlayerStorageHashes";

		[Preserve]
		public PlayerStorageSaveLoadService()
		{
		}

		protected override IDisposable ExcludeTransientSendData(PlayerStorage data)
		{
			return base.PresetGrantSession?.Exclude(data);
		}
	}
}
