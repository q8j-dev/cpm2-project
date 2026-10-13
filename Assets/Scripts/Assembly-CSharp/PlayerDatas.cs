using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Common;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.PlayerData;
using CPM.Scripts.VinylDrawer;
using CPM.Transmission;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DataEntities.Vinyl;
using Persistence.Systems;
using Photon.Pun;
using RootMotion.FinalIK;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement.Settings;

public class PlayerDatas : MonoBehaviour
{
	public class CarLoadingUtility
	{
		private IPrefabFactory _prefabFactory;

		private ICarTuningFactory _tuningFactory;

		private CarFactoryService _carFactoryService;

		private ICarInfoService _carInfoService;

		private DrawQueue _drawQueue;

		private LevelsProvider _levelsProvider;

		private CarInputManager _carInputManager;

		public static Vector3 ZeroSpawnPoint => Vector3.zero;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, ICarTuningFactory tuningFactory, CarFactoryService carFactoryService, DrawQueue drawQueue, LevelsProvider levelsProvider, ICarInfoService carInfoService, CarInputManager carInputManager)
		{
			_levelsProvider = levelsProvider;
			_drawQueue = drawQueue;
			_carInfoService = carInfoService;
			_tuningFactory = tuningFactory;
			_prefabFactory = prefabFactory;
			_carFactoryService = carFactoryService;
			_carInputManager = carInputManager;
		}

		public GameObject LoadRacingCarLevel(int carId, Vector3 pos, Vector3[] targetsFollow, int initIndex)
		{
			GameObject obj = Resources.Load<GameObject>($"car{carId}");
			_levelsProvider.RacingCarLevel = _prefabFactory.Instantiate(obj, pos, Quaternion.Euler(Vector3.zero));
			CarInfo carInfo = _carInfoService.CreateDefault(carId);
			CarEntity carEntity = _tuningFactory.AssemblyCar(carInfo);
			GameObject racingCarLevel = _levelsProvider.RacingCarLevel;
			if (racingCarLevel.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				component.IsAiCar = true;
				component.CarEntity = carEntity;
				component.Init(initSuspension: true);
			}
			if (!racingCarLevel.TryGetComponent<CarAIController>(out var component2))
			{
				component2 = racingCarLevel.AddComponent<CarAIController>();
			}
			component2.AiType = AiType.LevelRacingCar;
			component2.targets = targetsFollow;
			component2.TargetSpeed = 5f;
			component2.CurrentTarIndex = initIndex;
			component2.Sign = -1;
			component2.CurrentWay = 1;
			component2.StopCar = true;
			component2.AutoReverse = false;
			if (racingCarLevel.TryGetComponent<VehicleData>(out var component3))
			{
				component3.SetupAsRacingCar();
				component3.BodyLoadingComposition.LoadFromResource(immediately: false).Forget();
				component3.InteriorsComposition.ShowLowInterior();
				if (!component3.CarMat)
				{
					component3.ColorsService.CreateFirstMat(component3);
				}
				component3.CarMat.color = UnityEngine.Random.ColorHSV();
				if ((bool)component3.WheelComposition.WheelMatFront1)
				{
					component3.WheelComposition.WheelMatFront1.color = UnityEngine.Random.ColorHSV();
				}
				WheelConstructor.SetWheel(component3, carInfo.TuningData).Forget();
			}
			return racingCarLevel;
		}

