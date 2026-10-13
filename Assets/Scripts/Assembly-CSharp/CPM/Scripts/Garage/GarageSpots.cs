using UnityEngine;
using VContainer;

namespace CPM.Scripts.Garage
{
	public class GarageSpots : MonoBehaviour
	{
		[SerializeField]
		private GarageSpot[] SpawnPoints = new GarageSpot[0];

		[SerializeField]
		private Transform CarSpawnPoint = default;

		[SerializeField]
		private GameObject TunnelVisuals = default;

		[SerializeField]
		private Transform FreePersonPlace = default;

		[SerializeField]
		private Transform InnerGarageTunnelChild = default;

		[Inject]
		public void Construct(GarageController garageController)
		{
			garageController.SpawnPoints = SpawnPoints;
			garageController.CarSpawnPoint = CarSpawnPoint;
			garageController.GarageTunnelVisuals = TunnelVisuals;
			garageController.FreePersonPosition = FreePersonPlace;
			garageController.InnerGarageTunnelChild = InnerGarageTunnelChild;
		}
	}
}
