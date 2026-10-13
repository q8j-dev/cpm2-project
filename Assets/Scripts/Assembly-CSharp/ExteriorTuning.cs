using CPM.Scripts.Garage;
using Racing.CameraManagement;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;

public class ExteriorTuning : MonoBehaviour
{
	private VehicleData _vehicleData;

	private GarageController _garageController;

	private CameraManager _cameraManager;

	private GaragePositionsConfig _garagePositionsConfig;

	private IGarageSelectorService _garageSelectorService;

	private CarInputManager _carInputManager;

	private ManagerUI _managerUI;

	[Inject]
	public void Construct(ManagerUI managerUI, GarageController garageController, CameraManager cameraManager, GaragePositionsConfig garagePositionsConfig, IGarageSelectorService garageSelectorService, CarInputManager carInputManager)
	{
		_cameraManager = cameraManager;
		_garageController = garageController;
		_managerUI = managerUI;
		_garagePositionsConfig = garagePositionsConfig;
		_garageSelectorService = garageSelectorService;
		_carInputManager = carInputManager;
	}

	private Transform GetPaintBoxTransform()
	{
		int currentGarageIndex = _garageSelectorService.CurrentGarageIndex;
		V.CoreGameManager.menuControl.carPosInGarage.localPosition = _garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].CarPaintbox.Position;
		V.CoreGameManager.menuControl.carPosInGarage.localRotation = _garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].CarPaintbox.Rotation;
		return V.CoreGameManager.menuControl.carPosInGarage;
	}

	public void OpenVisualTuning(bool openFromUI)
	{
		GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 3f);
		_vehicleData = V.Car.GetComponent<VehicleData>();
		_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
		Transform paintBoxTransform = GetPaintBoxTransform();
		Vector3 position = paintBoxTransform.position;
		position.y = _vehicleData.transform.position.y;
		V.Car.transform.SetPositionAndRotation(position, paintBoxTransform.rotation);
		_garageController.ClearSpaceForCar(_vehicleData);
		_vehicleData.GetPowertrain.ResetTransmission();
		ResetSteerAndWheelsPositions();
	}

	private void ResetSteerAndWheelsPositions()
	{
		if ((bool)_vehicleData.GetPowertrain)
		{
			_vehicleData.GetPowertrain.SetSteer(0f, useAssistent: false);
			_carInputManager.ResetSteerAndWheelsPosition();
		}
	}

	public void ExitVisualTuning(bool closeFromUI = true)
	{
		GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 3f);
		if (closeFromUI)
		{
			_garageController.SetOffCheckers();
			_garageController.ResetCarToSavedPosition();
			_garageController.CorrectCamera(_vehicleData);
			_garageController.DisableCarPhysicsDelayed(_vehicleData, 2f);
		}
		else
		{
			_managerUI.ShowMenuGarageControllerHUD();
		}
	}
}
