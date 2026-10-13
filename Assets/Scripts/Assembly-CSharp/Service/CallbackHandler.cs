using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Service.Delivery;

namespace Service
{
	public class CallbackHandler : MonoBehaviourPunCallbacks
	{
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
			if (!(V.DriverPerson == null))
			{
				TrailerController trailerController = Global.TaskManager?.DeliveryManager?.GetTrailerController(targetPlayer.ActorNumber);
				if ((bool)trailerController)
				{
					trailerController.containerController?.SetItemsLocation(changedProps, state: false);
				}
			}
		}
	}
}
