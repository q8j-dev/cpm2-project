using System;

namespace CPM.PlayerData
{
	[Serializable]
	public class FriendData
	{
		public string userID;

		public string accountID;

		public string playerName;

		public int timestamp;

		public bool online;

		public bool isPrefFriend;

		public int order;

		public FriendData(string userID, string accountID, string playerName)
		{
			this.userID = userID;
			this.accountID = accountID;
			this.playerName = playerName;
		}

		public FriendData()
		{
		}
	}
}