		public void LoadCarConfig(GameObject car, MainCarValues carValues)
		{
			carValues = UnityEngine.Object.Instantiate(carValues);
			carValues.RuntimeInstance = true;
			CarDefaultTuningDataConfig tuning = CarTuningFactory.GetDefaultCarTuningDataConfig(carValues.CarID).Tuning;
			ChassisData chassisData = tuning.ChassisData;
			if (carValues.bodyMass == 0f)
			{
				carValues.bodyMass = chassisData.Weight;
			}
			float num = 0f;
			if (car.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				_ = component.SwapEngine(carValues.CarID);
				component.SetFuelInTank(chassisData.FuelTankCapacity);
				num = component.CarEntity.MaintenanceInfo.FuelInTank * 0.73722f;
				EngineItem baseEngine = tuning.GetBaseEngine();
				component.CarEntity.MaintenanceInfo.OilCapacity = (baseEngine.RotorHousing ? baseEngine.RotorHousing.OilCapacity : baseEngine.Block.OilCapacity);
				component.CarEntity.MaintenanceInfo.OilLife = 100f;
				component.CarEntity.MaintenanceInfo.FrontTyreHealth = new float[2] { 1f, 1f };
				component.CarEntity.MaintenanceInfo.RearTyreHealth = new float[2] { 1f, 1f };
			}
			CarTuningData carTuningData = new CarTuningData();
			carTuningData.FrontWheelData = carValues.FrontWheelData;
			carTuningData.RearWheelData = carValues.RearWheelData;
			if (carValues.UseStockFrontData)
			{
				carTuningData.FrontWheelData.SuspensionData = tuning.FrontWheelData.SuspensionData.Clone() as SteerSuspensionData;
			}
			if (carValues.UseStockRearData)
			{
				carTuningData.RearWheelData.SuspensionData = tuning.RearWheelData.SuspensionData.Clone() as SuspensionData;
			}
			if (car.TryGetComponent<VehicleData>(out var component2))
			{
				component2.CarId = carValues.CarID;
				component2.CarValues = carValues;
				component2.CarChildsReferences.FuelTank?.SetupForRace(chassisData.FuelTankCapacity, chassisData.FuelTankCapacity);
				component2.ColorsService.CreateFirstMat(component2);
				component2.BodyLoadingComposition.LoadFromResource(immediately: false).Forget();
				_drawQueue.Enqueue(component2.VinylsComposition);
			}
			if (car.TryGetComponent<Powertrain>(out var component3))
			{
				component3.GetDesc(out var desc);
				int num2 = 70;
				desc.Aero.Cx = chassisData.Cx;
				desc.Aero.Sx = chassisData.Width * chassisData.Height * 0.84f;
				desc.Aero.FrontDownforce = chassisData.FrontDownforce;
				desc.Aero.RearDownforce = chassisData.RearDownforce;
				desc.Chassis.Mass = carValues.bodyMass + num + (float)num2;
				desc.Chassis.Length = chassisData.Length;
				desc.Chassis.Width = chassisData.Width;
				desc.Chassis.Height = chassisData.Height;
				desc.Chassis.FrontPercent = chassisData.WeightDistribution;
				desc.Chassis.UpPercent = chassisData.WeightDistributionVertical;
				desc.Chassis.BrakeBalance = (int)chassisData.BrakeBalance;
				carTuningData.FrontWheelData.SuspensionData.GetShockAbsorberParams(out var rate);
				carTuningData.FrontWheelData.SuspensionData.GetSpringParams(out var specs);
				desc.FrontSuspension.Camber = carTuningData.FrontWheelData.SuspensionData.Camber;
				desc.FrontSuspension.ToeIn = carTuningData.FrontWheelData.SuspensionData.ToeIn;
				desc.FrontSuspension.Caster = carTuningData.FrontWheelData.SuspensionData.Caster;
				desc.FrontSuspension.Ackerman = carTuningData.FrontWheelData.SuspensionData.Ackerman;
				desc.FrontSuspension.MaxSteerAngle = tuning.FrontWheelData.SuspensionData.MaxSteerAngle;
				desc.FrontSuspension.BrakeDiskTorque = carTuningData.FrontWheelData.BrakesData.GetFullBrakeTorque;
				desc.FrontSuspension.BrakeDiscType = carTuningData.FrontWheelData.BrakesData.GetBrakeDiscType;
				desc.FrontSuspension.AntirollBar = carTuningData.FrontWheelData.SuspensionData.AntiSwayBarForce;
				desc.FrontSuspension.Bump = rate.x;
				desc.FrontSuspension.Rebound = rate.y;
				desc.FrontSuspension.SpringLength = specs.x;
				desc.FrontSuspension.Stiffness = specs.y;
				carTuningData.RearWheelData.SuspensionData.GetShockAbsorberParams(out rate);
				carTuningData.RearWheelData.SuspensionData.GetSpringParams(out specs);
				desc.RearSuspension.Camber = carTuningData.RearWheelData.SuspensionData.Camber;
				desc.RearSuspension.ToeIn = carTuningData.RearWheelData.SuspensionData.ToeIn;
				desc.RearSuspension.BrakeDiskTorque = carTuningData.RearWheelData.BrakesData.GetFullBrakeTorque;
				desc.RearSuspension.BrakeDiscType = carTuningData.RearWheelData.BrakesData.GetBrakeDiscType;
				desc.RearSuspension.AntirollBar = carTuningData.RearWheelData.SuspensionData.AntiSwayBarForce;
				desc.RearSuspension.Bump = rate.x;
				desc.RearSuspension.Rebound = rate.y;
				desc.RearSuspension.SpringLength = specs.x;
				desc.RearSuspension.Stiffness = specs.y;
				desc.Lsd.FrontLSD.Type = carValues.FrontLsd;
				desc.Lsd.RearLSD.Type = carValues.RearLsd;
				desc.Lsd.FrontLSD.LockPercent = carValues.FrontLsdLockPercent;
				desc.Lsd.RearLSD.LockPercent = carValues.RearLsdLockPercent;
				if (car.TryGetComponent<Axles>(out var component4))
				{
					foreach (WheelProvider wheel in component4.FrontAxle.Wheels)
					{
						wheel.SetRimSize(carTuningData.FrontWheelData.wheelSize, carTuningData.FrontWheelData.rimWidth);
						wheel.SetTyreHealth(1f, carTuningData.FrontWheelData.tyreType);
						if ((bool)wheel.WheelModel)
						{
							wheel.WheelModel.BrakeDisk.Resize(carTuningData.FrontWheelData.BrakesData.GetBrakeDiskDiameter());
						}
					}
					desc.FrontTyre.DiscWidth = carTuningData.FrontWheelData.rimWidth;
					desc.FrontTyre.DiscDiam = carTuningData.FrontWheelData.wheelSize;
					desc.FrontTyre.TyreType = carTuningData.FrontWheelData.tyreType;
					desc.FrontTyre.Width = carTuningData.FrontWheelData.tyreWidth;
					desc.FrontTyre.Profile = carTuningData.FrontWheelData.tyreProfile;
					desc.FrontTyre.Pressure = Utils.Bar2Pa(carTuningData.FrontWheelData.Pressure);
					desc.FrontTyre.Mass = component3.CalculateWheelMass(carTuningData.FrontWheelData);
					desc.FrontTyre.FrictionMultiplier = 1.3f;
					desc.RearTyre.DiscWidth = carTuningData.RearWheelData.rimWidth;
					desc.RearTyre.DiscDiam = carTuningData.RearWheelData.wheelSize;
					desc.RearTyre.TyreType = carTuningData.RearWheelData.tyreType;
					desc.RearTyre.Width = carTuningData.RearWheelData.tyreWidth;
					desc.RearTyre.Profile = carTuningData.RearWheelData.tyreProfile;
					desc.RearTyre.Pressure = Utils.Bar2Pa(carTuningData.RearWheelData.Pressure);
					desc.RearTyre.Mass = component3.CalculateWheelMass(carTuningData.RearWheelData);
					desc.RearTyre.FrictionMultiplier = 1.5f;
					foreach (WheelProvider wheel2 in component4.RearAxle.Wheels)
					{
						wheel2.SetRimSize(carTuningData.RearWheelData.wheelSize, carTuningData.RearWheelData.rimWidth);
						wheel2.SetTyreHealth(1f, carTuningData.RearWheelData.tyreType);
						if ((bool)wheel2.WheelModel)
						{
							wheel2.WheelModel.BrakeDisk.Resize(carTuningData.RearWheelData.BrakesData.GetBrakeDiskDiameter());
						}
					}
					component3.Automatic = true;
					component3.ManualHelper = false;
					component3.IsAutomaticGearbox = true;
					desc.Engine.IdleRpm = carValues.IdleRPM;
					desc.Engine.TurboPressure = Utils.Bar2Pa(carValues.turboPressure);
					desc.Engine.TurboRange = new float2(carValues.minTurboRPM, carValues.maxTurboRPM);
					desc.Engine.TurboType = carValues.Induction;
					desc.Engine.MaxRpm = carValues.maxRPM;
					desc.Engine.RedlineRpm = carValues.redlineRPM;
					desc.Engine.InertiaRatio = carValues.EngineInertiaRatio;
					desc.Engine.LcRpm = carValues.LCrpm;
					desc.Engine.LcTime = 100000f;
					desc.Engine.Options.Lc = true;
					desc.Engine.Orientation = chassisData.Orientation;
					desc.GearBox.GearShiftingDelay = carValues.ShiftTime;
					desc.GearBox.Gears = carValues.gears;
					if (carValues.finalGearRatio != 0f)
					{
						desc.GearBox.FinalDrive = carValues.finalGearRatio;
					}
					desc.Transmission.TransmissionLossCoef = carValues.GearBoxLoss / 100f;
					desc.Transmission.Kind = carValues.Gearbox;
					desc.Transmission.GearType = carValues.drivingWheel;
					desc.Transmission.TorqueSplit = carValues.TorqueSplit4Wd;
					desc.Engine.TorqueCurveSamples = Utils.GenerateTorqueCurve(carValues.torque, carValues.torqueRPM, (int)carValues.Induction, carValues.redlineRPM, carValues.maxRPM, carValues.minTurboRPM, carValues.maxTurboRPM, carValues.IdleRPM);
					Utils.GetEngineParams(desc.Engine.TorqueCurveSamples, carValues.Induction, carValues.turboPressure, out desc.Engine.MaxTorque, out desc.Engine.MaxTorqueRpm);
					desc.Transmission.ClutchMaxTorque = Mathf.Round(desc.Engine.MaxTorque * 3f);
					component3.Gear = 1;
					component3.SetDynoData(desc, apply: true);
					component3.SetAssits(carValues.Options);
				}
				WheelConstructor.SetWheel(component2, carTuningData, 0, randomWheelColor: true).Forget();
			}
			if (car.TryGetComponent<SoundController>(out var component5))
			{
				component5.HasBurble = true;
				component5.HasExhaustShot = true;
				component5.BurbleType = UnityEngine.Random.Range(1, 5);
				component5.BlowOffType = UnityEngine.Random.Range(0, 5);
				component5.SuperChargerType = UnityEngine.Random.Range(0, 3);
				component5.RefreshExhaustBank();
			}
			if (!car.TryGetComponent<BodyKits>(out var component6))
			{
				return;
			}
			if (carValues.BodyKitTypeInstall.Length != 0)
			{
				component6.TypesToIntall = carValues.BodyKitTypeInstall;
				component6.randomBodyKit = false;
				return;
			}
			component6.TypesToIntall = new int[8];
			for (int i = 0; i < component6.TypesToIntall.Length; i++)
			{
				component6.TypesToIntall[i] = -2;
			}
			component6.randomBodyKit = true;
		}

