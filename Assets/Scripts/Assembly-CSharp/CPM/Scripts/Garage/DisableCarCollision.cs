using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

namespace CPM.Scripts.Garage
{
	[RequireComponent(typeof(Collider))]
	public class DisableCarCollision : MonoBehaviour
	{
		private OnlineCollectionsProvider _onlineCollectionsProvider;

		[Inject]
		public void Construct(OnlineCollectionsProvider onlineCollectionsProvider)
		{
			_onlineCollectionsProvider = onlineCollectionsProvider;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (PhotonNetwork.IsConnected && (other.CompareTag("Car") || other.CompareTag("OnlineCar")))
			{
				PlayerDatas.IgnoreCollisionWithOtherCars(other.transform.parent.gameObject, ignore: true, _onlineCollectionsProvider, out var _);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Car") || other.CompareTag("OnlineCar"))
			{
				FlagsStorage.CanEnterGarage = true;
				if (PhotonNetwork.IsConnected)
				{
					PlayerDatas.IgnoreCollisionWithOtherCars(other.transform.parent.gameObject, ignore: false, _onlineCollectionsProvider, out var _);
				}
			}
		}
	}
}
