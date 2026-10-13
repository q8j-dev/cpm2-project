using Photon.Pun;
using Photon.Realtime;

namespace CPM.Common
{
	public class PhotonUtility
	{
		public static bool TryGetPlayer(int index, out Player player)
		{
			if (PhotonNetwork.CurrentRoom.Players.TryGetValue(index, out player))
			{
				return true;
			}
			return false;
		}
	}
}