		private GameObject CreateRacingCarBase(MainCarValues mainCarValues, Vector3 pos, AiType aiType, bool markAiCar, out VehicleData carData, out CarAIController aiController)
		{
			GameObject obj = Resources.Load<GameObject>($"car{mainCarValues.CarID}");
			GameObject gameObject = _prefabFactory.Instantiate(obj, pos, Quaternion.Euler(Vector3.zero));
			MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in components)
			{
				if ((bool)monoBehaviour)
				{
					monoBehaviour.enabled = true;
				}
			}
			if (gameObject.TryGetComponent<VehicleData>(out carData))
			{
				carData.SetupAsRacingCar();
			}
			if (markAiCar && gameObject.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				component.IsAiCar = true;
			}
			LoadCarConfig(gameObject, mainCarValues);
			if (gameObject.TryGetComponent<BodyKits>(out var component2))
			{
				component2.LoadAllKits();
			}
			gameObject.AddComponent<RacingCarController>();
			if (gameObject.TryGetComponent<CarAIController>(out aiController))
			{
				aiController.Sign = 1;
				aiController.TargetSpeed = 0f;
				aiController.SteerAssistent = false;
				aiController.AiType = aiType;
				aiController.randomOffset = UnityEngine.Random.insideUnitSphere * 0.7f;
				aiController.randomOffset.y = 0f;
			}
			return gameObject;
		}

		public GameObject LoadDragRacingCar(MainCarValues mainCarValues, Vector3 pos)
		{
			GameObject gameObject = CreateRacingCarBase(mainCarValues, pos, AiType.DragRacingCar, markAiCar: false, out var carData, out var _);
			if ((bool)carData)
			{
				GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<RacingCarTriggerControl>().Initialize(carData);
			}
			if (mainCarValues.NickName.Trim() == "")
			{
				mainCarValues.NickName = ((mainCarValues.gender == 0) ? ChatFilter.maleRandomName[UnityEngine.Random.Range(0, ChatFilter.maleRandomName.Length)] : ChatFilter.femaleRandomName[UnityEngine.Random.Range(0, ChatFilter.femaleRandomName.Length)]);
			}
			if (gameObject.TryGetComponent<Powertrain>(out var component))
			{
				component.StartEngine(starterSound: false);
			}
			return gameObject;
		}

		public async UniTask<GameObject> LoadCircuitRacingCar(MainCarValues mainCarValues, Vector3 pos)
		{
			await Resources.LoadAsync<GameObject>($"car{mainCarValues.CarID}");
			GameObject car = CreateRacingCarBase(mainCarValues, pos, AiType.CircuitRacingCar, markAiCar: true, out var carData, out var aiController);
			if ((bool)aiController)
			{
				CircuitCarStearing stearing = (aiController.circuitCarSteering = aiController.gameObject.AddComponent<CircuitCarStearing>());
				stearing.AiController = aiController;
				stearing.MinSpeedThreshold = 150f;
				stearing.BrakeSpeedInPercent = 0.1f;
				stearing.MinAngleThreshold = 0.1f;
				stearing.NextTargetCoefficient = 1f;
				stearing.SteeringToSpeedCurve = mainCarValues.steeringToSpeedCurve;
				if (mainCarValues.speedAdjustmentCoefficient != 0f)
				{
					stearing.SpeedAdjustmentCoefficient = mainCarValues.speedAdjustmentCoefficient;
				}
				aiController.TargetSpeed = 500f;
				carData.DirtComposition.SkipWrite = true;
				GameObject obj = await Addressables.LoadAssetAsync<GameObject>("Circuit_label").BindTo(car);
				if (!car)
				{
					return null;
				}
				GameObject gameObject = _prefabFactory.Instantiate(obj, car.transform);
				gameObject.transform.localPosition = new Vector3(0f, carData.RoofPosition + 4.5f, 0f);
				stearing.CircuitTopUserMeshLabel = gameObject.GetComponent<CircuitTopUserMeshLabel>();
				gameObject.SetActive(value: false);
			}
			if (car.TryGetComponent<Powertrain>(out var component))
			{
				component.StartEngine(starterSound: false);
				component.IsRaceCar = true;
			}
			return car;
		}

		public void TrySetUnUsedCar(VehicleData vehicleData)
		{
			if (!vehicleData || vehicleData.isTraffiCar || vehicleData.isTruck || V.Car == vehicleData.gameObject || !ScenesHelper.IsParkingLevelActive() || FlagsStorage.RacingGameMode)
			{
				return;
			}
			if ((bool)_levelsProvider.LevelScript)
			{
				LevelType levelType = _levelsProvider.LevelScript.LevelType;
				if (levelType == LevelType.Chasing || levelType == LevelType.DragRace)
				{
					return;
				}
			}
			MonoBehaviour.print("SetUnUsedCar");
			CarInfo carInfo = _carInfoService.CreateDefault(vehicleData.CarId);
			CarEntity carEntity = _tuningFactory.AssemblyCar(carInfo);
			if (vehicleData.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				component.CarEntity = carEntity;
				component.Init(initSuspension: true);
			}
			int wheelId = UnityEngine.Random.Range(0, 63);
			WheelConstructor.SetWheel(vehicleData, wheelId);
			vehicleData.InteriorsComposition.ShowLowInterior();
			vehicleData.gameObject.AddComponent<CarAIController>();
			if ((bool)vehicleData.CarMat)
			{
				vehicleData.CarMat.color = UnityEngine.Random.ColorHSV();
			}
			if ((bool)vehicleData.WheelComposition.WheelMatFront1)
			{
				vehicleData.WheelComposition.WheelMatFront1.color = UnityEngine.Random.ColorHSV();
			}
			if ((bool)vehicleData.CarChildsReferences.FuelTank)
			{
				vehicleData.CarChildsReferences.FuelTank.enabled = false;
			}
		}

		public void UnloadCar(GameObject car, bool clearResources = true)
		{
			VehicleData component = car.GetComponent<VehicleData>();
			if ((bool)component.CarBodyReferences && (bool)component.CarBodyReferences.Body)
			{
				UnityEngine.Object.Destroy(component.CarBodyReferences.Body);
			}
			if ((bool)component.WheelComposition.TireRenderTexture)
			{
				UnityEngine.Object.Destroy(component.WheelComposition.TireRenderTexture);
			}
			if ((bool)component.WheelComposition.TireRenderNormalTexture)
			{
				UnityEngine.Object.Destroy(component.WheelComposition.TireRenderNormalTexture);
			}
			if (PhotonNetwork.IsConnected && component.TryGetComponent<PhotonView>(out var _))
			{
				PhotonNetwork.Destroy(car);
			}
			else
			{
				UnityEngine.Object.Destroy(car);
			}
		}

		public async UniTask<(CarInfo, GameObject)> LoadDefaultCar(int carId, Vector3 pos, Quaternion rot, bool tryUnloadCurrent, bool waitBodyLoad = false, float defaultY = 1000f)
		{
			CarInfo carInfo = _carInfoService.CreateDefault(carId);
			Vector3 vector = pos;
			vector.y = defaultY;
			GameObject gameObject = await LoadCarInternal(carInfo, FlagsStorage.IsCheckLoadCarDebug ? pos : vector, rot, tryUnloadCurrent, waitBodyLoad);
			if (!gameObject)
			{
				throw new Exception($"LoadDefaultCar: failed to assembly car {carId} (invalid id or factory failure)");
			}
			gameObject.transform.position = pos;
			return (carInfo, gameObject);
		}

		public async UniTask<(CarInfo, GameObject)> LoadCarFromSlot(int slotId, Vector3 pos, Quaternion rot, bool tryUnloadCurrent, bool waitBodyLoad = false, bool isMultiplayer = false, bool resetPos = true, bool isPlayerCar = true)
		{
			CarInfo carInfo = _carInfoService.GetFromSlot(slotId);
			if (carInfo == null)
			{
				throw new Exception($"CarInfo is null in slot {slotId}");
			}
			Vector3 vector = pos;
			vector.y = 1000f;
			GameObject gameObject = await LoadCarInternal(carInfo, FlagsStorage.IsCheckLoadCarDebug ? pos : vector, rot, tryUnloadCurrent, waitBodyLoad, slotId, isMultiplayer, isPlayerCar);
			if (resetPos && (bool)gameObject)
			{
				gameObject.transform.position = pos;
			}
			return (carInfo, gameObject);
		}

		public async UniTask<(CarInfo, GameObject)> LoadCarFromTemp(CarInfo tempInfo, Vector3 pos, Quaternion rot, bool tryUnloadCurrent, bool waitBodyLoad = false, bool isMultiplayer = false, bool resetPos = true)
		{
			Vector3 vector = pos;
			vector.y = 1000f;
			GameObject gameObject = await LoadCarInternal(tempInfo, FlagsStorage.IsCheckLoadCarDebug ? pos : vector, rot, tryUnloadCurrent, waitBodyLoad, -1, isMultiplayer);
			if (resetPos && (bool)gameObject)
			{
				gameObject.transform.position = pos;
			}
			return (tempInfo, gameObject);
		}

		private async UniTask<GameObject> LoadCarInternal(CarInfo carInfo, Vector3 pos, Quaternion rot, bool tryUnloadCurrent, bool waitBodyLoad, int slotId = -1, bool isMultiplayer = false, bool isPlayerCar = true)
		{
			GameObject car = V.Car;
			Vector3 prevCarPos = Vector3.zero;
			Quaternion prevCarRot = Quaternion.identity;
			Vector3 prevFollowTarget = Vector3.zero;
			if (tryUnloadCurrent && (bool)car)
			{
				prevCarPos = V.Car.transform.position;
				prevCarRot = V.Car.transform.rotation;
				prevCarRot.z = 0f;
				prevFollowTarget = _carInputManager.GetFollowPoint;
				UnloadCar(car);
				if ((bool)V.DriverPerson)
				{
					V.DriverPerson.SetActive(value: false);
				}
			}
			GameObject gameObject = await _carFactoryService.AssemblyMyCar(carInfo, pos, rot, slotId, waitBodyLoad, isMultiplayer, isPlayerCar);
			if (!gameObject)
			{
				return gameObject;
			}
			SetPhysicsSettings(gameObject, carInfo.CarID);
			if (tryUnloadCurrent)
			{
				if ((bool)V.DriverPerson)
				{
					V.DriverPerson.SetActive(value: true);
				}
				_carInputManager.SetFollowPoint(prevFollowTarget);
				gameObject.transform.SetPositionAndRotation(prevCarPos, prevCarRot);
			}
			return gameObject;
		}

		private void SetPhysicsSettings(GameObject car, int carId)
		{
			if (carId == 59)
			{
				car.transform.position += new Vector3(0f, 0.3f, 0f);
			}
		}

		private void SetCharacterJoints(GameObject car, Rigidbody rb)
		{
			if (car.TryGetComponent<CharacterJoint>(out var component))
			{
				if (SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 13)
				{
					component.breakForce = float.PositiveInfinity;
					component.breakTorque = float.PositiveInfinity;
					return;
				}
				UnityEngine.Object.Destroy(component);
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.isKinematic = true;
			}
		}

		public static void CreateMainCarValues(GameObject car)
		{
			MainCarValues mainCarValues = ScriptableObject.CreateInstance<MainCarValues>();
			mainCarValues.RuntimeInstance = true;
			bool flag = PlayerPrefs.GetInt("PRND", 1) == 0;
			mainCarValues.Gearbox = ((!flag) ? GearboxKind.AT : GearboxKind.MT);
			VehicleData component = car.GetComponent<VehicleData>();
			mainCarValues.NickName = _playerDataStorage.PlayerName;
			mainCarValues.language = (SystemLanguage)PlayerPrefs.GetInt("languageIndex", (int)Application.systemLanguage);
			mainCarValues.CarID = component.CarId;
			MonoBehaviour.print(mainCarValues.language);
			component.CarValues = mainCarValues;
			component.CarValues.car = car;
		}

		private Color GetColor(string name, Color defaultValue)
		{
			Color result = new Color(0f, 0f, 0f, 0f);
			result.r = PlayerPrefs.GetFloat(name + ".r", defaultValue.r);
			result.g = PlayerPrefs.GetFloat(name + ".g", defaultValue.g);
			result.b = PlayerPrefs.GetFloat(name + ".b", defaultValue.b);
			result.a = PlayerPrefs.GetFloat(name + ".a", defaultValue.a);
			return result;
		}
	}

	public class FriendsUtility : IFriendsUtility
	{
		public const int MaxFriendListNumber = 120;

		private Dictionary<string, int> friendIndexesDict = new Dictionary<string, int>();

		public Func<List<FriendData>> GetEntitiesFriends { get; set; }

		public List<PrefFriendData> Friends { get; private set; }

		public List<FriendData> PrefsFriends { get; private set; }

		public event Action<string> OnAuthorizedFriendDeleted;

		public event Action<FriendData> OnAuthorizedFriendAdded;

		public event Action<FriendData> OnPrefFriendAdded;

		[Inject]
		public FriendsUtility()
		{
			friendsUtility = this;
		}

		public void EnsureFriendsLoaded()
		{
			if (Friends == null)
			{
				RefillFriendsList();
			}
		}

		public void RefillFriendsList(bool keepPrefs = false)
		{
			Friends = new List<PrefFriendData>();
			if (keepPrefs && PrefsFriends != null)
			{
				for (int i = 0; i < PrefsFriends.Count; i++)
				{
					AddKeepedPrefFriend(PrefsFriends[i]);
				}
			}
			else
			{
				PrefsFriends = new List<FriendData>();
				friendIndexesDict.Clear();
				for (int j = 0; j <= 120; j++)
				{
					TryAddFriendFromPlayerPrefs(j, friendIndexesDict);
				}
			}
			List<FriendData> list = GetEntitiesFriends?.Invoke();
			if (list == null)
			{
				return;
			}
			foreach (FriendData item in list)
			{
				AddFriendFromEntities(friendIndexesDict, item);
			}
		}

		private void AddKeepedPrefFriend(FriendData friendData)
		{
			Friends.Add(new PrefFriendData
			{
				accountID = friendData.accountID,
				id = friendData.userID,
				Name = friendData.playerName
			});
		}

		private void AddFriendFromEntities(Dictionary<string, int> friendIndexesDict, FriendData data)
		{
			if (friendIndexesDict.ContainsKey(data.userID))
			{
				int index = friendIndexesDict[data.userID];
				PrefFriendData prefFriendData = Friends[index];
				if (string.IsNullOrEmpty(data.accountID.Trim()))
				{
					prefFriendData.accountID = data.accountID;
					prefFriendData.Name = data.playerName;
				}
			}
			else
			{
				Friends.Add(new PrefFriendData
				{
					accountID = data.accountID,
					id = data.userID,
					Name = data.playerName
				});
			}
		}

		private void TryAddFriendFromPlayerPrefs(int i, Dictionary<string, int> friendIndexesDict)
		{
			if (PlayerPrefs.HasKey("FriendNo" + i))
			{
				PrefFriendData prefFriendData = new PrefFriendData
				{
					OrderNumber = i
				};
				int num = PlayerPrefs.GetInt("FriendID" + i, -1);
				if (num != -1)
				{
					PlayerPrefs.DeleteKey("FriendID" + i);
					PlayerPrefs.SetString("FriendID" + i, num.ToString());
				}
				prefFriendData.id = PlayerPrefs.GetString("FriendID" + i, "-1");
				prefFriendData.Name = PlayerPrefs.GetString("FriendName" + i, "");
				prefFriendData.accountID = PlayerPrefs.GetString("FriendAccountID" + i, "");
				friendIndexesDict.TryAdd(prefFriendData.id, Friends.Count);
				Friends.Add(prefFriendData);
				PrefsFriends.Add(new FriendData
				{
					accountID = prefFriendData.accountID,
					playerName = prefFriendData.Name,
					userID = prefFriendData.id
				});
			}
		}

		public void RemoveFriendWithLocalID(string localID)
		{
			foreach (PrefFriendData friend in Friends)
			{
				if (!(localID == friend.id))
				{
					continue;
				}
				try
				{
					if (!string.IsNullOrWhiteSpace(friend.accountID))
					{
						OnAuthorizedFriendDeleted?.Invoke(friend.accountID);
					}
				}
				catch (Exception message)
				{
					Debug.LogWarning(message);
				}
				PlayerPrefs.DeleteKey("FriendNo" + friend.OrderNumber);
				Friends.Remove(friend);
				RefillFriendsList();
				break;
			}
		}

		public void ClearAll()
		{
			PrefsFriends = new List<FriendData>();
			Friends = new List<PrefFriendData>();
			friendIndexesDict?.Clear();
		}

		public void RemoveFriendWithAccountID(string toRemove)
		{
			foreach (PrefFriendData friend in Friends)
			{
				if (toRemove == friend.accountID)
				{
					int orderNumber = friend.OrderNumber;
					PlayerPrefs.DeleteKey("FriendNo" + orderNumber);
					PlayerPrefs.DeleteKey("FriendID" + orderNumber);
					PlayerPrefs.DeleteKey("FriendName" + orderNumber);
					PlayerPrefs.DeleteKey("FriendMail" + orderNumber);
					PlayerPrefs.DeleteKey("FriendAccountID" + orderNumber);
					Friends.Remove(friend);
					RefillFriendsList();
					break;
				}
			}
		}

		public bool CanAddFriend()
		{
			if (Friends.Count >= 120)
			{
				return false;
			}
			return true;
		}

		public bool HasFriendWithID(string IDCheck)
		{
			if (Friends != null)
			{
				foreach (PrefFriendData friend in Friends)
				{
					if (IDCheck == friend.id)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void UpdateFriendName(string id, string accountID, string NewName)
		{
			if (Friends != null && HasFriendWithID(id))
			{
				PrefFriendData friendByID = GetFriendByID(id);
				PlayerPrefs.SetString("FriendName" + friendByID.OrderNumber, NewName);
				if (!PlayerPrefs.HasKey("FriendAccountID" + friendByID.OrderNumber))
				{
					PlayerPrefs.SetString("FriendAccountID" + friendByID.OrderNumber, accountID);
				}
			}
		}

		public void UpdateFriendAccountID(string id, string accountID)
		{
			if (Friends != null && HasFriendWithID(id))
			{
				PrefFriendData friendByID = GetFriendByID(id);
				PlayerPrefs.SetString("FriendAccountID" + friendByID.OrderNumber, accountID);
			}
		}

		public void AddFriend(string localID, string name, string accountID, bool fromRoom = false)
		{
			if (Friends == null || HasFriendWithID(localID))
			{
				return;
			}
			for (int i = 0; i <= 120; i++)
			{
				if (PlayerPrefs.HasKey("FriendNo" + i))
				{
					continue;
				}
				PrefFriendData prefFriendData = new PrefFriendData();
				PlayerPrefs.SetInt("FriendNo" + i, 1);
				prefFriendData.OrderNumber = i;
				PlayerPrefs.SetString("FriendID" + i, localID);
				prefFriendData.id = localID;
				PlayerPrefs.SetString("FriendName" + i, name);
				prefFriendData.Name = name;
				PlayerPrefs.SetString("FriendAccountID" + i, accountID);
				prefFriendData.accountID = accountID;
				FriendData obj = new FriendData(localID, accountID, name);
				if (fromRoom)
				{
					if (!string.IsNullOrEmpty(accountID.Trim()))
					{
						OnAuthorizedFriendAdded?.Invoke(obj);
					}
					else
					{
						OnPrefFriendAdded?.Invoke(obj);
					}
				}
				Friends.Add(prefFriendData);
				break;
			}
		}

		public void AddFriend(PrefFriendData frientData)
		{
			if (Friends != null)
			{
				AddFriend(frientData.id, frientData.Name, frientData.accountID);
			}
		}

		public void DeleteAllFriends()
		{
			for (int i = 0; i <= 120; i++)
			{
				PlayerPrefs.DeleteKey("FriendNo" + i);
				PlayerPrefs.DeleteKey("FriendMail" + i);
				PlayerPrefs.DeleteKey("FriendID" + i);
				PlayerPrefs.DeleteKey("FriendAccountID" + i);
				PlayerPrefs.DeleteKey("FriendName" + i);
			}
			RefillFriendsList();
		}

		public PrefFriendData GetFriendByID(string idFriend)
		{
			foreach (PrefFriendData friend in Friends)
			{
				if (friend.id == idFriend)
				{
					return friend;
				}
			}
			return null;
		}
	}

	public class VinylsDataUtility
	{
		private VinylDrawer _vinylDrawer;

		[Inject]
		public VinylsDataUtility(VinylDrawer vinylDrawer)
		{
			_vinylDrawer = vinylDrawer;
		}

		public void LoadAllVinyls(CarInfo carInfo, VinylsType type, bool isFrontPlate = false)
		{
			if (!TryGetVinylsFile(carInfo, type, out var file, isFrontPlate))
			{
				return;
			}
			string[] keys = file.GetKeys();
			foreach (string vinylKey in keys)
			{
				try
				{
					LoadVinyl(vinylKey, file, type);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
		}

		public void LoadAllVinyls(VynilOneCar oneVynil, VinylsType type)
		{
			if (oneVynil?.allVynils == null)
			{
				return;
			}
			foreach (VinylData allVynil in oneVynil.allVynils)
			{
				CreateVinyl(allVynil, type);
			}
		}

		public void LoadAllVinyls(ES3File file, VinylsType type, GameObject[] vinyls)
		{
			int num = 0;
			string[] array = (from x in file.GetKeys()
				orderby int.Parse(x, CultureInfo.InvariantCulture)
				select x).ToArray();
			foreach (string vinylKey in array)
			{
				try
				{
					if (num < vinyls.Length)
					{
						vinyls[num++] = LoadVinyl(vinylKey, file, type);
					}
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
		}

		private bool TryGetVinylsFile(CarInfo carInfo, VinylsType type, out ES3File file, bool isFrontPlate)
		{
			int num = ((type == VinylsType.PlateVinyls) ? PlateDataUtility.TryGetPlateIdForCar(carInfo.CarInstanceId, isFrontPlate) : 0);
			if (EasySaveDataHandler.TryGetFile(type switch
			{
				VinylsType.CarVinyls => DirectoryHelper.GetCarVinylsPath(carInfo.CarInstanceId), 
				VinylsType.PlateVinyls => DirectoryHelper.GetPlateVinylsPath(num.ToString()), 
				VinylsType.WindowVinyls => DirectoryHelper.GetWindowVinylsPath(carInfo.CarInstanceId), 
				_ => throw new UnityException("Vinyls for others not implemented"), 
			}, out file))
			{
				return file.GetKeys().Length != 0;
			}
			return false;
		}

		private GameObject LoadVinyl(string vinylKey, ES3File file, VinylsType type = VinylsType.CarVinyls)
		{
			VinylData vinylData = file.Load<VinylData>(vinylKey);
			if (vinylData == null)
			{
				return null;
			}
			return CreateVinyl(vinylData, type);
		}

		public GameObject CreateVinyl(VinylData vinylData, VinylsType type)
		{
			if (V.AllVinyls == null)
			{
				return null;
			}
			GameObject gameObject = _vinylDrawer.CreateVinyl(type);
			ApplyVinylData(vinylData, gameObject);
			return gameObject;
		}

		public void ApplyVinylData(VinylData vinylData, GameObject vinyl)
		{
			BitMaskPacker.Unpack(vinylData.packedData, out var isText, out var italic, out var bold, out var flipX, out var fontIndex, out var stickerIndex, out var _, out var _);
			Color color = BitMaskColorPacker.UnpackColor(vinylData.color);
			int num = Convert.ToInt32(vinylData.position.z);
			Transform transform = vinyl.transform;
			TextMesh component = transform.GetChild(0).GetComponent<TextMesh>();
			SpriteRenderer component2 = vinyl.GetComponent<SpriteRenderer>();
			transform.localPosition = new Vector3(vinylData.position.x, vinylData.position.y, -0.01f * (float)num);
			transform.localScale = new Vector3(vinylData.scaleRotation.x, vinylData.scaleRotation.y, 1f);
			transform.localEulerAngles = new Vector3(0f, 0f, vinylData.scaleRotation.z);
			component2.flipX = flipX;
			if (isText)
			{
				component.gameObject.SetActive(value: true);
				component.text = vinylData.text;
				if (fontIndex >= V.Fonts.Length)
				{
					fontIndex = 0;
				}
				Font font = V.Fonts[fontIndex];
				component.font = font;
				component.GetComponent<Renderer>().sharedMaterial = V.FontMaterials[fontIndex];
				ItalicOrBold(italic, bold, component);
				component2.enabled = false;
				AdjustTextScaleForFlip(component, flipX);
			}
			else
			{
				if (stickerIndex >= V.AllVinyls.Length || stickerIndex < 0 || stickerIndex == 1)
				{
					stickerIndex = 0;
				}
				component2.sprite = V.AllVinyls[stickerIndex].sprite;
				component2.enabled = true;
				component.gameObject.SetActive(value: false);
			}
			Color color2 = (component2.color = color);
			component.color = color2;
		}

		private void AdjustTextScaleForFlip(TextMesh textMesh, bool flipX)
		{
			Vector3 localScale = textMesh.gameObject.transform.localScale;
			int num = ((!flipX) ? 1 : (-1));
			localScale.x *= num;
			textMesh.gameObject.transform.localScale = localScale;
		}

		public static void TrySaveCarVinyls(List<VinylData> carBodyVinyls, string getCarVinylsPath)
		{
			if (carBodyVinyls != null && carBodyVinyls.Count > 0)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				carBodyVinyls = carBodyVinyls.OrderBy((VinylData vinyl) => vinyl.position.z).ToList();
				for (int num = 0; num < carBodyVinyls.Count; num++)
				{
					VinylData value = carBodyVinyls[num];
					dictionary.Add(num.ToString(CultureInfo.InvariantCulture), value);
				}
				if (dictionary.Count > 0)
				{
					EasySaveDataHandler.SaveValues(dictionary, getCarVinylsPath);
				}
			}
		}
	}

	private static PlayerDatas _instance;

	public static Texture2D poitnTexture;

	public static Texture2D car2Texture;

	public static FriendsUtility friendsUtility;

	public static int countToUnload = 0;

	public static int[] NotForTowCars;

	private static readonly Vector3 TowingCarRotation = new Vector3(-3f, 0f, 0f);

	private static IPrefabFactory _prefabFactory;

	private static PlayerDataStorage _playerDataStorage;

	private static IChatFilterUtility _chatFilterUtility;

	private const ulong IncrementalGcSliceNanoseconds = 3000000uL;

	public static ParticleSystem SharedSkidSmoke { get; private set; }

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, IChatFilterUtility chatFilterUtility)
	{
		_chatFilterUtility = chatFilterUtility;
		_playerDataStorage = playerDataStorage;
	}

	public void SetPrefabFactory(IPrefabFactory factory)
	{
		_prefabFactory = factory;
	}

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
		NotForTowCars = new int[2] { 70, 126 };
		Application.lowMemory += ApplicationOnLowMemory;
	}

	private void OnDestroy()
	{
		Application.lowMemory -= ApplicationOnLowMemory;
	}

	private void ApplicationOnLowMemory()
	{
		Debug.LogError("On low memory. Unload unused assets");
		UnloadResourcesForce();
	}

	public static LevelDataScript GetNextSceneData(int currentId, LevelsConfig levelsConfig, AllCarsInfo allCarsInfo = null)
	{
		int num = -1;
		for (int i = 0; i < levelsConfig.LevelsDatas.Length; i++)
		{
			if (levelsConfig.LevelsDatas[i].ID != currentId || i == levelsConfig.LevelsDatas.Length - 1)
			{
				continue;
			}
			for (int j = i + 1; j < levelsConfig.LevelsDatas.Length; j++)
			{
				LevelDataScript levelDataScript = levelsConfig.LevelsDatas[j];
				if (levelsConfig.LevelCategories[levelDataScript.CategoryIndex].isActive && !IsLevelCarBanned(levelDataScript, allCarsInfo))
				{
					num = j;
					break;
				}
			}
			break;
		}
		if (num != -1)
		{
			return levelsConfig.LevelsDatas[num];
		}
		return null;
	}

	private static bool IsLevelCarBanned(LevelDataScript level, AllCarsInfo allCarsInfo)
	{
		if (allCarsInfo == null)
		{
			return false;
		}
		if (level.CarId > 0 && allCarsInfo.IsBannedCar(level.CarId))
		{
			return true;
		}
		if (level.RacingCarId > 0 && allCarsInfo.IsBannedCar(level.RacingCarId))
		{
			return true;
		}
		return false;
	}

	public static void SetOnlineCarBool(GameObject car, bool isOnline)
	{
		car.GetComponent<Powertrain>().OnLineCar = isOnline;
	}

	public static void LookAtCarAudience(GameObject target, List<GameObject> npcList)
	{
		foreach (GameObject npc in npcList)
		{
			if ((bool)npc && npc.transform.TryGetComponent<BipedIK>(out var component))
			{
				if ((bool)target)
				{
					component.solvers.lookAt.target = target.transform;
				}
				component.enabled = true;
				component.solvers.lookAt.IKPositionWeight = 1f;
			}
		}
	}

	public static async UniTask<GameObject> LoadTrailerFromResource(Vector3 pos, Vector3 euler, int indexTrailer)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>($"Trailer_{indexTrailer}");
		GameObject obj = await handle;
		GameObject result;
		try
		{
			result = _prefabFactory.Instantiate(obj, pos, Quaternion.Euler(euler));
		}
		catch
		{
			if (handle.IsValid())
			{
				handle.Release();
			}
			return null;
		}
		_ = handle.BindTo(result);
		return result;
	}

	public static void RecordBestReuslt(float best, LevelsProvider levelsProvider)
	{
		if (!(levelsProvider.CurrentLevelData != null))
		{
			return;
		}
		int iD = levelsProvider.CurrentLevelData.ID;
		float num = ObscuredPrefs.GetFloat($"bestResultID{iD}", best);
		if (best <= num)
		{
			if (best == 0f)
			{
				best = 1f;
			}
			ObscuredPrefs.SetFloat($"bestResultID{iD}", best);
		}
	}

	public static void SetValues()
	{
		if (!ScenesHelper.IsParkingLevelActive() && friendsUtility != null)
		{
			friendsUtility.RefillFriendsList();
		}
		CurrencyManagerController.Instance.UpdateLabelsEvents();
		V.Steering = PlayerPrefs.GetInt("steering");
		SetMirrorValueFromPrefs();
	}

	private static void SetMirrorValueFromPrefs()
	{
		FlagsStorage.ShowCarMirror = PlayerPrefs.GetInt("showCarMirror", 0) != 0;
	}

	public static bool IsBought(int i)
	{
		return PlayerPrefs.GetInt($"{i}_{V.CarId}") == 1;
	}

	public static void PriceSoldLogic(AdvancedInt moneyInput)
	{
		float num = ObscuredPrefs.GetFloat("priceSold");
		moneyInput.Get(out int value);
		if (num > 0f)
		{
			num += (float)value;
			if (num < 0f)
			{
				num = 0f;
			}
			ObscuredPrefs.SetFloat("priceSold", num);
		}
		if (num > 50000000f && ObscuredPrefs.GetInt("checkMonyCoin") == 1)
		{
			ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version, 1);
		}
	}

	public static void IgnoreCollisionWithOtherCars(GameObject car, bool ignore, OnlineCollectionsProvider onlineCollectionsProvider, out bool checkAgain)
	{
		checkAgain = false;
		if (!car)
		{
			checkAgain = ignore;
			return;
		}
		if (onlineCollectionsProvider.IsOnlineCarsEmpty())
		{
			checkAgain = ignore;
			return;
		}
		VehicleData component = car.GetComponent<VehicleData>();
		Collider collider = (component ? component.Collider : null);
		if (!collider)
		{
			checkAgain = ignore;
			return;
		}
		MultiPlayerPosControl[] carsOnlineNow = onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if (!multiPlayerPosControl || multiPlayerPosControl.gameObject == car)
			{
				continue;
			}
			if (multiPlayerPosControl.TryGetComponent<VehicleData>(out var component2))
			{
				Collider collider2 = component2.Collider;
				if ((bool)collider && (bool)collider2)
				{
					if (ignore)
					{
						CarColliders.SetIgnore(component, component2, ignore: true);
						continue;
					}
					Powertrain component3 = multiPlayerPosControl.GetComponent<Powertrain>();
					if (!component3)
					{
						checkAgain = true;
					}
					else if (((multiPlayerPosControl.gameObject != V.Car) ? component3.SpeedMpsOnline : component3.SpeedKmh) < 40f && component2.СurrentSellCarTrigger == null)
					{
						if (!collider.bounds.Intersects(collider2.bounds))
						{
							CarColliders.SetIgnore(component, component2, ignore: false);
						}
						else
						{
							checkAgain = true;
						}
					}
					else
					{
						CarColliders.SetIgnore(component, component2, ignore: true);
						checkAgain = true;
					}
				}
				else
				{
					checkAgain = true;
				}
			}
			else
			{
				checkAgain = true;
			}
		}
		Physics.IgnoreLayerCollision(17, 31, ignore: false);
	}

	public static void SetColor(string name, Color color)
	{
		PlayerPrefs.SetFloat(name + ".r", color.r);
		PlayerPrefs.SetFloat(name + ".g", color.g);
		PlayerPrefs.SetFloat(name + ".b", color.b);
		PlayerPrefs.SetFloat(name + ".a", color.a);
	}

	public static Color GetColor(string name, Color defaultValue)
	{
		Color result = new Color(0f, 0f, 0f, 0f);
		result.r = PlayerPrefs.GetFloat(name + ".r", defaultValue.r);
		result.g = PlayerPrefs.GetFloat(name + ".g", defaultValue.g);
		result.b = PlayerPrefs.GetFloat(name + ".b", defaultValue.b);
		result.a = PlayerPrefs.GetFloat(name + ".a", defaultValue.a);
		return result;
	}

	public static void DeleteColor(string name)
	{
		PlayerPrefs.GetFloat(name + ".r");
		PlayerPrefs.GetFloat(name + ".g");
		PlayerPrefs.GetFloat(name + ".b");
		PlayerPrefs.GetFloat(name + ".a");
	}

	public static void ItalicOrBold(bool italic, bool bold, TextMesh textMesh)
	{
		FontStyle fontStyle = (italic ? ((!bold) ? FontStyle.Italic : FontStyle.BoldAndItalic) : (bold ? FontStyle.Bold : FontStyle.Normal));
		textMesh.fontStyle = fontStyle;
	}

	public static void ItalicOrBold(int indexFontStyle, TextMesh textMesh)
	{
		textMesh.fontStyle = indexFontStyle switch
		{
			0 => FontStyle.Normal, 
			1 => FontStyle.Italic, 
			2 => FontStyle.Bold, 
			_ => FontStyle.BoldAndItalic, 
		};
	}

	public static void SetVector3(string name, Vector3 vector)
	{
		PlayerPrefs.SetFloat(name + ".X", vector.x);
		PlayerPrefs.SetFloat(name + ".Y", vector.y);
		PlayerPrefs.SetFloat(name + ".Z", vector.z);
	}

	public static Vector3 GetVector3(string name, Vector3 defaultValue)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		result.x = PlayerPrefs.GetFloat(name + ".X", defaultValue.x);
		result.y = PlayerPrefs.GetFloat(name + ".Y", defaultValue.y);
		result.z = PlayerPrefs.GetFloat(name + ".Z", defaultValue.z);
		return result;
	}

	public static void DeletePlayerPrefsVector3(string name)
	{
		PlayerPrefs.DeleteKey(name + ".X");
		PlayerPrefs.DeleteKey(name + ".Y");
		PlayerPrefs.DeleteKey(name + ".Z");
	}

	public static void DisableEnableAll(bool isEnable, GameObject[] objects)
	{
		foreach (GameObject gameObject in objects)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(isEnable);
			}
		}
	}

	public static Vector3[] FromTransformToVector3(Transform[] toConvert)
	{
		Vector3[] array = new Vector3[toConvert.Length];
		int num = 0;
		foreach (Transform transform in toConvert)
		{
			array[num] = transform.position;
			num++;
		}
		return array;
	}

	public static void ClearCarData(int id)
	{
		if (ObscuredPrefs.GetInt("LastCarId") == id)
		{
			ObscuredPrefs.DeleteKey("LastCarId");
		}
		ObscuredPrefs.DeleteKey("car" + id);
		ObscuredPrefs.DeleteKey("carID" + id);
		ObscuredPrefs.DeleteKey("maxTorqueRPM" + id);
		ObscuredPrefs.DeleteKey("maxPower" + id);
		ObscuredPrefs.DeleteKey("maxPowerRpm" + id);
		ObscuredPrefs.DeleteKey("maxRpm" + id);
		ObscuredPrefs.DeleteKey("maxTorque" + id);
		ObscuredPrefs.DeleteKey("FforwardGripFactor" + id);
		ObscuredPrefs.DeleteKey("GforwardGripFactor" + id);
		ObscuredPrefs.DeleteKey("brake" + id);
		ObscuredPrefs.DeleteKey("hasTurbo" + id);
		ObscuredPrefs.DeleteKey("shiftTime" + id);
		ObscuredPrefs.DeleteKey("Cd" + id);
		ObscuredPrefs.DeleteKey("wheelToSet" + id);
		ObscuredPrefs.DeleteKey("FsuspensionTravel" + id);
		ObscuredPrefs.DeleteKey("RsuspensionTravel" + id);
		ObscuredPrefs.DeleteKey("FsuspensionRate" + id);
		ObscuredPrefs.DeleteKey("RsuspensionRate" + id);
		ObscuredPrefs.DeleteKey("Fcamber" + id);
		ObscuredPrefs.DeleteKey("Rcamber" + id);
		ObscuredPrefs.DeleteKey("COGx" + id);
		ObscuredPrefs.DeleteKey("COGy" + id);
		DeleteColor("WheelColorReflective" + id);
		DeleteColor("Car" + id + "W");
		DeleteColor("CarColor" + id);
		DeleteColor("CarColorReflective" + id);
		DeleteColor("CarColorWindow" + id);
		DeleteColor("WindowSpecColor" + id);
		ObscuredPrefs.DeleteKey("CarPoliceSet" + id);
		ObscuredPrefs.DeleteKey("CarPoliceBought" + id);
		ObscuredPrefs.DeleteKey("burble" + id);
		ObscuredPrefs.DeleteKey("turboShot" + id);
		ObscuredPrefs.DeleteKey("mileage" + id);
		for (int i = 0; i < 9; i++)
		{
			ObscuredPrefs.DeleteKey("type" + i + "_" + id);
			for (int j = 0; j < 9; j++)
			{
				ObscuredPrefs.DeleteKey("type" + i + "_has_" + j + "_" + id);
			}
		}
		for (int k = 0; k < 300; k++)
		{
			PlayerPrefs.DeleteKey("vinyl" + k + "_" + id);
			PlayerPrefs.DeleteKey(k + "_" + id);
			PlayerPrefs.DeleteKey(k + "GearRatio" + id);
			PlayerPrefs.DeleteKey("position_" + k + "_" + id);
			PlayerPrefs.DeleteKey("euler_" + k + "_" + id);
			PlayerPrefs.DeleteKey("scale_" + k + "_" + id);
			PlayerPrefs.DeleteKey("color_" + k + "_" + id);
			PlayerPrefs.DeleteKey("stickerIndex_" + k + "_" + id);
			PlayerPrefs.DeleteKey("HasSticker_" + k + "_" + id);
			PlayerPrefs.DeleteKey("isText_" + k + "_" + id);
		}
		PlayerPrefs.DeleteKey($"New_vinyls_{id}");
	}

	public static int GetRowNumber(int currentIndex)
	{
		if (currentIndex < 839)
		{
			if (currentIndex >= 225)
			{
				if (currentIndex < 263)
				{
					return 6;
				}
				return 8;
			}
			if (currentIndex > 0)
			{
				return 2;
			}
		}
		else
		{
			if (currentIndex < 1044)
			{
				if (currentIndex < 934)
				{
					return 4;
				}
				return 2;
			}
			if (currentIndex < 1152)
			{
				return 4;
			}
		}
		return 2;
	}

	public static int GetRoadDirection(int row)
	{
		if (row >= 8)
		{
			if (row > 9)
			{
				return -1;
			}
			return 1;
		}
		if (row > 3)
		{
			return -1;
		}
		return 1;
	}

	public static int GetRandomRow(int currentIndex)
	{
		int num = 5;
		return GetRowNumber(currentIndex) switch
		{
			2 => UnityEngine.Random.Range(4, 6), 
			4 => UnityEngine.Random.Range(3, 7), 
			6 => UnityEngine.Random.Range(2, 8), 
			8 => UnityEngine.Random.Range(1, 9), 
			_ => num, 
		} - 1;
	}

	public static int getNextRowIndex(int currentIndex, int currentRow)
	{
		int num = currentRow;
		if (GetRowNumber(currentIndex) > 3)
		{
			num = currentRow switch
			{
				3 => 2, 
				2 => 3, 
				4 => 5, 
				5 => 4, 
				_ => num, 
			};
		}
		return num;
	}

	public static int getEqualTargetIndex(int currentIndex, int row)
	{
		if (row == 3 || row == 4 || row > 7)
		{
			return currentIndex;
		}
		int result = 0;
		switch (row)
		{
		case 0:
		case 7:
			result = currentIndex - 262;
			break;
		case 1:
		case 6:
			result = currentIndex - 224;
			break;
		case 2:
		case 5:
			if (currentIndex < 932)
			{
				result = currentIndex - 224;
			}
			else if (currentIndex > 1041)
			{
				result = currentIndex - 332;
			}
			break;
		}
		return result;
	}

	public static int getEqualTargetIndexToMainRows(int currentIndex, int row)
	{
		int result = currentIndex;
		if (row == 2 || row == 5)
		{
			if (currentIndex < 709)
			{
				result = currentIndex + 224;
			}
			else if (currentIndex > 709)
			{
				result = currentIndex + 331;
			}
		}
		return result;
	}

	public static AsyncOperation LoadMainCityScene()
	{
		return LoadEnvironmentSceneAsync(14);
	}

	public static AsyncOperation LoadMountainScene()
	{
		return LoadEnvironmentSceneAsync(16);
	}

	public static AsyncOperation LoadRoad2Scene()
	{
		return LoadEnvironmentSceneAsync(18);
	}

	public static AsyncOperation LoadDesertScene()
	{
		return LoadEnvironmentSceneAsync(15);
	}

	private static AsyncOperation LoadEnvironmentSceneAsync(int buildIndex)
	{
		if (SceneManager.GetSceneByBuildIndex(buildIndex).IsValid())
		{
			return null;
		}
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
		if (asyncOperation == null)
		{
			return null;
		}
		SceneLoadTracker.Begin();
		LoadingPriorityBoost.Acquire();
		asyncOperation.completed += (AsyncOperation _) =>
		{
			LoadingPriorityBoost.Release();
			SceneLoadTracker.End();
		};
		return asyncOperation;
	}

	public static void TryUnloadResource()
	{
		V.UnloadResourceCounter++;
		int num = 10;
		if (V.UnloadResourceCounter > num)
		{
			UnloadResourcesForce();
			V.UnloadResourceCounter = 0;
		}
	}

	public static void UnloadResourcesForce()
	{
		AsyncOperation unload = UnloadUnusedAssets("UnloadResourcesForce");
		if (unload != null)
		{
			unload.completed += AOncompleted;
		}
		void AOncompleted(AsyncOperation obj)
		{
			unload.completed -= AOncompleted;
			CollectGarbage();
		}
	}

	public static AsyncOperation UnloadUnusedAssets(string reason = null)
	{
		if (SceneLoadTracker.HasPendingLoads)
		{
			return null;
		}
		return Resources.UnloadUnusedAssets();
	}

	public static void CollectGarbage(string reason = null)
	{
		if (!SceneLoadTracker.HasPendingLoads)
		{
			if (GarbageCollector.isIncremental)
			{
				GarbageCollector.CollectIncremental(3000000uL);
			}
			else
			{
				GC.Collect();
			}
		}
	}

	public static void TowCar(GameObject car, GameObject LoadCar)
	{
		Powertrain component = LoadCar.GetComponent<Powertrain>();
		VehicleData component2 = car.GetComponent<VehicleData>();
		Vector3 vector = component2.GetPowertrain.TrailerJoint.localPosition;
		vector = new Vector3(vector.x, vector.y + component.GetWeightBounds.size.y * 0.5f, vector.z - 1f);
		component.TowCar = true;
		component.SetAlignPlane(default, usePlane: true);
		component2.TowingComposition.LoadCar = car;
		LoadCar.transform.parent = car.transform;
		LoadCar.transform.SetLocalPositionAndRotation(vector, Quaternion.Euler(TowingCarRotation));
		if (!LoadCar.transform.TryGetComponent<ConfigurableJoint>(out var component3))
		{
			component3 = LoadCar.gameObject.AddComponent<ConfigurableJoint>();
		}
		component3.connectedBody = car.GetComponent<Rigidbody>();
		component3.enableCollision = true;
		component3.configuredInWorldSpace = false;
		component3.enablePreprocessing = false;
		component3.breakForce = float.PositiveInfinity;
		component3.breakTorque = float.PositiveInfinity;
		component3.axis = Vector3.right;
		component3.secondaryAxis = Vector3.up;
		component3.autoConfigureConnectedAnchor = false;
		component3.anchor = new Vector3(0f, component2.GetPowertrain.TrailerJoint.localPosition.y, 0f);
		component3.connectedAnchor = vector;
		component3.massScale = 1f;
		component3.connectedMassScale = 1f / (float)component2.GetPowertrain.PhysSteps;
		component3.xMotion = ConfigurableJointMotion.Locked;
		component3.yMotion = ConfigurableJointMotion.Limited;
		component3.zMotion = ConfigurableJointMotion.Locked;
		component3.angularXMotion = ConfigurableJointMotion.Locked;
		component3.angularYMotion = ConfigurableJointMotion.Locked;
		component3.angularZMotion = ConfigurableJointMotion.Locked;
		JointDrive jointDrive = new JointDrive
		{
			positionSpring = 2000f,
			positionDamper = 300f,
			maximumForce = 20000f
		};
		JointDrive zDrive = (component3.yDrive = (component3.xDrive = jointDrive));
		component3.zDrive = zDrive;
		jointDrive = new JointDrive
		{
			positionSpring = 500f,
			positionDamper = 100f,
			maximumForce = 5000f
		};
		JointDrive angularYZDrive = (component3.angularXDrive = jointDrive);
		component3.angularYZDrive = angularYZDrive;
		SoftJointLimit linearLimit = new SoftJointLimit
		{
			limit = 0.15f
		};
		component3.linearLimit = linearLimit;
		SoftJointLimitSpring linearLimitSpring = new SoftJointLimitSpring
		{
			spring = 1000f,
			damper = 100f
		};
		component3.linearLimitSpring = linearLimitSpring;
		SoftJointLimit softJointLimit = new SoftJointLimit
		{
			limit = 5f
		};
		SoftJointLimit angularZLimit = (component3.angularYLimit = (component3.highAngularXLimit = (component3.lowAngularXLimit = softJointLimit)));
		component3.angularZLimit = angularZLimit;
		SoftJointLimitSpring softJointLimitSpring = new SoftJointLimitSpring
		{
			spring = 300f,
			damper = 50f
		};
		SoftJointLimitSpring angularYZLimitSpring = (component3.angularXLimitSpring = softJointLimitSpring);
		component3.angularYZLimitSpring = angularYZLimitSpring;
		if (FlagsStorage.IsFreeDriveMode)
		{
			LoadCar.transform.parent = V.GlobalPoint;
		}
	}

	public static bool CanTow(int CarID)
	{
		return NotForTowCars.All((int t) => t != CarID);
	}

	public static string CheckNameValid(string str)
	{
		string message = str.Where((char symbol) => !char.IsSurrogate(symbol)).Aggregate("", (string current, char symbol) => current + symbol);
		message = _chatFilterUtility?.Filter(message);
		if (message?.Length > (int)AccountSection.MaxPlayerNameLength)
		{
			message = message?.Substring(0, AccountSection.MaxPlayerNameLength);
		}
		return message;
	}

	public static string CheckCarModelText(string str)
	{
		string text = "";
		for (int i = 0; i < str.Length; i++)
		{
			char c = str[i];
			if (!char.IsSurrogate(c))
			{
				text += c;
			}
		}
		text = _chatFilterUtility.Filter(text);
		if (text.Length > (int)AccountSection.SellCarModelTextLength)
		{
			text = text.Substring(0, AccountSection.SellCarModelTextLength);
		}
		return text;
	}

	public static void SaveVynilsFromDataBase(VynilOneCar vynilOneCar)
	{
		if (vynilOneCar?.allVynils == null)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		try
		{
			foreach (VinylData allVynil in vynilOneCar.allVynils)
			{
				string key = Convert.ToInt32(allVynil.position.z).ToString();
				dictionary.Add(key, allVynil);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"car{vynilOneCar.CarID} vynils save error error: {arg}");
		}
		if (dictionary.Count > 0)
		{
			EasySaveDataHandler.SaveValues(dictionary, DirectoryHelper.GetCarVinylsPath(vynilOneCar.CarInstanceID));
		}
	}

	public static void SaveVynilsAsPreset(OneCarOld vynilOneCar)
	{
		if (vynilOneCar?.Vynils != null)
		{
			_ = vynilOneCar.Vynils.allVynils.Count;
		}
	}

	public static Color VectorToColor(Vector3 input)
	{
		return new Color(input.x, input.y, input.z);
	}

	public static void SetDiod(GameObject diod, Color color)
	{
	}

	public static void CreateSkid(GameObject car)
	{
		VehicleData component = car.GetComponent<VehicleData>();
		if ((bool)component.WheelComposition.SkidMark)
		{
			return;
		}
		Skidmarks orCreateSharedSkidmarks = GetOrCreateSharedSkidmarks();
		if (!orCreateSharedSkidmarks)
		{
			return;
		}
		component.WheelComposition.SkidMark = orCreateSharedSkidmarks.gameObject;
		ParticleSystem orCreateSharedSkidSmoke = GetOrCreateSharedSkidSmoke();
		component.WheelComposition.SkidSmoke = orCreateSharedSkidSmoke;
		if (!car.TryGetComponent<Axles>(out var component2))
		{
			return;
		}
		SetSmokeColor(component2, SmokeColorOf(component));
		foreach (WheelProvider allWheel in component2.AllWheels)
		{
			allWheel.InstantiateSkidmarks(orCreateSharedSkidmarks, orCreateSharedSkidSmoke);
		}
	}

	public static void SetSmokeColor(Axles axles, Color color)
	{
		if (!axles || axles.AllWheels == null)
		{
			return;
		}
		color.a = 1f;
		foreach (WheelProvider allWheel in axles.AllWheels)
		{
			if ((bool)allWheel)
			{
				allWheel.SkidSmokeColor = color;
			}
		}
	}

	private static Color SmokeColorOf(VehicleData vehicleData)
	{
		CarInfo carInfo = (vehicleData ? vehicleData.carInfo : null);
		if (carInfo?.Colors == null)
		{
			return Color.white;
		}
		return carInfo.Colors.SmokeColor;
	}

	private static ParticleSystem GetOrCreateSharedSkidSmoke()
	{
		if ((bool)SharedSkidSmoke)
		{
			return SharedSkidSmoke;
		}
		if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.skidSmoke)
		{
			return SharedSkidSmoke = ParkAtGlobalPoint(V.CoreGameManager.skidSmoke);
		}
		GameObject gameObject = (V.SkidSmoke ? V.SkidSmoke : Resources.Load<GameObject>("Other/SkidSmoke"));
		if (!gameObject)
		{
			return null;
		}
		return SharedSkidSmoke = ParkAtGlobalPoint(UnityEngine.Object.Instantiate(gameObject).GetComponent<ParticleSystem>());
	}

	private static ParticleSystem ParkAtGlobalPoint(ParticleSystem smoke)
	{
		if (!smoke)
		{
			return null;
		}
		Transform transform = (((bool)V.CoreGameManager && (bool)V.CoreGameManager.globalPoint) ? V.CoreGameManager.globalPoint.transform : null);
		if (!transform)
		{
			return smoke;
		}
		Transform transform2 = smoke.transform;
		if (transform2.parent != transform)
		{
			transform2.SetParent(transform, worldPositionStays: false);
		}
		return smoke;
	}

	private static Skidmarks GetOrCreateSharedSkidmarks()
	{
		for (int i = 0; i < Skidmarks.allSkidmarks.Count; i++)
		{
			if ((bool)Skidmarks.allSkidmarks[i])
			{
				return Skidmarks.allSkidmarks[i];
			}
		}
		GameObject gameObject = Resources.Load<GameObject>("Other/Skidmarks");
		if (!gameObject)
		{
			return null;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.globalPoint)
		{
			gameObject2.transform.parent = V.CoreGameManager.globalPoint.transform;
		}
		gameObject2.transform.localPosition = Vector3.zero;
		return gameObject2.GetComponent<Skidmarks>();
	}

	public void Init()
	{
		NotForTowCars = new int[2] { 70, 126 };
	}

	public static string CreateNewCarID(int carToDelete)
	{
		return "123";
	}

	public static void addDeletedCarID(string carGeneratedID)
	{
		if (carGeneratedID == "" || hasInDeletedList(carGeneratedID))
		{
			return;
		}
		for (int i = 0; i < 50; i++)
		{
			if (!ObscuredPrefs.HasKey("deletedGeneratedCarID" + i))
			{
				ObscuredPrefs.SetString("deletedGeneratedCarID" + i, carGeneratedID);
				return;
			}
		}
		for (int j = 0; j < 50; j++)
		{
			ObscuredPrefs.DeleteKey("deletedGeneratedCarID" + j);
		}
		ObscuredPrefs.SetString("deletedGeneratedCarID" + 0, carGeneratedID);
	}

	public static bool hasInDeletedList(string carGeneratedID)
	{
		for (int i = 0; i < 50; i++)
		{
			if (ObscuredPrefs.GetString("deletedGeneratedCarID" + i) == carGeneratedID)
			{
				return true;
			}
		}
		return false;
	}
}
