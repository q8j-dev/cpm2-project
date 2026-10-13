using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public class SellExchangeFreePunCallbacks : MonoBehaviourPunCallbacks
	{
		private ISellExchangeFreeService _sellExchangeFreeService;

		private MultiPlayer _multiPlayer;

		[Inject]
		public void Construct(ISellExchangeFreeService sellExchangeFreeService, MultiPlayer multiPlayer)
		{
			_sellExchangeFreeService = sellExchangeFreeService;
			_multiPlayer = multiPlayer;
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			GameObject carByOwnerID = _multiPlayer.GetCarByOwnerID(otherPlayer.ActorNumber);
			if ((bool)carByOwnerID)
			{
				VehicleData component = carByOwnerID.GetComponent<VehicleData>();
				if (component.СurrentSellCarTrigger != null)
				{
					_sellExchangeFreeService.SellCarExit(component.СurrentSellCarTrigger);
				}
			}
		}
	}
}
