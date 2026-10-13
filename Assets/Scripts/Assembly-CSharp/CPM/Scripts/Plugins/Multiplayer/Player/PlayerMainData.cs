using System;

namespace CPM.Scripts.Plugins.Multiplayer.Player
{
	[Serializable]
	public class PlayerMainData
	{
		public string PlayerId;

		public string AccountId;

		public string PlayerName;

		public PlayerMainData(string playerId, string accountId, string playerName)
		{
			PlayerId = playerId;
			AccountId = accountId;
			PlayerName = playerName;
		}

		public override string ToString()
		{
			return "PlayerName: \"" + PlayerName + "\", playerID: \"" + PlayerId + "\", accountId: \"" + AccountId + "\"";
		}
	}
}
