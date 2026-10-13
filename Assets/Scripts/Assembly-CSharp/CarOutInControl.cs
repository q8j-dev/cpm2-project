using System;
using System.Threading;
using CPMEngine;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using Photon.Pun;
using RootMotion.FinalIK;
using Service;
using Service.Delivery;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WorldInteraction;

public class CarOutInControl : MonoBehaviour
{
	public LockCarButtonController LockCarButtonController;

	private PlayerComponent _playerComponent;

	private PersonCameraControll _pcc;

	private ISlotService _slotService;

	private CarInputManager _carInputManager;

	private OptionalDependency<FreeTimeAttackController> _timeAttackControllerDep;

	private FreeTimeAttackController _timeAttackController;

	private bool _isDestroyed;

	private IDisposable _resetIsKinematicDisposable;

	private IDisposable _isKinematicDisposable;

	private CancellationTokenSource _checkCarNearCts;

	public LevelsProvider LevelsProvider { get; set; }

	public FreeDriveProvider FreeDriveProvider { get; set; }

	public InputItemsProvider ControlItemsProvider { get; set; }

	public GameObject CarToEnter { get; private set; }

	public event Action<bool> OnCarMenuSet;

	[Inject]
	public void Construct(ISlotService slotService, CarInputManager carInputManager, OptionalDependency<FreeTimeAttackController> timeAttackController, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider)
	{
		_slotService = slotService;
		_carInputManager = carInputManager;
		_timeAttackControllerDep = timeAttackController;
		FreeDriveProvider = freeDriveProvider;
		LevelsProvider = levelsProvider;
	}

	private void Start()
	{
		_timeAttackController = _timeAttackControllerDep?.Value;
		TryGetComponent<PersonCameraControll>(out _pcc);
		_checkCarNearCts = new CancellationTokenSource();
		CheckCarNearLoop(_checkCarNearCts.Token).Forget();
	}

