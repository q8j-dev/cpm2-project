using System;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using Service;
using Service.Taxi;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Other.ProceduralDoors;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;

public class CarTriggers : MonoBehaviour
{
	private const float BannerSendDelay = 1f;

	private readonly uint _carRepair = AudioWrapper.GetId("Car_Repair");

	private readonly uint _wheelRepair = AudioWrapper.GetId("Wheel_Repair");

	public LevelScript LevelScript;

	private int collidedObjectIndex;

	public int DamageCount;

	private bool _canReactionOnBanner = true;

	private bool _repairSoundTrigger;

	private bool _repairTurnedOff;

	private MultiPlayerPosControl _multiplayerPosControl;

	private VehicleData _vehicleData;

	private Rigidbody _rigidbody;

	private SoundController _soundController;

	private IDisposable _checkDisposable2;

	private IDisposable _checkDisposable1;

	private IDisposable _disposableOnCarDamaged;

	private IDisposable _disposableResetAnim;

	private IDisposable _disposableOnCarAccident;

	private IDisposable _disposableCarFinish;

	private IDisposable _disposableReleaseBrake;

	private IPricesService _pricesService;

	public FreeDriveProvider FreeDriveProvider { get; set; }

	public LevelsProvider LevelsProvider { get; set; }

	public RacingProvider RacingProvider { get; set; }

	public LevelsAnalytic LevelsAnalytic { get; set; }

	private void Awake()
	{
		TryGetComponent<MultiPlayerPosControl>(out _multiplayerPosControl);
		TryGetComponent<VehicleData>(out _vehicleData);
		TryGetComponent<Rigidbody>(out _rigidbody);
		TryGetComponent<SoundController>(out _soundController);
		_pricesService = _vehicleData.GetPricesService();
	}

	private void OnDestroy()
	{
		_disposableCarFinish?.Dispose();
		_disposableReleaseBrake?.Dispose();
	}

	private void SetSpeed(GameObject other)
	{
		SetCarSpeed component = other.GetComponent<SetCarSpeed>();
		if (!(component == null))
		{
			if (component.clampSpeed != 0f && base.gameObject.TryGetComponent<Powertrain>(out var component2) && component2.Velocity.magnitude > component.clampSpeed)
			{
				component2.Velocity = Vector3.ClampMagnitude(component2.Velocity, component.clampSpeed);
			}
			CarAIController component3 = base.gameObject.GetComponent<CarAIController>();
			if (_vehicleData.GetCarInputManager.TargetSpeed != 0f)
			{
				_vehicleData.GetCarInputManager.TargetSpeed = component.targetSpeed;
			}
			if ((bool)component3 && component3.TargetSpeed != 0f)
			{
				component3.TargetSpeed = component.targetSpeed + (float)UnityEngine.Random.Range(-3, 3);
			}
		}
	}

	private void OnTriggerEnterRaceMode(Collider other)
	{
		if ((bool)_vehicleData.GetCarInputManager && other.name == "stop" && !_vehicleData.GetCarInputManager.Ready && !FlagsStorage.RaceStarted && V.Car == base.gameObject)
		{
			_vehicleData.GetCarInputManager.State = 1;
			_vehicleData.GetCarInputManager.Ready = true;
			Powertrain component = V.Car.GetComponent<Powertrain>();
			component.Velocity = Vector3.zero;
			component.ResetTransmission();
			component.FixVehicle(stop: true);
			if ((bool)FreeDriveProvider.DragRacingControlMultiplayer)
			{
				FreeDriveProvider.DragRacingControlMultiplayer.onReachedStartLine();
			}
			if (FlagsStorage.IsReadyMultRaceCar2 && !FlagsStorage.IsFreeDriveMode && (bool)LevelsProvider.TreeControl)
			{
				LevelsProvider.TreeControl.OnBothCarOnStartLine();
			}
		}
		if (base.gameObject == V.Car && FlagsStorage.RaceStarted)
		{
			DragRacingControlMultiplayer dragRacingControlMultiplayer = FreeDriveProvider.DragRacingControlMultiplayer;
			DragRacingMultiplayerTree dragRacingMultiplayerTree = (((bool)dragRacingControlMultiplayer && (bool)dragRacingControlMultiplayer.newRacingMultiplayerController) ? dragRacingControlMultiplayer.newRacingMultiplayerController.CurrentRacingTree : null);
			if (((dragRacingMultiplayerTree != null) ? dragRacingMultiplayerTree.IsFinishCollider(other) : (other.name == "FinishRace")) && (bool)dragRacingControlMultiplayer)
			{
				dragRacingControlMultiplayer.onReachedFinishLine();
			}
		}
		switch (other.name)
		{
		case "toGarage":
			RacingProvider.DragRacingController.ToGarage(base.gameObject);
			break;
		case "ClampSpeed":
			SetSpeed(other.gameObject);
			break;
		case "insideGarage":
			RacingProvider.DragRacingController.onGarage(base.gameObject, setToGaragePosition: true);
			break;
		case "ToRaceAgain":
			RacingProvider.DragRacingController.ToRaceAgain(base.gameObject);
			break;
		case "BurnOutZone2":
			if (_vehicleData.IsRacingCar)
			{
				_vehicleData.GetPowertrain.BurnOutNow(5f).Forget();
			}
			break;
		}
	}

