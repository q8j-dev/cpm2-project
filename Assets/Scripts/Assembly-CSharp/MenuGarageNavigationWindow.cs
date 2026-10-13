using CPM.Scripts.Garage;
using CPMEngine;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

public class MenuGarageNavigationWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private GameObject LeftRightButtons;

	[SerializeField]
	private QuickCarSettings QuickCarSettings;

	[SerializeField]
	private GameObject[] CarControllerHUD;

	[SerializeField]
	private GameObject[] PersonControllerHUD;

	[SerializeField]
	private GameObject RoofButtonHUD;

	private SceneControl _sceneControl;

	private MenuLevelParent _menuLevelParent;

	private GarageController _garageController;

	private CameraManager _cameraManager;

	private InputItemsProvider _controlItemsProvider;

	private CoreGameManager _coreGameManager;

	private SoundControllerExtension _carInOutSound;

	private EventRegistry _eventRegistry;

	private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

	private RegistryHandle<ScriptableEvent> _toRoadHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToRoad);

	private const int CountFramesToWaitLoading = 3;

	protected override bool ChangeRootGameInputGroup => true;

	[Inject]
	public void Construct(EventRegistry eventRegistry, SceneControl sceneControl, ManagerUI managerUI, InputItemsProvider controlItemsProvider, MenuLevelParent menuLevelParent, GarageController garageController, CoreGameManager coreGameManager, CameraManager cameraManager, SoundControllerExtension carInOutSound)
	{
		_carInOutSound = carInOutSound;
		_coreGameManager = coreGameManager;
		_controlItemsProvider = controlItemsProvider;
		_cameraManager = cameraManager;
		_garageController = garageController;
		_menuLevelParent = menuLevelParent;
		_sceneControl = sceneControl;
		_managerUI = managerUI;
		_eventRegistry = eventRegistry;
	}

	private new void Awake()
	{
		Init();
		if (FlagsStorage.OutOfCar)
		{
			UpdateHUD(isInCar: false);
		}
		else
		{
			_managerUI.ChangeBalancePanelActiveState?.Invoke(obj: true);
		}
	}

	protected override void OnEnable()
	{
		WindowInputGroup = (FlagsStorage.OutOfCar ? InputGroupsEnum.Person : InputGroupsEnum.CarFree);
		base.OnEnable();
	}

	public override void SetAdditionalData(string data)
	{
		base.SetAdditionalData(data);
		bool correctCamera = data != "PhoneWindow";
		InitBackButton();
		_sceneControl.MenuGarageNavigationWindow = this;
		_managerUI.OnActivateBalancePanel?.Invoke(obj: false);
		PrepareCar(correctCamera);
		if (!FlagsStorage.OutOfCar)
		{
			GameObject[] personControllerHUD = PersonControllerHUD;
			for (int i = 0; i < personControllerHUD.Length; i++)
			{
				personControllerHUD[i].SetActive(value: false);
			}
		}
	}

	private void Init()
	{
		_garageController.MenuGarageNavigationWindow = this;
		_menuLevelParent.LeftRight = LeftRightButtons;
		_changeCameraHandle.Get(_eventRegistry).Subscribe(ChangeCameraPosition);
		_toRoadHandle.Get(_eventRegistry).Subscribe(ResetCarPositionForSubscription);
		V.CoreGameManager.SetSteering();
		_garageController.UpdateCarEnterSubscriptions(toSubscribe: true);
	}

	private void ResetCarPositionForSubscription()
	{
		ResetCarPosition();
	}

	private void PrepareCar(bool correctCamera = true)
	{
		if ((bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<VehicleData>(out var component))
		{
			_garageController.EnableCarPhysics(component, isEnabled: true);
		}
		if (correctCamera)
		{
			ChangeInCarCamera();
		}
		_controlItemsProvider.TouchField.SetActive(active: false);
	}

	public void UpdateHUD(bool isInCar, bool isOpenCloseRoof = false)
	{
		if (isInCar)
		{
			_managerUI.ChangeBalancePanelActiveState?.Invoke(obj: true);
			PrepareCar();
		}
		GameObject[] carControllerHUD = CarControllerHUD;
		for (int i = 0; i < carControllerHUD.Length; i++)
		{
			carControllerHUD[i].SetActive(isInCar);
		}
		carControllerHUD = PersonControllerHUD;
		for (int i = 0; i < carControllerHUD.Length; i++)
		{
			carControllerHUD[i].SetActive(!isInCar);
		}
		RoofButtonHUD?.SetActive(isOpenCloseRoof);
		if ((bool)_menuLevelParent && (bool)_menuLevelParent.HeadLampIconUI)
		{
			_menuLevelParent.HeadLampIconUI.gameObject.SetActive(isInCar);
		}
	}

	public void UpdateHUDClickDriverButton()
	{
		GameObject[] carControllerHUD = CarControllerHUD;
		for (int i = 0; i < carControllerHUD.Length; i++)
		{
			carControllerHUD[i].SetActive(FlagsStorage.OutOfCar);
		}
		if ((bool)_menuLevelParent && (bool)_menuLevelParent.HeadLampIconUI)
		{
			_menuLevelParent.HeadLampIconUI.gameObject.SetActive(FlagsStorage.OutOfCar);
		}
	}

	public void ChangeCameraPosition()
	{
		if (FlagsStorage.OutOfCar)
		{
			V.CurrentCamIndex = ++V.CurrentCamIndex % 5;
			V.CoreGameManager.carCamerasController.SetCamera(V.CurrentCamIndex, V.DriverPerson.transform, newTarget: true);
		}
		else
		{
			_garageController.IsCarOutsideCamMode = !_garageController.IsCarOutsideCamMode;
			ChangeInCarCamera();
		}
	}

	public bool TrySuspendInCarFirstPersonCamera()
	{
		if (FlagsStorage.OutOfCar || !V.LastUsedCar)
		{
			return false;
		}
		if (_garageController.IsCarOutsideCamMode || _cameraManager.CurrentCameraType != RacingCameraType.InCar)
		{
			return false;
		}
		_garageController.IsCarOutsideCamMode = true;
		ChangeInCarCamera();
		return true;
	}

	public void RestoreInCarFirstPersonCamera()
	{
		_garageController.IsCarOutsideCamMode = false;
		if (!FlagsStorage.OutOfCar && (bool)V.LastUsedCar)
		{
			ChangeInCarCamera();
		}
	}

	public void ResetCarPosition(bool tryTurnOffEngine = false)
	{
		if (!V.DriverPerson || !V.DriverPerson.TryGetComponent<MainPerson>(out var component) || ((bool)component.PersonAnimationsController && component.PersonAnimationsController.AnimationService.IsAnimatingPlayer()))
		{
			return;
		}
		if ((bool)V.Car && tryTurnOffEngine && V.Car.TryGetComponent<Powertrain>(out var component2) && component2.IsEngineWorks)
		{
			component2.ShutdownEngine();
		}
		if (FlagsStorage.OutOfCar)
		{
			if ((bool)V.LastUsedCar && V.DriverPerson.transform.position.y < V.LastUsedCar.transform.position.y - 10f)
			{
				_garageController.ResetCarToSavedPosition();
			}
			GaragePersonReturn.PlaceAtLastCarDriverDoor(V.DriverPerson);
		}
		else if (!_garageController.IsInElevator)
		{
			_garageController.ResetCarPosition();
		}
	}

	public void SaveCarsPosition()
	{
		if (!_garageController.IsInElevator)
		{
			_garageController.SaveCarsPositions();
		}
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			base.Close();
			_managerUI.OnActivateBalancePanel?.Invoke(obj: true);
			_garageController.UpdateCarEnterSubscriptions(toSubscribe: false);
			_cameraManager.garageCam.ResetCamToDefault();
			_changeCameraHandle.Get(_eventRegistry)?.Unsubscribe(ChangeCameraPosition);
			_toRoadHandle.Get(_eventRegistry)?.Unsubscribe(ResetCarPositionForSubscription);
		}
	}

	private void ChangeInCarCamera()
	{
		if (_garageController.IsCarOutsideCamMode)
		{
			_cameraManager.ActivateCamera(RacingCameraType.ForwardView, V.LastUsedCar.transform);
			_carInOutSound.SetOutCarSound();
			if ((bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<VehicleData>(out var component) && CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low)
			{
				component.InteriorsComposition.ShowLowInterior();
			}
			return;
		}
		VehicleData component2 = V.LastUsedCar.GetComponent<VehicleData>();
		_cameraManager.ActivateCamera(RacingCameraType.InCar, V.LastUsedCar.GetComponent<DriverController>().camInsideView.transform);
		_carInOutSound.UpdateInCarSound(component2.CarId);
		if (!component2.InteriorsComposition.Interior)
		{
			component2.InteriorsComposition.TryRestoreSavedInterior();
		}
		else
		{
			component2.InteriorsComposition.ShowHighInterior();
		}
	}

	public void SetFreeCamera()
	{
		if ((bool)V.LastUsedCar)
		{
			_garageController.IsCarOutsideCamMode = true;
			_cameraManager.ActivateCamera(RacingCameraType.ForwardView, V.LastUsedCar.transform);
			AudioWrapper.SetState(SoundController.StateCarInOut, SoundController.OutCar);
			if (V.LastUsedCar.TryGetComponent<VehicleData>(out var component) && CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low)
			{
				component.InteriorsComposition.ShowLowInterior();
			}
		}
	}

	public void SetInsideCamera()
	{
		if (!V.LastUsedCar)
		{
			return;
		}
		VehicleData component = V.LastUsedCar.GetComponent<VehicleData>();
		if ((bool)component && V.LastUsedCar.TryGetComponent<DriverController>(out var component2) && (bool)component2.camInsideView)
		{
			_garageController.IsCarOutsideCamMode = false;
			_cameraManager.ActivateCamera(RacingCameraType.InCar, component2.camInsideView.transform);
			_carInOutSound.UpdateInCarSound(component.CarId);
			if (!component.InteriorsComposition.Interior)
			{
				component.InteriorsComposition.TryRestoreSavedInterior();
			}
			else
			{
				component.InteriorsComposition.ShowHighInterior();
			}
		}
	}

	private async UniTaskVoid OnClose()
	{
		if (!V.LastUsedCar)
		{
			return;
		}
		VehicleData vehicleData = V.LastUsedCar.GetComponent<VehicleData>();
		if (!vehicleData)
		{
			return;
		}
		if (V.LastUsedCar.TryGetComponent<Powertrain>(out var component) && !component.IsAutomaticGearbox)
		{
			component.Gear = 0;
			if (V.LastUsedCar.TryGetComponent<CarInputManager>(out var component2))
			{
				component2.SelectorState = 0;
			}
		}
		if (V.DriverPerson.TryGetComponentInChildren<PersonAnimationsController>(out var component3))
		{
			bool isSittingIntoCarAnimate = component3.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate;
			bool isGettingOutOfTheCarAnimate = component3.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
			if (isSittingIntoCarAnimate)
			{
				_garageController.EnableCarPhysics(vehicleData, isEnabled: true);
				component3.SitGetOutCarSystem.CarAnimationStopper.ForceSit();
				await UniTask.DelayFrame(3);
			}
			else if (isGettingOutOfTheCarAnimate)
			{
				_garageController.EnableCarPhysics(vehicleData, isEnabled: true);
				component3.SitGetOutCarSystem.CarAnimationStopper.ForceGetOut();
				await UniTask.DelayFrame(3);
			}
			if (V.LastUsedCar.TryGetComponent<CarAnimationsController>(out var component4))
			{
				component4.CloseMainOpenedDoors();
			}
		}
		if (FlagsStorage.OutOfCar)
		{
			await _coreGameManager.CarOutInControl.InCar(V.DriverPerson, V.LastUsedCar, isSendRpc: true, playDoorSound: true, whenBack: true);
		}
		vehicleData = TryUpdateVehicleData(vehicleData);
		_carInOutSound.SetOutCarSound();
		_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, vehicleData.transform);
		_garageController.DisableCarPhysicsDelayed(vehicleData, 1f);
	}

	private VehicleData TryUpdateVehicleData(VehicleData vehicleData)
	{
		if (!V.LastUsedCar)
		{
			return vehicleData;
		}
		if (V.LastUsedCar.TryGetComponent<VehicleData>(out var component) && (bool)component && component != vehicleData)
		{
			vehicleData = component;
		}
		return vehicleData;
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		if (!_garageController.IsInElevator)
		{
			SaveCarsPosition();
			OnClose().Forget();
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 1f);
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Dyno, 1f);
			_managerUI.ShowGarageWindow();
			FinishWalkExit();
		}
	}

	public bool TryExitWalkForAnimationShop()
	{
		if (_garageController.IsInElevator)
		{
			return false;
		}
		SaveCarsPosition();
		OnClose().Forget();
		GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 1f);
		GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Dyno, 1f);
		FinishWalkExit();
		return true;
	}

	private void FinishWalkExit()
	{
		QuickCarSettings.DisableAllLights();
		if ((bool)_garageController)
		{
			_garageController.SetOffCheckers();
		}
		Close();
	}

	public QuickCarSettings GetQuickCarSettings()
	{
		return QuickCarSettings;
	}
}
