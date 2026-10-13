using CPM.Scripts.Garage.GarageBuyer;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;

namespace CPM.Scripts.Garage
{
	[RequireComponent(typeof(Collider))]
	public class MainCollider : MonoBehaviour
	{
		[SerializeField]
		private GarageDoor door = default;

		[SerializeField]
		private int GarageIndex = default;

		private IGarageBuyService _garageBuyService;

		[Inject]
		public void Construct(IGarageBuyService garageBuyService)
		{
			_garageBuyService = garageBuyService;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!FlagsStorage.IsPhotoMode && (other.CompareTag("Car") || other.CompareTag("OnlineCar") || other.CompareTag("DriverPerson")) && (!_garageBuyService.GetGarageStatus(GarageIndex) ? false : true))
			{
				door.Open();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!FlagsStorage.IsPhotoMode && (other.CompareTag("Car") || other.CompareTag("OnlineCar") || other.CompareTag("DriverPerson")))
			{
				door.Close();
			}
		}
	}
}
