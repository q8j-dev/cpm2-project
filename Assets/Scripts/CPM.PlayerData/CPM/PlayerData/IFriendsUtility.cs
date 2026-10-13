using System;
using System.Collections.Generic;

namespace CPM.PlayerData
{
	public interface IFriendsUtility
	{
		Func<List<FriendData>> GetEntitiesFriends { get; set; }

		List<FriendData> PrefsFriends { get; }

		List<PrefFriendData> Friends { get; }

		event Action<string> OnAuthorizedFriendDeleted;

		event Action<FriendData> OnAuthorizedFriendAdded;

		event Action<FriendData> OnPrefFriendAdded;

		void RefillFriendsList(bool keepPrefs = false);

		void RemoveFriendWithLocalID(string localID);

		void ClearAll();
	}
}
