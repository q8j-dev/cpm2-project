using CPM.PlayerData;
using Photon.Pun;

namespace _Car_Parking.Scripts.Multiplayer
{
	public static class MultiPlayerAccountHelper
	{
		public static string GetFirebaseId(int actorNumber, MultiPlayer multiPlayer, PlayerDataStorage playerDataStorage)
		{
			if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				return playerDataStorage.AccountID;
			}
			if ((bool)multiPlayer)
			{
				MultiPersonControl multiPersonControl = multiPlayer.GetMultiPersonControl(actorNumber);
				if ((bool)multiPersonControl)
				{
					return multiPersonControl.ThisPlayerAccountID;
				}
			}
			return "";
		}

		public static string GetLocalId(int actorNumber, MultiPlayer multiPlayer, PlayerDataStorage playerDataStorage)
		{
			if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				return playerDataStorage.PlayerID;
			}
			if ((bool)multiPlayer)
			{
				MultiPersonControl multiPersonControl = multiPlayer.GetMultiPersonControl(actorNumber);
				if ((bool)multiPersonControl)
				{
					return multiPersonControl.ThisPlayerID;
				}
			}
			return "";
		}
	}
}
