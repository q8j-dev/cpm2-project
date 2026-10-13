using UnityEngine;
using VContainer;

namespace CPM.Scripts.Garage
{
	public class GarageSpots : MonoBehaviour
	{
		[SerializeField]
		private GarageSpot[] SpawnPoints = new GarageSpot[0];

		[SerializeField]
		private Transform CarSpawnPoint;

		[SerializeField]
		private GameObject TunnelVisuals;

		[SerializeField]
		private Transform FreePersonPlace;

		[SerializeField]
		private Transform InnerGarageTunnelChild;

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