	private async UniTaskVoid CheckCarNearLoop(CancellationToken ct)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		while (!ct.IsCancellationRequested)
		{
			CheckCarNear();
			await UniTask.Delay(TimeSpan.FromMilliseconds(300.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
	}

	private float PersonOutHeight(Vector3 v3, GameObject car)
	{
		Vector3 direction = car.transform.TransformDirection(Vector3.down);
		v3.y = car.transform.localPosition.y;
		float num = 2.5f;
		Collider[] array = Physics.OverlapBox(v3 + V.GlobalOffset, Vector3.one * 0.01f, Quaternion.identity, LayerMask.GetMask("map"), QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.CompareTag("Door") && array[i].TryGetComponent<SpecialPersonOutHeight>(out var component))
			{
				num = component.Height;
			}
		}
		v3.y += num;
		LayerMask layerMask = -1;
		float maxDistance = 10f;
		if (!Physics.Raycast(v3 + V.GlobalOffset, direction, out var hitInfo, maxDistance, layerMask.value, QueryTriggerInteraction.Ignore))
		{
			return car.transform.position.y;
		}
		v3.y = v3.y - hitInfo.distance + 0.2f;
		return v3.y;
	}

	private void OutOtherCar(GameObject person, GameObject car)
	{
		if (!car.TryGetComponent<VehicleData>(out var component) || (component.VehicleMembersController.PlacesCount >= 1 && (bool)component.Driver))
		{
			if (!person.TryGetComponent<Rigidbody>(out var component2))
			{
				component2 = person.AddComponent<Rigidbody>();
			}
			component2.interpolation = RigidbodyInterpolation.None;
			component2.constraints = RigidbodyConstraints.FreezeRotation;
			component2.isKinematic = false;
			car.TryGetComponent<Powertrain>(out var component3);
			person.TryGetComponent<MultiPersonControl>(out var component4);
			person.TryGetComponent<DriverActionController>(out var component5);
			person.TryGetComponent<MainPerson>(out var component6);
			Vector3 localPosition = person.transform.localPosition;
			Vector3 vector = car.transform.right * UnityEngine.Random.Range(1.3f, 1.5f);
			int placeOnCar = component.VehicleMembersController.GetPlaceOnCar(person);
			localPosition = (component.VehicleMembersController.IsPassenger(person) ? (component.RightHanded ? (localPosition - vector) : (localPosition + vector)) : (component.RightHanded ? (localPosition + vector) : (localPosition - vector)));
			float num = PersonOutHeight(localPosition, car);
			if (component.VehicleMembersController.PlacesCount > placeOnCar)
			{
				component.VehicleMembersController.SetMember(placeOnCar, null);
			}
			component3?.RestoreRigidbodyPhysics();
			if (component.isTruck)
			{
				localPosition.y += 2f;
			}
			person.transform.localPosition = localPosition;
			localPosition = person.transform.position;
			person.transform.position = new Vector3(localPosition.x, num + 0.1f, localPosition.z);
			person.transform.parent = (FlagsStorage.IsFreeDriveMode ? V.CoreGameManager.globalPoint.transform : null);
			person.transform.eulerAngles = Vector3.zero;
			FullBodyBipedIK ik = component6.ik;
			if ((bool)component6.mainBodySkin)
			{
				component6.mainBodySkin.shadowCastingMode = ShadowCastingMode.On;
			}
			if ((bool)component6.CombinedBodySkin)
			{
				component6.CombinedBodySkin.shadowCastingMode = ShadowCastingMode.On;
			}
			int num2 = 7;
			if (component6.allKits.Length > num2 && (bool)component6.allKits[num2])
			{
				component6.allKits[num2].gameObject.SetActive(value: true);
			}
			component4.currentCar = null;
			component4.isPassanger = false;
			if ((bool)ik)
			{
				ik.solver.IKPositionWeight = 0f;
			}
			_pcc.target = person.transform;
			component5.isPassenger = false;
			component5.EnableDisableAllColliders(WillEnable: true);
			component3.AngularVelocity = Vector3.zero;
			component3.Velocity = Vector3.zero;
			playCarCloseSound(car);
			person.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public async UniTask SetPersonKinematicWithIgnoreTrigger(Rigidbody personRb, bool isKinematic)
	{
		FlagsStorage.IgnoreTriggersAfterExit = true;
		await UniTask.Yield();
		if (!V.DriverPerson || _isDestroyed)
		{
			FlagsStorage.IgnoreTriggersAfterExit = false;
			return;
		}
		if ((bool)personRb)
		{
			personRb.isKinematic = isKinematic;
		}
		await UniTask.Yield();
		if ((bool)V.DriverPerson)
		{
			_ = _isDestroyed;
		}
		FlagsStorage.IgnoreTriggersAfterExit = false;
	}

	public void OutCar(GameObject person, GameObject car, bool enableCollidersImmediately = false, bool isChangePos = true, bool isSendRpc = true)
	{
		if ((bool)car)
		{
			car.GetComponent<Rigidbody>();
		}
		if (_timeAttackController != null && _timeAttackController.IsRaceInProgress())
		{
			_timeAttackController.StopRaceImmediately(FailReason.DISQUALIFIED);
		}
		FlagsStorage.IsDoorOpenAnimation = false;
		if (!car || !person)
		{
			return;
		}
		bool num = person == V.DriverPerson;
		QuickCarSettings quickCarSettings = FreeDriveProvider.QuickCarSettings;
		if (num && (bool)LevelsProvider.LevelScript)
		{
			if ((bool)quickCarSettings && car.TryGetComponent<LightController>(out var component))
			{
				quickCarSettings.oldHState = component.HState;
			}
			LevelsProvider.LevelScript.CleanForPersonView(isEnableTouch: false);
		}
		if (num && (bool)V.CoreGameManager.MenuLevelParent)
		{
			if ((bool)V.CoreGameManager.MenuLevelParent.LeftRight)
			{
				V.CoreGameManager.MenuLevelParent.LeftRight.SetActive(value: false);
			}
			if ((bool)_carInputManager.UGUISteeringWheel)
			{
				_carInputManager.UGUISteeringWheel.SetActive(active: false);
			}
		}
		if (person != V.DriverPerson)
		{
			OutOtherCar(person, car);
			return;
		}
		MainPerson component2 = person.GetComponent<MainPerson>();
		DriverActionController component3 = person.GetComponent<DriverActionController>();
		if (component3.IsExitCarBlocked)
		{
			return;
		}
		if (!FlagsStorage.InGarageMenu)
		{
			FreeDriveProvider.QuickCarSettings.ChangeCarLightMask(car, 1);
		}
		else
		{
			ControlItemsProvider.ShifterControl?.HideObjects();
		}
		if (!person.GetComponent<Rigidbody>())
		{
			person.AddComponent<Rigidbody>();
		}
		Rigidbody component4 = person.GetComponent<Rigidbody>();
		component4.interpolation = RigidbodyInterpolation.Interpolate;
		Rigidbody component5 = car.GetComponent<Rigidbody>();
		MultiPersonControl component6 = person.GetComponent<MultiPersonControl>();
		VehicleData component7 = car.GetComponent<VehicleData>();
		RagdollCPM ragdollCpm = person.GetComponent<DriverActionController>().ragdollCpm;
		component6.LastReceivedRawPosition = Vector3Int.RoundToInt(car.transform.localPosition);
		component6.LastSentRawPosition = component6.LastReceivedRawPosition;
		if (car.TryGetComponent<MultiPlayerPosControl>(out var component8))
		{
			component8.LastReceivedRawPosition = component6.LastReceivedRawPosition;
			component8.LastSentRawPosition = component8.LastReceivedRawPosition;
			component8.PredictedSentRawPosition = component8.LastReceivedRawPosition;
			component8.PredictedRawPosition = component8.LastReceivedRawPosition;
		}
		if ((bool)FreeDriveProvider.MultiPlayer)
		{
			FreeDriveProvider.MultiPlayer.RaiseRawPositionToAll(false, forceSendValue: true);
			FreeDriveProvider.MultiPlayer.RaiseRawPositionToAll(true, forceSendValue: true);
		}
		if (!ragdollCpm || !ragdollCpm.isRagdoll)
		{
			component4.constraints = RigidbodyConstraints.FreezeRotation;
			SetPersonKinematicWithIgnoreTrigger(component4, isKinematic: false).Forget();
		}
		FlagsStorage.OutOfCar = true;
		if ((bool)V.CoreGameManager && V.CoreGameManager.MapIconsModel != null)
		{
			V.CoreGameManager.MapIconsModel.GotOutOfCar(gotOut: true);
		}
		FlagsStorage.NowPolice = false;
		Vector3 localPosition = person.transform.localPosition;
		Vector3 vector = car.transform.right * UnityEngine.Random.Range(1.3f, 1.5f);
		if (!FlagsStorage.IsPassenger)
		{
			if (component7.RightHanded)
			{
				localPosition += vector;
			}
			else
			{
				localPosition -= vector;
			}
		}
		else if (component7.RightHanded)
		{
			localPosition -= vector;
		}
		else
		{
			localPosition += vector;
		}
		float num2 = PersonOutHeight(localPosition, car);
		component7.VehicleMembersController.SetMember(component6.placeOnCar, null);
		car.GetComponent<Powertrain>()?.RestoreRigidbodyPhysics();
		FlagsStorage.IsPassenger = false;
		component6.currentCar = null;
		component6.isPassanger = false;
		FlagsStorage.KeepRow = false;
		if (component7.isTruck)
		{
			localPosition.y += 2f;
		}
		if (isChangePos)
		{
			person.transform.localPosition = localPosition;
		}
		localPosition = person.transform.position;
		if ((bool)V.CoreGameManager.globalPoint && car.transform.parent == V.CoreGameManager.globalPoint.transform && Mathf.Approximately(V.GlobalOffset.y, 100f))
		{
			num2 += 100f;
		}
		if (isChangePos)
		{
			person.transform.position = new Vector3(localPosition.x, num2 + 0.1f, localPosition.z);
		}
		person.transform.parent = (FlagsStorage.IsFreeDriveMode ? V.CoreGameManager.globalPoint.transform : null);
		if (isChangePos)
		{
			person.transform.eulerAngles = Vector3.zero;
		}
		if ((bool)ControlItemsProvider.InputTouch)
		{
			ControlItemsProvider.InputTouch.enabled = true;
		}
		if ((bool)component2.mainBodySkin)
		{
			component2.mainBodySkin.shadowCastingMode = ShadowCastingMode.On;
		}
		if ((bool)component2.CombinedBodySkin)
		{
			component2.CombinedBodySkin.shadowCastingMode = ShadowCastingMode.On;
		}
		if ((bool)component2.allKits[7])
		{
			component2.allKits[7].gameObject.SetActive(value: true);
		}
		if ((bool)component2.allKits[3] && PersonElements.CapNeedHideInCar(component2.Gender, component2.allKits[3].GetComponent<Equipment>().id))
		{
			component2.allKits[3].gameObject.SetActive(value: true);
			if (component2.HairHiddenByHat && (bool)component2.allKits[0])
			{
				component2.allKits[0].SetActive(value: false);
			}
			if ((bool)component2.allKits[2] && component2.allKits[3].GetComponent<Equipment>().HideBeard)
			{
				component2.allKits[2].SetActive(value: false);
			}
		}
		if ((int)component2.Gender == 1 && (bool)component2.allKits[0])
		{
			Equipment component9 = component2.allKits[0].GetComponent<Equipment>();
			int id = component9.id;
			if (component2.HairsToIgnore.Contains(id))
			{
				SkinnedMeshRenderer component10 = component2.allKits[0].GetComponent<SkinnedMeshRenderer>();
				component10.sharedMesh = component9.originalMesh;
				component10.materials = component9.originalMats;
			}
		}
		_pcc.target = person.transform;
		person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: true, enableCollidersImmediately);
		if (FlagsStorage.IsMultiFree && !FlagsStorage.InGarageFree)
		{
			person.GetComponent<MultiPersonControl>().SendCarExit(person, car, isSendRpc);
			if (!FlagsStorage.IsPassenger)
			{
				car.GetComponent<MultiPlayerPosControl>().updateLabel(person, Show: false);
			}
			SetAllWheelOwn(car, Own: true);
		}
		bool ignoreBrakes = false;
		if ((bool)LevelsProvider.LevelScript)
		{
			LevelsProvider.LevelScript.Resume(ignoreBrakes);
		}
		if ((bool)V.CoreGameManager.MenuLevelParent.InsideCamControllParent)
		{
			V.CoreGameManager.MenuLevelParent.InsideCamControllParent.SetActive(value: false);
		}
		V.LastUsedCar = car;
		if (!FlagsStorage.InGarageMenu)
		{
			FreeDriveProvider.QuickCarSettings.showDoor = false;
			FreeDriveProvider.QuickCarSettings.ShowDoor();
			LockCarButtonController.HideKickPassengersButton();
		}
		component3.isPassenger = false;
		if (!component5.isKinematic)
		{
			component5.velocity = Vector3.zero;
			component5.angularVelocity = Vector3.zero;
		}
		if ((bool)V.Car && V.Car.TryGetComponent<VehicleEntityProvider>(out var component11))
		{
			component11.TrySaveMileage(useFullSave: false);
		}
		V.Car = null;
		if ((bool)ControlItemsProvider.ThrottleSliderControl)
		{
			ControlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
		}
		if ((bool)ControlItemsProvider.InputTouch)
		{
			InputTouch inputTouch = ControlItemsProvider.InputTouch;
			if ((bool)inputTouch.transform.parent)
			{
				inputTouch.transform.parent.gameObject.SetActive(value: true);
			}
			if ((bool)inputTouch.leftJoy)
			{
				inputTouch.leftJoy.SetActive(value: true);
			}
			if ((bool)inputTouch.rightJoy)
			{
				inputTouch.rightJoy.SetActive(value: true);
			}
			if ((bool)inputTouch.jumpButton)
			{
				inputTouch.jumpButton.SetActive(active: true);
			}
			if ((bool)inputTouch.TouchField)
			{
				inputTouch.TouchField.gameObject.SetActive(value: true);
			}
		}
		if ((bool)V.CoreGameManager.MenuLevelParent.ParkingMenuUI)
		{
			V.CoreGameManager.MenuLevelParent.ParkingMenuUI.ShowWalkElements(canShow: true);
		}
		if ((bool)ControlItemsProvider.TouchField)
		{
			ControlItemsProvider.TouchField.SetActive(active: true);
		}
		V.CoreGameManager.SetSteering();
		if ((bool)FreeDriveProvider.CarCameras)
		{
			FreeDriveProvider.CarCameras.NewCarEntered(car);
		}
		if (isChangePos)
		{
			playCarCloseSound(car);
		}
		person.GetComponent<PhotonView>().Synchronization = ViewSynchronization.UnreliableOnChange;
		V.PersonCamIndex = PlayerPrefs.GetInt("PersonCamIndex", 0);
		if (TryGetComponent<CarCamerasController>(out var component12))
		{
			component12.DoNotUpdatePersonCam = true;
			component12.SetCamera(V.PersonCamIndex, V.DriverPerson.transform, newTarget: true);
		}
		person.transform.localScale = new Vector3(1f, 1f, 1f);
		V.RaiseCarExitEvent(person, component7);
		component7.CarMirrorController.SetMirror(val: false);
		if (V.DriverPerson == person)
		{
			if (!_playerComponent)
			{
				_playerComponent = person.GetComponent<PlayerComponent>();
			}
			else
			{
				_playerComponent.enabled = true;
			}
		}
		LockCarButtonController.SetActiveRoot(isActive: true);
		component2.SwitchHeadVisible(visible: true);
		if (CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low)
		{
			component7.InteriorsComposition.ShowLowInterior();
		}
	}

	public void ShowBusyMessage(int indexMessage)
	{
		string text = new LocalizedString("CarOutInControl.CarIsBusy");
		string mes = "CarOutInControl.CarIsBusy";
		switch (indexMessage)
		{
		case 1:
			text = new LocalizedString("CarOutInControl.CarIsLocked");
			mes = "CarOutInControl.CarIsLocked";
			break;
		case 2:
			text = new LocalizedString("CarOutInControl.ItIsJustTrafficCar");
			mes = "CarOutInControl.ItIsJustTrafficCar";
			break;
		}
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			if ((bool)V.CoreGameManager)
			{
				V.CoreGameManager.SetMessage(text, 3f);
			}
		}
		else if ((bool)LevelsProvider.LevelScript)
		{
			LevelsProvider.LevelScript.ShowMessage(mes, isImportant: true);
		}
		Debug.Log("Message:" + text);
	}

	private void InCarOtherCar(GameObject person, GameObject car)
	{
		if (car.TryGetComponent<DriverController>(out var component))
		{
			component.DriverPerson = person;
		}
		if (person.TryGetComponent<DriverActionController>(out var component2))
		{
			component2.EnableDisableAllColliders(WillEnable: false);
		}
		component.presetAfterRacingPerson(this.GetCancellationTokenOnDestroy()).Forget();
	}

	public async UniTask InCar(GameObject person, GameObject car, bool isSendRpc = true, bool playDoorSound = true, bool whenBack = false)
	{
		MonoBehaviour.print($"In car person: {person} car:{car}");
		FlagsStorage.IsDoorOpenAnimation = false;
		if (!person || !car)
		{
			return;
		}
		if (person.TryGetComponentInChildren<PersonAnimationsController>(out var component))
		{
			component.SitGetOutCarSystem.SitGetOutData.SetCarSitData(car, 0, isSitting: false);
		}
		RagdollCPM ragdollCpm = person.GetComponent<DriverActionController>().ragdollCpm;
		if ((bool)ragdollCpm && ragdollCpm.isRagdoll)
		{
			ragdollCpm.DisableRagdoll(immediately: true);
		}
		if (person != V.DriverPerson)
		{
			InCarOtherCar(person, car);
			return;
		}
		CheckCarForEnter(car, out var toReturn);
		if (!toReturn)
		{
			return;
		}
		VehicleData carData = car.GetComponent<VehicleData>();
		InputStorage.LeftJoyInputX = 0f;
		InputStorage.LeftJoyInputY = 0f;
		if (!FlagsStorage.InGarageMenu)
		{
			if (((bool)TimeProvider.skyboxBlender && TimeProvider.skyboxBlender.dayTime == SkyboxBlender.DayTime.Night) || TimeProvider.skyboxSelectedRuntime == 3)
			{
				FreeDriveProvider.QuickCarSettings.ChangeCarLightMask(car, 0);
			}
			else if ((bool)V.LastUsedCar)
			{
				FreeDriveProvider.QuickCarSettings.ChangeCarLightMask(V.LastUsedCar, 1);
			}
		}
		if (person.TryGetComponent<WorldInteractionController>(out var component2))
		{
			component2.StopForCarEnter();
		}
		FlagsStorage.OutOfCar = false;
		if (person.TryGetComponent<PersonLocomotion>(out var component3))
		{
			component3.ResetToRest();
		}
		ResetKeepLane();
		DriverController component4 = car.GetComponent<DriverController>();
		MultiPersonControl component5 = person.GetComponent<MultiPersonControl>();
		Powertrain component6 = car.GetComponent<Powertrain>();
		if (!car.GetComponent<Dragy>())
		{
			car.AddComponent<Dragy>();
		}
		component6.enabled = true;
		component5.placeOnCar = 0;
		person.GetComponent<DriverActionController>().isPassenger = false;
		carData.VehicleMembersController.SetDriver(person);
		component6.RestoreRigidbodyPhysics();
		FullBodyBipedIK ik = person.GetComponent<MainPerson>().ik;
		if ((bool)ik)
		{
			ik.solver.IKPositionWeight = 1f;
		}
		person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: false);
		component5.currentCar = car;
		ChangeCar(car);
		carData.InteriorsComposition.TryRestoreSavedInterior();
		if (car.TryGetComponent<MultiPlayerPosControl>(out var component7))
		{
			component7.LastReceivedRawPosition = Vector3Int.RoundToInt(car.transform.localPosition);
			component7.LastSentRawPosition = component7.LastReceivedRawPosition;
			component7.PredictedSentRawPosition = component7.LastReceivedRawPosition;
			component7.PredictedRawPosition = component7.LastReceivedRawPosition;
		}
		if ((bool)FreeDriveProvider.MultiPlayer)
		{
			FreeDriveProvider.MultiPlayer.RaiseRawPositionToAll(true, forceSendValue: true);
		}
		if ((bool)V.CoreGameManager && V.CoreGameManager.MapIconsModel != null)
		{
			V.CoreGameManager.MapIconsModel.GotOutOfCar(gotOut: false);
		}
		InCarMenuSet(car);
		ResetNewCarEntered(car);
		component4.DriverPerson = person;
		component4.PreSetPerson(person, 0);
		if ((bool)V.CoreGameManager.carCamerasController && !FlagsStorage.InGarageMenu)
		{
			if (V.CoreGameManager.carCamerasController.i == 0)
			{
				V.CurrentCamIndex = 0;
			}
			switch (V.CurrentCamIndex)
			{
			case 0:
				V.CoreGameManager.carCamerasController.SetCamera(0, V.Car.GetComponent<DriverController>().camInsideView.transform, newTarget: true);
				FreeDriveProvider.CarCameras.Target = V.Car.GetComponent<DriverController>().camInsideView.transform;
				V.CoreGameManager.MenuLevelParent.InsideCamControllParent.SetActive(value: true);
				break;
			case 5:
				if ((bool)V.CoreGameManager.MenuLevelParent)
				{
					V.CoreGameManager.MenuLevelParent.InsideCamControllParent.SetActive(value: true);
				}
				break;
			default:
				if ((bool)V.CoreGameManager.MenuLevelParent.InsideCamControllParent)
				{
					V.CoreGameManager.MenuLevelParent.InsideCamControllParent.SetActive(value: false);
				}
				break;
			}
		}
		if (FlagsStorage.IsMultiFree)
		{
			car.GetComponent<VehicleData>().KeepRowComposition.CurrentTargetIndex = V.CurrentIndexArrayPositionCar;
			person.GetComponent<MultiPersonControl>().SendCarEnter(person, car, isSendRpc);
			bool hideLabel = false;
			if ((bool)FreeDriveProvider.FirstCarMultiPlayer)
			{
				hideLabel = car == FreeDriveProvider.FirstCarMultiPlayer;
			}
			if (car.TryGetComponent<MultiPlayerPosControl>(out var component8))
			{
				component8.updateLabel(person, Show: false, hideLabel);
			}
			_resetIsKinematicDisposable?.Dispose();
			_resetIsKinematicDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
			{
				resetIsKinematic();
			}).AddTo(this);
			SetAllWheelOwn(car, Own: true);
			if (!carData.MainModel)
			{
				await carData.BodyLoadingComposition.LoadFromResource(immediately: true);
				if (car.TryGetComponent<BodyKits>(out var component9))
				{
					component9.LoadAllKits();
				}
			}
			carData.BodyLoadingComposition.ShowMainModelOnly();
			car.GetComponent<SoundController>().enabled = true;
		}
		if (FlagsStorage.ShowCarMirror)
		{
			carData.CarMirrorController.SetMirror(val: true);
		}
		CloseAllDoors(carData);
		if (LevelsProvider.CurrentLevelData != null && (bool)LevelsProvider.LevelScript && LevelsProvider.CurrentLevelData.ID == 56)
		{
			LevelsProvider.LevelScript.ShowMessage("CarOutInControl.DriveToPoint", isImportant: true);
			LevelsProvider.LevelScript.levelDatas[LevelsProvider.CurrentLevelData.IndexInScene].ObjectsForEnable[1].gameObject.SetActive(value: false);
		}
		FlagsStorage.NowPolice = carData.LightsComposition.IsPolice;
		if (!FlagsStorage.InGarageMenu & playDoorSound)
		{
			playCarCloseSound(car);
		}
		if ((bool)FreeDriveProvider.CarCameras)
		{
			FreeDriveProvider.CarCameras.NewCarEntered(car);
		}
		person.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
		if (FlagsStorage.IsFreeDriveMode)
		{
			TrailerController trailerController = Global.TaskManager.DeliveryManager.GetTrailerController(PhotonNetwork.LocalPlayer.ActorNumber);
			if ((bool)trailerController && car != trailerController.Car)
			{
				trailerController.Owner.GetComponent<DeliveryController>().ShowAndHideDestination(trailerController, Global.TaskManager.Task.Order.GetDestination());
			}
		}
		if (car == FreeDriveProvider.FirstCarMultiPlayer)
		{
			LockCarButtonController.ShowKickPassengersButton(carData);
		}
		if (V.DriverPerson == person)
		{
			if (!_playerComponent)
			{
				_playerComponent = person.GetComponent<PlayerComponent>();
			}
			if ((bool)_playerComponent)
			{
				_playerComponent.enabled = false;
			}
		}
		V.RaiseCarEnterEvent(person, carData);
		V.CoreGameManager.SetSteering();
		if ((bool)V.CoreGameManager.MenuLevelParent.ParkingMenuUI)
		{
			V.CoreGameManager.MenuLevelParent.ParkingMenuUI.SwitchTurnSignal();
		}
	}

	private void ResetKeepLane()
	{
		if (FreeDriveProvider != null && (bool)FreeDriveProvider.QuickCarSettings)
		{
			FreeDriveProvider.QuickCarSettings.DisableKeepLane(message: false);
		}
		else
		{
			FlagsStorage.KeepRow = false;
		}
	}

	private void CloseAllDoors(VehicleData carData)
	{
		if (carData.TryGetComponent<CarAnimationsController>(out var component))
		{
			component.CloseMainOpenedDoors();
		}
	}

	private void resetIsKinematic()
	{
		if ((bool)V.Car)
		{
			V.Car.GetComponent<Rigidbody>().isKinematic = true;
			SetAllWheelOwn(V.Car, Own: true);
			_isKinematicDisposable?.Dispose();
			_isKinematicDisposable = Observable.Timer(TimeSpan.FromSeconds(0.30000001192092896)).Subscribe((long _) =>
			{
				IsKinematic();
			}).AddTo(this);
		}
	}

	private void IsKinematic()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<Rigidbody>(out var component))
		{
			component.isKinematic = false;
			component.constraints = RigidbodyConstraints.None;
			component.freezeRotation = false;
			if (V.Car.TryGetComponent<Powertrain>(out var component2))
			{
				component2.RestoreRigidbodyPhysics();
			}
			else
			{
				component.ResetInertiaTensor();
			}
		}
	}

	public void InCarPassenger(GameObject person, GameObject car, int placeToEnter = -1, bool isSendRpc = true, bool playDoorSound = true)
	{
		if (!person || !car)
		{
			return;
		}
		CheckCarForEnterPassenger(car, out var toReturn);
		if (!toReturn)
		{
			return;
		}
		if (placeToEnter == -1)
		{
			placeToEnter = GetPlaceForPassenger(car);
		}
		if (placeToEnter == -1)
		{
			ShowBusyMessage(0);
			return;
		}
		FlagsStorage.IsPassenger = true;
		if (person.TryGetComponent<WorldInteractionController>(out var component))
		{
			component.StopForCarEnter();
		}
		FlagsStorage.OutOfCar = false;
		if (person.TryGetComponent<PersonLocomotion>(out var component2))
		{
			component2.ResetToRest();
		}
		ResetKeepLane();
		FlagsStorage.NowPolice = false;
		VehicleData component3 = car.GetComponent<VehicleData>();
		DriverController component4 = car.GetComponent<DriverController>();
		MultiPersonControl component5 = person.GetComponent<MultiPersonControl>();
		DriverActionController component6 = person.GetComponent<DriverActionController>();
		component5.placeOnCar = placeToEnter;
		component3.VehicleMembersController.SetMember(placeToEnter, person);
		car.GetComponent<Powertrain>()?.RestoreRigidbodyPhysics();
		FullBodyBipedIK ik = person.GetComponent<MainPerson>().ik;
		if ((bool)ik)
		{
			ik.solver.IKPositionWeight = 1f;
		}
		person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: false);
		FreeDriveProvider.QuickCarSettings.showDoor = false;
		FreeDriveProvider.QuickCarSettings.ShowDoor();
		if (car == FreeDriveProvider.FirstCarMultiPlayer)
		{
			LockCarButtonController.ShowKickPassengersButton(component3);
		}
		component5.currentCar = car;
		component5.isPassanger = true;
		component6.isPassenger = true;
		DisableInputParktronikMakeOnline(V.LastUsedCar);
		V.Car = car;
		component3.InteriorsComposition.TryRestoreSavedInterior();
		FreeDriveProvider.CarCameras.Target = V.Car.transform;
		InCarMenuPassangerSet();
		ResetNewCarEntered(car);
		component4.DriverPerson = person;
		component4.PreSetPerson(person, placeToEnter);
		V.CoreGameManager.carCamerasController.SetCachedCar(person, component3);
		if (V.CurrentCamIndex == 0)
		{
			V.CoreGameManager.carCamerasController.SetCamera(0, V.Car.GetComponent<DriverController>().camInsideView.transform, newTarget: true);
			FreeDriveProvider.CarCameras.Target = V.Car.GetComponent<DriverController>().camInsideView.transform;
			V.CoreGameManager.MenuLevelParent.InsideCamControllParent.SetActive(value: true);
		}
		else if (V.CurrentCamIndex == 5)
		{
			V.CoreGameManager.MenuLevelParent.InsideCamControllParent.SetActive(value: true);
		}
		else
		{
			V.CoreGameManager.MenuLevelParent.InsideCamControllParent.SetActive(value: false);
			if (FlagsStorage.FixedCameraAndLookAt)
			{
				V.CurrentCamIndex = 2;
				V.CoreGameManager.carCamerasController.SetCamera(2, V.Car.transform, newTarget: true);
			}
		}
		if (FlagsStorage.IsMultiFree)
		{
			person.GetComponent<MultiPersonControl>().SendCarEnterPassanger(person, car, placeToEnter, isSendRpc);
			if (car.TryGetComponent<MultiPlayerPosControl>(out var component7))
			{
				component7.updateLabel(person, Show: false);
			}
			SetAllWheelOwn(car, Own: true);
		}
		if (LevelsProvider.CurrentLevelData != null && LevelsProvider.CurrentLevelData.ID == 56)
		{
			LevelsProvider.LevelScript.ShowMessage("CarOutInControl.SitAsDriver", isImportant: true);
		}
		if (playDoorSound)
		{
			playCarCloseSound(car);
		}
	}

	public int GetPlaceForPassenger(GameObject car)
	{
		VehicleData component = car.GetComponent<VehicleData>();
		if (component.VehicleMembersController.PlacesCount == 2)
		{
			return 1;
		}
		return component.VehicleMembersController.FirstFreePlaceForPassenger;
	}

	public void SetAllWheelOwn(GameObject car, bool Own)
	{
		foreach (WheelProvider allWheel in car.GetComponent<Axles>().AllWheels)
		{
			if (Own)
			{
				allWheel.Wheel.Enabled = true;
			}
		}
	}

	private void HideWalkButtons()
	{
		if ((bool)ControlItemsProvider.InputTouch)
		{
			if ((bool)ControlItemsProvider.InputTouch.leftJoy)
			{
				ControlItemsProvider.InputTouch.leftJoy.SetActive(value: false);
			}
			if ((bool)ControlItemsProvider.InputTouch.rightJoy)
			{
				ControlItemsProvider.InputTouch.rightJoy.SetActive(value: false);
			}
			if ((bool)ControlItemsProvider.InputTouch.jumpButton)
			{
				ControlItemsProvider.InputTouch.jumpButton.SetActive(active: false);
			}
		}
		if ((bool)ControlItemsProvider.TouchField)
		{
			ControlItemsProvider.TouchField.SetActive(active: false);
		}
		if ((bool)V.CoreGameManager.MenuLevelParent.ParkingMenuUI)
		{
			V.CoreGameManager.MenuLevelParent.ParkingMenuUI.ShowWalkElements(canShow: false);
		}
	}

	public void InCarMenuSet(GameObject car)
	{
		HideWalkButtons();
		if ((bool)FreeDriveProvider.QuickCarSettings)
		{
			if ((bool)LevelsProvider.LevelScript)
			{
				LevelsProvider.LevelScript.Resume();
			}
			FreeDriveProvider.QuickCarSettings.showDoor = false;
			FreeDriveProvider.QuickCarSettings.ShowDoor();
		}
		if ((bool)V.CoreGameManager.MenuLevelParent.IndicatorsParent && !FlagsStorage.IsPassenger)
		{
			V.CoreGameManager.MenuLevelParent.IndicatorsParent.SetActive(value: true);
		}
		if ((bool)V.CoreGameManager.MenuLevelParent.ParkingMenuUI)
		{
			V.CoreGameManager.MenuLevelParent.ParkingMenuUI.SwitchTurnSignal();
		}
		OnCarMenuSet?.Invoke(obj: true);
	}

	public void InCarMenuPassangerSet()
	{
		HideWalkButtons();
		LevelsProvider.LevelScript.Resume(ignoreBrakes: true);
		ControlItemsProvider.ShifterControl.HideObjects();
		OnCarMenuSet?.Invoke(obj: false);
	}

	private void CheckCarForEnter(GameObject car, out bool toReturn)
	{
		toReturn = true;
		if (car.TryGetComponent<VehicleData>(out var component))
		{
			bool flag = component.CarLocked && car == FreeDriveProvider.FirstCarMultiPlayer;
			if ((component.CarLocked & flag) && !component.Driver)
			{
				toReturn = true;
			}
			else if (component.isTraffiCar || (bool)component.Driver || component.CarLocked)
			{
				toReturn = false;
				if (component.CarLocked)
				{
					ShowBusyMessage(1);
					_ = (bool)FreeDriveProvider.FreeDriveControl;
				}
				else if (component.isTraffiCar)
				{
					ShowBusyMessage(2);
				}
				else
				{
					ShowBusyMessage(0);
				}
			}
		}
		else
		{
			toReturn = false;
			ShowBusyMessage(1);
		}
	}

	private void CheckCarForEnterPassenger(GameObject car, out bool toReturn)
	{
		toReturn = true;
		if (car.TryGetComponent<VehicleData>(out var component))
		{
			if (component.isTraffiCar || !component.VehicleMembersController.HasPlaceForPassenger || (component.CarLocked && component.gameObject != FreeDriveProvider.FirstCarMultiPlayer))
			{
				toReturn = false;
				if (component.CarLocked)
				{
					ShowBusyMessage(1);
					_ = (bool)FreeDriveProvider.FreeDriveControl;
				}
				else if (component.isTraffiCar)
				{
					ShowBusyMessage(2);
				}
				else
				{
					ShowBusyMessage(0);
				}
			}
		}
		else
		{
			toReturn = false;
			ShowBusyMessage(1);
		}
	}

	private void CheckCarNear()
	{
		if (!V.DriverPerson)
		{
			return;
		}
		if (FlagsStorage.OutOfCar)
		{
			if ((bool)V.DriverPerson)
			{
				Vector3 direction = V.DriverPerson.transform.TransformDirection(new Vector3(0f, -0.33f, 1f));
				Vector3 direction2 = V.DriverPerson.transform.TransformDirection(new Vector3(0f, 0.33f, 1f));
				Vector3 position = V.DriverPerson.transform.position;
				position.y += 0.7f;
				LayerMask layerMask = -1;
				bool flag = Physics.Raycast(position, direction, out var hitInfo, 3f, layerMask.value, QueryTriggerInteraction.Ignore);
				bool flag2 = Physics.Raycast(position, direction2, out var hitInfo2, 3f, layerMask.value, QueryTriggerInteraction.Ignore);
				if (flag | flag2)
				{
					if (flag)
					{
						ProcessRay(hitInfo);
					}
					if (flag2)
					{
						ProcessRay(hitInfo2);
					}
				}
			}
			else
			{
				CarToEnter = null;
			}
			if (FreeDriveProvider != null && (bool)FreeDriveProvider.QuickCarSettings)
			{
				if ((bool)CarToEnter)
				{
					FreeDriveProvider.QuickCarSettings.ShowDoor();
				}
				else
				{
					FreeDriveProvider.QuickCarSettings.HideDoor();
				}
			}
			if (FlagsStorage.InGarageMenu)
			{
				return;
			}
			if (!CarToEnter && (bool)LockCarButtonController)
			{
				LockCarButtonController.HideKickPassengersButton();
			}
			if (FreeDriveProvider != null && (bool)FreeDriveProvider.FirstCarMultiPlayer && (bool)CarToEnter && CarToEnter == FreeDriveProvider.FirstCarMultiPlayer)
			{
				if (!FreeDriveProvider.FirstCarMultiPlayer.TryGetComponent<VehicleData>(out var component))
				{
					return;
				}
				component.IsTraded.Get(out var value);
				if ((bool)LockCarButtonController)
				{
					if (value)
					{
						LockCarButtonController.HideKickPassengersButton();
					}
					else
					{
						LockCarButtonController.ShowKickPassengersButton(component);
					}
				}
			}
			else if ((bool)LockCarButtonController)
			{
				LockCarButtonController.HideKickPassengersButton();
			}
		}
		else
		{
			if (FlagsStorage.IsInGarageTunnel || !_carInputManager.GetPowertrain)
			{
				return;
			}
			if ((bool)LockCarButtonController)
			{
				LockCarButtonController.HideKickPassengersButton();
			}
			bool num = _carInputManager.GetPowertrain.SpeedKmh > 10f;
			bool flag3 = FlagsStorage.RacingGameMode && !FlagsStorage.IsLevelScene;
			bool chasingRaceModeStarted = FlagsStorage.ChasingRaceModeStarted;
			bool flag4 = LevelsProvider != null && (bool)LevelsProvider.CurrentLevelData && LevelsProvider.CurrentLevelData.Type == LevelType.DragRace;
			bool flag5 = LevelsProvider != null && (bool)LevelsProvider.CurrentLevelData && LevelsProvider.CurrentLevelData.Type == LevelType.Chasing;
			if (num | flag3 | chasingRaceModeStarted | flag4 | flag5)
			{
				if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.QuickCarSettings)
				{
					V.CoreGameManager.FreeDriveProvider.QuickCarSettings.HideDoor();
				}
				if ((bool)LockCarButtonController)
				{
					LockCarButtonController.HideKickPassengersButton();
				}
			}
			else
			{
				if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.QuickCarSettings)
				{
					V.CoreGameManager.FreeDriveProvider.QuickCarSettings.ShowDoor();
				}
				if ((bool)V.Car && (bool)FreeDriveProvider.FirstCarMultiPlayer && V.Car == FreeDriveProvider.FirstCarMultiPlayer && (bool)LockCarButtonController)
				{
					LockCarButtonController.ShowKickPassengersButton(V.Car.GetComponent<VehicleData>());
				}
			}
		}
	}

	private void ProcessRay(RaycastHit hit)
	{
		if (hit.transform.CompareTag("Trailer") && !FlagsStorage.InGarageMenu)
		{
			FreeDriveProvider.QuickCarSettings.ShowResetTrailer(hit);
		}
		CarToEnter = null;
		if (hit.transform.CompareTag("Car") || hit.transform.CompareTag("OnlineCar") || hit.transform.CompareTag("CarSellBlockCollider"))
		{
			if (hit.transform.CompareTag("CarSellBlockCollider"))
			{
				if (hit.transform.TryGetComponent<CarRef>(out var component))
				{
					CarToEnter = component.Car;
				}
			}
			else
			{
				CarToEnter = hit.transform.gameObject;
			}
			if ((bool)CarToEnter && CarToEnter.GetComponent<Rigidbody>().velocity.magnitude > 5f)
			{
				CarToEnter = null;
			}
		}
		else
		{
			CarToEnter = null;
		}
	}

	public void ChangeCar(GameObject car)
	{
		GameObject lastUsedCar = V.LastUsedCar;
		V.Car = car;
		if ((bool)lastUsedCar && lastUsedCar != LevelsProvider.RacingCarLevel)
		{
			DisableInputParktronikMakeOnline(lastUsedCar);
		}
		if (car.TryGetComponent<VehicleData>(out var component))
		{
			if (component.isGarageCar)
			{
				_slotService.GetSlotCollectionBySlotId(component.slotId).LastSlotId = component.slotId;
				V.SlotID = component.slotId;
				V.CarId = component.CarId;
				AnalyticsHelper.SaveLastCarId(V.CarId);
			}
			V.LastUsedCar = (V.Car = component.gameObject);
			component.CarMirrorController.SetMirror(FlagsStorage.ShowCarMirror);
		}
		if (TryGetComponent<DashBoard>(out var component2))
		{
			component2.CarChanged = true;
		}
		if ((bool)FreeDriveProvider.CarCameras)
		{
			FreeDriveProvider.CarCameras.Target = V.Car.transform;
		}
	}

	private void DisableInputParktronikMakeOnline(GameObject oldCar)
	{
		if ((bool)oldCar && oldCar.TryGetComponent<VehicleData>(out var component) && component.ParkTronikComposition != null)
		{
			component.ParkTronikComposition.TrySetActiveParktronikParent(active: false);
		}
	}

	public void ChangeCarPassanger(GameObject car)
	{
		V.Car = car;
		FreeDriveProvider.CarCameras.Target = V.Car.transform;
	}

	private void ResetNewCarEntered(GameObject car)
	{
		if (car.TryGetComponent<Rigidbody>(out var component) && !component.isKinematic)
		{
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		if (!FlagsStorage.InGarageMenu)
		{
			FreeDriveProvider.QuickCarSettings.PreSetValues();
		}
	}

	public void playCarCloseSound(GameObject car)
	{
		if ((bool)car)
		{
			if (car.transform.position != PlayerDatas.CarLoadingUtility.ZeroSpawnPoint)
			{
				car.GetComponent<SoundController>().PlayDoorSound();
				return;
			}
			Debug.LogWarningFormat("Try Play door sound on zero position:{0}, pos: {1}", car, car.transform.position);
		}
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
		_checkCarNearCts?.Cancel();
		_checkCarNearCts?.Dispose();
		_checkCarNearCts = null;
		_resetIsKinematicDisposable?.Dispose();
		_isKinematicDisposable?.Dispose();
		ControlItemsProvider = null;
		FreeDriveProvider = null;
		LevelsProvider = null;
	}
}
