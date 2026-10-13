using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Database.PlayerData;

namespace CPM.Database
{
	public class FriendsSaveLoadService : SaveInventoryServiceBase<FriendsCollection, FriendsCollectionHashes>
	{
		public const string SendFunctionName = "SaveFriends24_1";

		public override string SendFunction => "SaveFriends24_1";

		public override string HashesPrefsKey => "FriendsHashes";
	}
}
