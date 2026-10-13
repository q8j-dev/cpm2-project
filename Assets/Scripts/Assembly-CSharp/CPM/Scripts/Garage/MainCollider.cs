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
		private GarageDoor door;

		[SerializeField]
		private int GarageIndex;

		private IGarageBuyService _garageBuyService;

		[Inject]
		public void Construct(IGarageBuyService garageBuyService)
		{
			_garageBuyService = garageBuyService;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!FlagsStorage.IsPhotoMode && (other.CompareTag("Car") || other.CompareTag("OnlineCar") || other.CompareTag("DriverPerson")) && !AdvancedBoolean.op_True(!_garageBuyService.GetGarageStatus(GarageIndex)))
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