	private void OnTriggerEnterNoFreeMode(Collider other)
	{
		if (FlagsStorage.IsFreeDriveMode || !_vehicleData.GetCarInputManager || !LevelScript || !LevelsProvider.RacingCarLevel || _vehicleData.GetCarInputManager.State == 1 || !(V.Car == base.gameObject) || !(_vehicleData.Driver == V.DriverPerson) || !(other.name == "Finish"))
		{
			return;
		}
		_disposableReleaseBrake?.Dispose();
		_disposableReleaseBrake = Observable.Timer(TimeSpan.FromSeconds(0.44999998807907104)).Subscribe((long _) =>
		{
			_vehicleData.GetCarInputManager.State = 3;
		}).AddTo(this);
		_disposableCarFinish?.Dispose();
		bool car2Finished = FlagsStorage.Car2Finished;
		_disposableCarFinish = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			if (!car2Finished)
			{
				LevelScript.OpenLevelDoneMenu();
			}
			else
			{
				LevelScript.FailString = new LocalizedString("MobileCarController.2NtuneOrChangeCar");
				LevelScript.OpenFailMenu();
			}
		}).AddTo(this);
		_vehicleData.GetCarInputManager.State = 1;
	}

	private void OnTriggerEnterLevelAndCheckPoint(Collider other)
	{
		if (V.Car != base.gameObject)
		{
			return;
		}
		string text = other.name;
		if (FlagsStorage.RaceStarted && text == "CheckPoint")
		{
			other.gameObject.SetActive(value: false);
			RacingCheckPoint component = other.gameObject.GetComponent<RacingCheckPoint>();
			component.carPassed = true;
			++FreeDriveProvider.DragRacingControlMultiplayer.checkPointCount;
			if ((bool)component.previousCheckpoint && !component.previousCheckpoint.GetComponent<RacingCheckPoint>().carPassed)
			{
				FreeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.OnMissedCheckPoint();
				++FreeDriveProvider.DragRacingControlMultiplayer.checkPointCount;
			}
		}
		if (!LevelScript || !LevelScript.ParkingBuilding)
		{
			return;
		}
		string text2 = text;
		if (!(text2 == "inside parking"))
		{
			if (text2 == "outside parking" && FlagsStorage.LevelStarted)
			{
				LevelScript.ParkingBuilding.gameObject.SetActive(value: false);
			}
		}
		else
		{
			LevelScript.ParkingBuilding.gameObject.SetActive(value: true);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (FlagsStorage.IsPhotoMode)
		{
			return;
		}
		OnTriggerEnterIsFreeMode(other);
		OnTriggerEnterNoFreeMode(other);
		OnTriggerEnterLevelAndCheckPoint(other);
		string text = other.name;
		if (FlagsStorage.RacingGameMode)
		{
			OnTriggerEnterRaceMode(other);
			return;
		}
		if (V.Car == base.gameObject)
		{
			if (other.CompareTag("SceneSettings"))
			{
				V.SceneControl.onSceneSettingEnter(text);
			}
			if (text == "StayInColliderLink" && other.TryGetComponent<StayInColliderLink>(out var component))
			{
				component.OnEnter();
			}
			if ((bool)V.DriverPerson)
			{
				V.DriverPerson.GetComponent<DriverActionController>().OnTriggerMutual(other).Forget();
			}
			if (LevelsProvider.CurrentLevelData.Type != LevelType.Chasing)
			{
				LevelScript.EnteredParkingCube = false;
				switch (text)
				{
				case "FLeft":
					other.gameObject.layer = 11;
					LevelScript.FLeft = true;
					LevelScript.EnteredParkingCube = true;
					break;
				case "FRight":
					other.gameObject.layer = 11;
					LevelScript.FRight = true;
					LevelScript.EnteredParkingCube = true;
					break;
				case "RLeft":
					other.gameObject.layer = 11;
					LevelScript.RLeft = true;
					LevelScript.EnteredParkingCube = true;
					break;
				case "RRight":
					other.gameObject.layer = 11;
					LevelScript.RRight = true;
					LevelScript.EnteredParkingCube = true;
					break;
				case "LevelDone":
					if (!FlagsStorage.LevelStarted)
					{
						break;
					}
					LevelScript.FLeft = (LevelScript.FRight = (LevelScript.RLeft = (LevelScript.RRight = true)));
					if (LevelsProvider.CurrentLevelData.ID != 64)
					{
						break;
					}
					foreach (WheelProvider allWheel in V.Car.GetComponent<Axles>().AllWheels)
					{
						if (allWheel.OnGroundDown)
						{
							LevelScript.FLeft = (LevelScript.FRight = (LevelScript.RLeft = (LevelScript.RRight = false)));
							break;
						}
					}
					if (Vector3.Dot(V.Car.transform.forward, Vector3.forward) < 0.8f)
					{
						LevelScript.FLeft = (LevelScript.FRight = (LevelScript.RLeft = (LevelScript.RRight = false)));
					}
					break;
				case "action":
					LevelsProvider.LevelScript.OnActionEntered();
					break;
				}
				LevelScript.IsShowNewMessage = true;
			}
			if (SceneManager.GetActiveScene().buildIndex != 13)
			{
				switch (text)
				{
				case "show other":
					LevelScript.OtherSceneBuildings.gameObject.SetActive(value: true);
					break;
				case "hide other":
					LevelScript.OtherSceneBuildings.gameObject.SetActive(value: false);
					break;
				case "outside parking":
					break;
				}
				return;
			}
			if (text == "ProceduralDoorFrontCollider" && other.TryGetComponent<ProceduralDoorFrontCollider>(out var component2))
			{
				component2.Enter?.Invoke();
				Debug.Log($"the door is {other}", other.gameObject);
			}
			if (text == "ProceduralDoorBackCollider" && other.TryGetComponent<ProceduralDoorBackCollider>(out var component3))
			{
				component3.Enter?.Invoke();
				Debug.Log($"the door is {other}", other.gameObject);
			}
			if (other.CompareTag("speedCheck"))
			{
				float speedKmh = _vehicleData.GetPowertrain.SpeedKmh;
				if (other.TryGetComponent<RewardBySpeed>(out var component4))
				{
					component4.CheckSpeed(speedKmh);
				}
				else
				{
					if (!other.TryGetComponent<BannerController>(out var component5) || !(_vehicleData.Driver == V.DriverPerson))
					{
						return;
					}
					component5.SetSpeed(speedKmh);
					if (speedKmh > 200f && _canReactionOnBanner)
					{
						_canReactionOnBanner = false;
						++FreeDriveProvider.FreeDriveDB.speedBanner;
						Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
						{
							_canReactionOnBanner = true;
						}).AddTo(this);
					}
				}
				return;
			}
			if (text == "Fuel")
			{
				other.GetComponent<FuelPlace>().OnFuelEnter(base.gameObject, multiplayerCar: false);
				return;
			}
			if (text == "Repair")
			{
				if (!FlagsStorage.IsPassenger && !_repairTurnedOff && (!FlagsStorage.StartInHome || (FlagsStorage.StartInHome && FreeDriveProvider.FreeDriveControl.isHomeLoaded)))
				{
					_repairTurnedOff = true;
					FreeDriveProvider.QuickCarSettings.RepairCar();
					other.transform.GetChild(0).gameObject.SetActive(value: false);
					DisableEndEnableRepairTrigger(other);
					if (!_repairSoundTrigger)
					{
						_repairSoundTrigger = true;
						AudioWrapper.SetSwitch(Scroll.UiSwitch, _carRepair, base.gameObject);
						AudioWrapper.PostEvent(Scroll.UiEvent, base.gameObject);
					}
				}
				other.transform.GetChild(0).gameObject.SetActive(value: false);
				return;
			}
			if (other.gameObject.CompareTag("TargetIndex") && !FlagsStorage.KeepRow)
			{
				TargetSet component6 = other.gameObject.GetComponent<TargetSet>();
				if (component6.targetIndexToSetMountain != 0 && (bool)_vehicleData && !FlagsStorage.RacingGameMode)
				{
					FreeDriveProvider.FreeDriveControl.current_tar_indexMountain = component6.targetIndexToSetMountain;
					V.CurrentIndexArrayPositionCar = component6.targetIndexToSetMountain;
				}
				if (component6.targetIndexToSetMainRoad != 0 && (bool)_vehicleData && !FlagsStorage.RacingGameMode)
				{
					FreeDriveProvider.FreeDriveControl.current_tar_index = component6.targetIndexToSetMainRoad;
					_vehicleData.KeepRowComposition.CurrentTargetIndex = component6.targetIndexToSetMainRoad;
					V.CurrentIndexArrayPositionCar = component6.targetIndexToSetMainRoad;
				}
				if (component6.setSpeed != 0f && _vehicleData.GetPowertrain.SpeedMps > component6.setSpeed)
				{
					GetComponent<Powertrain>().Velocity = Vector3.ClampMagnitude(GetComponent<Powertrain>().Velocity * 3.6f, component6.setSpeed);
					V.CoreGameManager.SetMessage(new LocalizedString("CarTriggers.CarSlowedDown"), 2f);
				}
				return;
			}
			switch (text)
			{
			case "OutOfRacePlace":
				_ = FlagsStorage.RaceStarted;
				break;
			case "SetNewTire":
			{
				GameObject gameObject = other.transform.GetChild(0).gameObject;
				if (!FlagsStorage.IsPassenger && gameObject.activeSelf)
				{
					GetComponent<VehicleData>().WheelComposition.SetNewTire();
					if (!_repairSoundTrigger)
					{
						_repairSoundTrigger = true;
						AudioWrapper.SetSwitch(Scroll.UiSwitch, _wheelRepair, base.gameObject);
						AudioWrapper.PostEvent(Scroll.UiEvent, base.gameObject);
					}
				}
				gameObject.SetActive(value: false);
				break;
			}
			default:
				_ = text == "TriggerDragRacing";
				break;
			}
			return;
		}
		switch (text)
		{
		case "action1":
			GetComponent<Rigidbody>().isKinematic = true;
			break;
		case "Fuel":
			other.GetComponent<FuelPlace>().OnFuelEnter(base.gameObject, multiplayerCar: true);
			break;
		case "Repair":
			other.transform.GetChild(0).gameObject.SetActive(value: false);
			break;
		}
		if (other.CompareTag("restrict"))
		{
			GameObject firstCarMultiPlayer = FreeDriveProvider.FirstCarMultiPlayer;
			if (firstCarMultiPlayer == base.gameObject)
			{
				FreeDriveProvider.FreeDriveControl.CarToRoad(firstCarMultiPlayer);
			}
		}
	}

	private void OnTriggerEnterIsFreeMode(Collider other)
	{
		string text = other.name;
		if (!FlagsStorage.IsFreeDriveMode || !_vehicleData.GetCarInputManager)
		{
			return;
		}
		string text2 = text;
		if (!(text2 == "Tall"))
		{
			if (text2 == "ShakeFinishGate")
			{
				Powertrain getPowertrain = _vehicleData.GetPowertrain;
				float highSpeed = (getPowertrain.OnLineCar ? getPowertrain.SpeedMpsOnline : getPowertrain.SpeedMps);
				other.transform.parent.GetComponent<ShakeFinishGate>().SetHighSpeed(highSpeed);
			}
			return;
		}
		bool result;
		ObscuredInt obscuredInt;
		if (other.gameObject.TryGetComponent<TollTriger>(out var component))
		{
			component.Open(base.gameObject);
			if (!FlagsStorage.IsPassenger && V.Car == base.gameObject)
			{
				ObscuredInt tall = _pricesService.Tall1;
				CoreGameManager coreGameManager = V.CoreGameManager;
				obscuredInt = tall;
				coreGameManager.SetMessage("-" + obscuredInt.ToString(), 1f);
				AdvancedInt price = new AdvancedInt(tall);
				CurrencyManagerController.Instance.TryMoneySpend(out result, price);
			}
			return;
		}
		if (!other.gameObject.TryGetComponent<TollTrigger2>(out var component2))
		{
			return;
		}
		component2.Open(base.gameObject);
		if (FlagsStorage.IsPassenger || !(V.Car == base.gameObject))
		{
			return;
		}
		float mass = _rigidbody.mass;
		if (mass > 1000f)
		{
			if (!(mass < 2000f))
			{
				goto IL_0164;
			}
			obscuredInt = _pricesService.Tall2LargeCar;
		}
		else
		{
			if (!(mass < 1200f))
			{
				goto IL_0164;
			}
			obscuredInt = _pricesService.Tall2SmallCar;
		}
		goto IL_0171;
		IL_0171:
		ObscuredInt obscuredInt2 = obscuredInt;
		V.CoreGameManager.SetMessage($"-{obscuredInt2}", 1f);
		AdvancedInt price2 = new AdvancedInt(obscuredInt2);
		CurrencyManagerController.Instance.TryMoneySpend(out result, price2, showShop: false, spendAnyway: true);
		return;
		IL_0164:
		obscuredInt = _pricesService.Tall2;
		goto IL_0171;
	}

	private void OnTriggerExit(Collider other)
	{
		if (FlagsStorage.RacingGameMode)
		{
			return;
		}
		if (V.Car == base.gameObject)
		{
			string text = other.name;
			if ((bool)V.DriverPerson)
			{
				V.DriverPerson.GetComponent<DriverActionController>().onTriggerExitMutual(other);
			}
			if (text == "StayInColliderLink" && other.TryGetComponent<StayInColliderLink>(out var component))
			{
				component.OnExit();
			}
			if (LevelsProvider.CurrentLevelData.Type != LevelType.Chasing)
			{
				switch (text)
				{
				case "FLeft":
					LevelScript.FLeft = false;
					break;
				case "FRight":
					LevelScript.FRight = false;
					break;
				case "RLeft":
					LevelScript.RLeft = false;
					break;
				case "RRight":
					LevelScript.RRight = false;
					break;
				case "intoBuilding":
					if (FlagsStorage.LevelStarted)
					{
					}
					break;
				case "action":
					LevelsProvider.LevelScript.OnActionExit();
					break;
				}
				LevelScript.IsShowNewMessage = true;
				switch (text)
				{
				case "NewPos":
					LevelScript.HasFixedCam = true;
					LevelsProvider.CurrentLevelData.CarInitialPos = new Vector3(623.64f, 89.97f, 1086.37f);
					LevelsProvider.CurrentLevelData.CarInitRot = new Vector3(3f, -96f, 0f);
					LevelScript.IsShowNewMessage = false;
					break;
				case "hasFixedCam":
					LevelScript.HasFixedCam = false;
					if (V.CurrentCamIndex == 6)
					{
						LevelScript.carCamerasController.SetCamera(1, V.Car.transform, newTarget: true);
						V.CurrentCamIndex = 2;
					}
					break;
				case "supermarket":
					other.GetComponent<superMarketBuilding>().EnableLights(enable: false);
					break;
				}
			}
			if (V.CoreGameManager.isFreeDrive)
			{
				if (text == "ProceduralDoorFrontCollider" && other.TryGetComponent<ProceduralDoorFrontCollider>(out var component2))
				{
					component2.Exit?.Invoke();
				}
				if (text == "ProceduralDoorBackCollider" && other.TryGetComponent<ProceduralDoorBackCollider>(out var component3))
				{
					component3.Exit?.Invoke();
				}
				switch (text)
				{
				default:
					_ = text == "TriggerDragRacing";
					break;
				case "Fuel":
					other.GetComponent<FuelPlace>().OnFuelExit();
					break;
				case "Repair":
					TryToDisposeChecks();
					_repairTurnedOff = false;
					other.transform.GetChild(0).gameObject.SetActive(value: true);
					_repairSoundTrigger = false;
					break;
				case "SetNewTire":
					other.transform.GetChild(0).gameObject.SetActive(value: true);
					_repairSoundTrigger = false;
					break;
				}
			}
		}
		else
		{
			switch (other.name)
			{
			case "action1":
				GetComponent<Rigidbody>().isKinematic = false;
				break;
			case "Fuel":
				other.GetComponent<FuelPlace>().OnFuelExitMultplayerCar();
				break;
			case "Repair":
			case "SetNewTire":
				_repairTurnedOff = false;
				other.transform.GetChild(0).gameObject.SetActive(value: true);
				break;
			}
		}
	}

	private void OnCarDamaged()
	{
		_disposableOnCarDamaged?.Dispose();
		if (V.CoreGameManager.isFreeDrive)
		{
			return;
		}
		switch (collidedObjectIndex)
		{
		case 1:
			LevelScript.FailString = new LocalizedString("TrailerTrigger.AvoidTraffic");
			break;
		case 2:
			LevelScript.FailString = new LocalizedString("TrailerTrigger.AvoidPedestrian");
			break;
		default:
			if ((bool)LevelsProvider.Trailer)
			{
				if (LevelsProvider.CurrentLevelData.LoadTrailer)
				{
					LevelScript levelScript = LevelScript;
					string failString;
					switch (_vehicleData.CarId)
					{
					case 70:
						failString = new LocalizedString("CarTriggers.TruckDamaged!");
						break;
					case 85:
					case 126:
					case 154:
					case 167:
					case 176:
					case 180:
					case 189:
					case 192:
						failString = new LocalizedString("CarTriggers.PickupDamaged!");
						break;
					case 55:
						failString = new LocalizedString("CarTriggers.HummerDamaged!");
						break;
					default:
						failString = LevelScript.FailString;
						break;
					}
					levelScript.FailString = failString;
				}
				else
				{
					LevelScript.FailString = new LocalizedString("CarTriggers.TruckDamaged!");
				}
			}
			else
			{
				LevelScript.FailString = new LocalizedString("CarTriggers.CarDamaged!");
			}
			break;
		}
		if (!FlagsStorage.EndlessCrashTime || collidedObjectIndex == 2)
		{
			LevelsAnalytic.LogFinishLevel("traffic_accident");
			LevelScript.OpenFailMenu();
			collidedObjectIndex = 0;
		}
	}

	private void OnCollidedWithCarFollow()
	{
		LevelScript.FailString = new LocalizedString("CarTriggers.KeeptheDistance!");
		LevelScript.OpenFailMenu();
	}

	private void ResetAnim()
	{
	}

	private void OnCarAccident()
	{
		if ((bool)RacingProvider?.DragRacingController)
		{
			RacingProvider.DragRacingController.OnCarAccident();
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!FlagsStorage.IsLevelScene && _vehicleData.Driver != null)
		{
			TaxiManager taxiManager = Global.TaskManager.TaxiManager;
			if ((bool)V.DriverPerson)
			{
				int ownerActorNr = PhotonView.Get(V.DriverPerson).OwnerActorNr;
				if (!other.collider.CompareTag("DriverPerson") && taxiManager.clients.ContainsKey(ownerActorNr) && base.gameObject == taxiManager.clients[ownerActorNr].Car)
				{
					Global.TaskManager.CancelReward();
				}
			}
		}
		float num = Mathf.Clamp01(other.relativeVelocity.magnitude * 0.1f);
		num *= Mathf.Clamp01(0.3f + Mathf.Abs(Vector3.Dot(other.relativeVelocity.normalized, other.contacts[0].normal)));
		if (other.gameObject.CompareTag("ped") || other.gameObject.CompareTag("DriverPerson"))
		{
			num = 0.2f;
		}
		if (FlagsStorage.RacingGameMode && base.gameObject == V.Car && num > 0.01f && other.transform.TryGetComponent<VehicleData>(out var _))
		{
			_disposableOnCarAccident?.Dispose();
			_disposableOnCarAccident = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				OnCarAccident();
			}).AddTo(this);
		}
		if (num <= 0.1f)
		{
			return;
		}
		if (base.gameObject == V.Car)
		{
			if ((bool)_vehicleData)
			{
				_vehicleData.GetPowertrain.CancelDrift();
			}
			if (FreeDriveProvider != null && (bool)FreeDriveProvider.CarCameras)
			{
				FreeDriveProvider.CarCameras.SetShake(0.4f, 3f, 2f);
			}
			if (num > 0.4f && (bool)V.CoreGameManager.Spark)
			{
				V.CoreGameManager.Spark.transform.position = other.contacts[0].point;
				V.CoreGameManager.Spark.GetComponent<ParticleSystem>().Play();
			}
			if (!_vehicleData.CarHealthComposition.Damaged && other.gameObject.layer != LayerViaName.DriverPersonLayer && (double)num > 0.8)
			{
				if (FreeDriveProvider != null && (bool)FreeDriveProvider.FreeDriveDB)
				{
					++FreeDriveProvider.FreeDriveDB.car_collided;
				}
				if (!ScenesHelper.IsParkingLevelActive() && (bool)_vehicleData.WindowComposition.WindowMat)
				{
					_vehicleData.WindowComposition.WindowMat.SetFloat(ShaderPropertyIDs.Shattered, 1f);
				}
			}
			_disposableResetAnim?.Dispose();
			_disposableResetAnim = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				ResetAnim();
			}).AddTo(this);
		}
		if (!other.transform.CompareTag("Road") && !other.transform.CompareTag("DriverPerson") && other.gameObject.layer != LayerViaName.RoadLayer && other.gameObject.layer != LayerViaName.LoadLayer)
		{
			float num2 = 0.3f;
			if (!FlagsStorage.IsCarDamaged && FlagsStorage.LevelStarted)
			{
				if (other.gameObject.CompareTag("TrafficCar"))
				{
					collidedObjectIndex = 1;
				}
				else if (other.gameObject.CompareTag("ped"))
				{
					num2 = 0f;
					collidedObjectIndex = 2;
				}
				else
				{
					collidedObjectIndex = 0;
				}
				bool flag = collidedObjectIndex == 1;
				if ((flag && num > 0.4f) || (!flag && num > LevelsProvider.CurrentLevelData.LimitCarDamaged))
				{
					if ((FlagsStorage.EndlessCrashTime || LevelsProvider.CurrentLevelData.ignoreCollision) && collidedObjectIndex != 2)
					{
						return;
					}
					_disposableOnCarDamaged = Observable.Timer(TimeSpan.FromSeconds(num2)).Subscribe((long _) =>
					{
						OnCarDamaged();
					}).AddTo(this);
					FlagsStorage.IsCarDamaged = true;
					LevelsProvider.LevelScript.SetSmoothCamMove(other.contacts[0].point, other.contacts[0].point + Vector3.up * 3f);
				}
			}
		}
		if ((bool)_soundController && (bool)_soundController.ExhaustSystem)
		{
			_soundController.ExhaustSystem.Play();
		}
	}

	private void DisableEndEnableRepairTrigger(Collider other)
	{
		VehicleDamage component = GetComponent<VehicleDamage>();
		if (component.repair)
		{
			TryToDisposeChecks();
			StartCheckInInterval(other, component);
			StartCheckEndOfRepair(other, component);
		}
	}

	private void StartCheckInInterval(Collider repairCollider, VehicleDamage vehicleDamage)
	{
		_checkDisposable1 = (from _ in Observable.Interval(TimeSpan.FromSeconds(0.30000001192092896)).Skip(1)
			where _repairTurnedOff
			where !IsCarAround(repairCollider)
			select _).Subscribe((long _) =>
		{
			CastToCheckCarInRepair(repairCollider);
		}).AddTo(repairCollider);
	}

	private void StartCheckEndOfRepair(Collider repairCollider, VehicleDamage vehicleDamage)
	{
		_checkDisposable2 = (from _ in Observable.EveryUpdate().Skip(1)
			where _repairTurnedOff
			where !vehicleDamage.repair
			select _).Take(1).Subscribe((long _) =>
		{
			CastToCheckCarInRepair(repairCollider);
		}).AddTo(repairCollider);
	}

	private bool IsCarAround(Collider repairCollider)
	{
		float num = 3f;
		return Vector3.Distance(base.transform.position, repairCollider.transform.position) < num;
	}

	private void CastToCheckCarInRepair(Collider repairCollider)
	{
		int mask = LayerMask.GetMask("car");
		Vector3 position = repairCollider.transform.position;
		RaycastHit[] array = Physics.CapsuleCastAll(position, position + Vector3.up * 2f, 0.5f, Vector3.up, 2f, mask);
		bool flag = false;
		RaycastHit[] array2 = array;
		foreach (RaycastHit raycastHit in array2)
		{
			if (raycastHit.collider.attachedRigidbody == base.gameObject.GetComponent<Rigidbody>())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			repairCollider.transform.GetChild(0).gameObject.SetActive(value: true);
			_repairTurnedOff = false;
			TryToDisposeChecks();
		}
	}

	private void TryToDisposeChecks()
	{
		_checkDisposable1?.Dispose();
		_checkDisposable2?.Dispose();
	}
}
