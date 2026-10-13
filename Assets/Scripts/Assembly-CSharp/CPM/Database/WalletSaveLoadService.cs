using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.PlayerData;

namespace CPM.Database
{
	[Preserve]
	public class WalletSaveLoadService : SaveInventoryServiceBase<WalletData, WalletDataHashes>
	{
		public const string SendFunctionName = "SaveWalletData24_1";

		public override string SendFunction => "SaveWalletData24_1";

		public override string HashesPrefsKey => "WalletDataHashes";

		[Preserve]
		public WalletSaveLoadService()
		{
		}
	}
}
