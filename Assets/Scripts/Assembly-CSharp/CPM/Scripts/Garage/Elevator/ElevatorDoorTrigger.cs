using Racing.CameraManagement;
using UnityEngine;
using VContainer;

namespace CPM.Scripts.Garage.Elevator
{
	public class ElevatorDoorTrigger : MonoBehaviour
	{
		[SerializeField]
		private int DoorId;

		[SerializeField]
		private Elevator Elevator;

		[SerializeField]
		private ElevatorDoorTrigger TargetDoor;

		[SerializeField]
		private ElevatorDoor ElevatorDoor;

		private CameraManager _cameraManager;

		private GarageController _garageController;

		[Inject]
		public void Construct(CameraManager cameraManager, GarageController garageController)
		{
			_garageController = garageController;
			_cameraManager = cameraManager;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.parent.TryGetComponent<VehicleData>(out var component))
			{
				Transform transform = base.transform;
				Vector3 normalized = (component.transform.position - transform.position).normalized;
				if (Vector3.Dot(transform.forward, normalized) < 0f)
				{
					ElevatorDoor.Open();
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.transform.parent.TryGetComponent<VehicleData>(out var component))
			{
				return;
			}
			ElevatorDoor.Close();
			Transform transform = base.transform;
			Vector3 vector = component.transform.position - transform.position;
			Vector3 normalized = vector.normalized;
			DriverActionController component2 = V.DriverPerson.GetComponent<DriverActionController>();
			if (Vector3.Dot(transform.forward, normalized) < 0f || vector.magnitude > 5f)
			{
				if (_garageController.IsCarOutsideCamMode)
				{
					_cameraManager.ActivateCamera(RacingCameraType.ForwardView, component.transform);
				}
				_garageController.IsInElevator = false;
				if ((bool)component2)
				{
					component2.IsExitCarBlocked = false;
				}
				return;
			}
			_garageController.IsInElevator = true;
			if ((bool)component2)
			{
				component2.IsExitCarBlocked = true;
			}
			Elevator.TranslatePodiumToDoor(TargetDoor.DoorId, component);
			if (_garageController.IsCarOutsideCamMode)
			{
				_cameraManager.ActivateCamera(RacingCameraType.InCar, component.GetComponent<DriverController>().camInsideView.transform);
			}
		}
	}
}
