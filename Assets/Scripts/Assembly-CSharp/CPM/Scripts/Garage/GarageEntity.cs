using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Database;
using CPM.Scripts.Garage.FreeDriverService;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Racing.CameraManagement;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking;
using _Car_Parking.Garage.ForbiddenZones;
using _Car_Parking.Garage.Poses;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;

namespace CPM.Scripts.Garage
{
	public abstract class GarageEntity : IGarageEntity
	{
		private const float CameraRenderMinDistance = 105f;

		private const int MaxResolveIterations = 16;

		private const int WheelsOnGroundToSave = 4;

		private const float ShrinkFactor = 0.9f;

		private const float MaxSavedY = 5f;

		private const float MinSavedY = 0f;

		private const float MinCarYPosition = -400f;

		protected GarageController _controller;

		protected readonly Dictionary<int, VehicleData> _spawnedCars = new Dictionary<int, VehicleData>();

		protected PlayerDatas.CarLoadingUtility CarLoadingUtility;

		protected ISlotService SlotService;

		protected GarageSpotCollection GarageSpots;

		protected ScreenFader ScreenFader;

		protected IStorageService StorageService;

		protected OptionalDependency<ManagerUI> ManagerUI;

		protected OptionalDependency<CameraManager> CameraManager;

		protected OptionalDependency<ICarTuningFactory> CarTuningFactory;

		protected OptionalDependency<IFreeDriverService> _freeDriverService;

		protected InputItemsProvider ControlItemsProvider;

		protected AnalyticEventFactory _analyticEventFactory;

		private CarInputManager _carInputManager;

		private IGarageCarPoseService _garagePoseService;

		private IGarageSelectorService _garageSelectorService;

		private IForbiddenZonesProvider _forbiddenZonesProvider;

		private AssetBundleManager _assetBundleManager;

		private ICarInfoService _carInfoService;

		private AllCarsInfo _allCarsInfo;

		private RainSoundState _rainSoundState;

		protected SoundControllerExtension CarInOutSound;

		private const int FallbackCarId = 132;

		private static readonly Color FallbackCarColor = new Color(1f, 0.31f, 0.65f);

		private readonly Queue<VehicleData> _pendingSettle = new Queue<VehicleData>();

		private bool _settlePumpRunning;

		[UnityEngine.Scripting.Preserve]
		[Inject]
		public void Construct(PlayerDatas.CarLoadingUtility loadingUtility, ISlotService slotService, IStorageService storageService, InputItemsProvider controlItemsProvider, ScreenFader screenFader, CarInputManager carInputManager, IGarageCarPoseService garagePoseService, IGarageSelectorService garageSelectorService, IForbiddenZonesProvider zonesProvider, AnalyticEventFactory analyticEventFactory, AssetBundleManager assetBundleManager, ICarInfoService carInfoService, AllCarsInfo allCarsInfo, OptionalDependency<ManagerUI> managerUI, OptionalDependency<CameraManager> cameraManager, OptionalDependency<ICarTuningFactory> carTuningFactory, OptionalDependency<IFreeDriverService> freeDriverService, RainSoundState rainSoundState, SoundControllerExtension carInOutSound)
		{
			ControlItemsProvider = controlItemsProvider;
			StorageService = storageService;
			ScreenFader = screenFader;
			CameraManager = cameraManager;
			SlotService = slotService;
			CarLoadingUtility = loadingUtility;
			ManagerUI = managerUI;
			CarTuningFactory = carTuningFactory;
			_freeDriverService = freeDriverService;
			_carInputManager = carInputManager;
			_garagePoseService = garagePoseService;
			_garageSelectorService = garageSelectorService;
			_forbiddenZonesProvider = zonesProvider;
			_analyticEventFactory = analyticEventFactory;
			_assetBundleManager = assetBundleManager;
			_carInfoService = carInfoService;
			_rainSoundState = rainSoundState;
			CarInOutSound = carInOutSound;
			_allCarsInfo = allCarsInfo;
		}

		private bool TryReleaseStaleSlot(int slotId)
		{
			if (!_allCarsInfo)
			{
				return false;
			}
			if (!_carInfoService.TryGetFromSlot(slotId, out var carInfo) || carInfo == null)
			{
				return false;
			}
			if (!_allCarsInfo.IsCarPreloadCached(carInfo.CarID))
			{
				return false;
			}
			CarSlot slot = SlotService.GetSlot(slotId);
			if (slot == null)
			{
				return false;
			}
			_carInfoService.DeleteFromSlot(slot);
			PlayerPrefs.SetInt("ForceSaveSlots", 1);
			EnsureGarageNotEmpty();
			return true;
		}

		private void EnsureGarageNotEmpty()
		{
			if (SlotService.HaveCarInAnyGarage)
			{
				return;
			}
			List<CarSlot> allSlotsInfos = SlotService.AllSlotsInfos;
			CarSlot carSlot = ((allSlotsInfos != null && allSlotsInfos.Count > 0) ? SlotService.AllSlotsInfos[0] : null);
			if (carSlot != null)
			{
				if ((bool)V.CoreGameManager)
				{
					V.CoreGameManager.AddNewCarToSlot(carSlot, 132, null, PaintFallbackCar);
					return;
				}
				CarInfo carInfo = _carInfoService.CreateDefault(132);
				PaintFallbackCar(carInfo);
				SlotService.SetSlot(carSlot.ID, carInfo.CarInstanceId);
			}
		}

		private static void PaintFallbackCar(CarInfo carInfo)
		{
			if (carInfo?.Colors != null)
			{
				carInfo.Colors.CarColor = new CarPaintColor(FallbackCarColor);
				carInfo.Save();
			}
		}

		public virtual async UniTask Init(VehicleData enteredVehicle, GarageController controller, CancellationToken cancellationToken)
		{
			RainWetnessDriver.ClearForGarage();
			_rainSoundState.Reset();
			CPMGraphicsSettingsManager.Instance.RenderDistanceController.ClearSettings();
			CPMGraphicsSettingsManager.Instance.RenderDistanceController.SetSettings(GraphicsDistanceController.PriorityType.Garage, 105f, GraphicsDistanceController.SetType.SetIfLarger);
			CPMGraphicsSettingsManager.Instance.RenderPropsDistanceController.ClearSettings();
			GarageSpots = StorageService.Data.GarageSpotCollection;
			_controller = controller;
			InitVSlotId();
			CarSlot slot = SlotService.GetSlot(V.SlotID);
			if (_assetBundleManager != null)
			{
				_assetBundleManager.CurrentGarage = this;
				_assetBundleManager.CarInfoService = _carInfoService;
				_assetBundleManager.CarSlotsService = SlotService;
				_assetBundleManager.UpdateInteriorsAssetsData();
			}
			if (enteredVehicle == null && slot != null && !string.IsNullOrEmpty(slot.CarFilePath) && SlotService.SlotCollection.Index == SlotService.GetSlotCollectionBySlotId(V.SlotID).Index && !TryReleaseStaleSlot(V.SlotID))
			{
				int spotId = GarageSpots.GetSpotBySlotIdOrEmpty(V.SlotID, SlotService);
				GarageSpot garageSpot = _controller.GetGarageSpot(spotId);
				Transform spawnTransform = garageSpot.spawnTransform;
				garageSpot.isOccupied = true;
				CarInfo carInfo;
				(carInfo, V.Car) = await CarLoadingUtility.LoadCarFromSlot(V.SlotID, spawnTransform.position, spawnTransform.rotation, tryUnloadCurrent: false, waitBodyLoad: true, FlagsStorage.IsMultiFree, resetPos: false);
				await UniTask.Yield();
				if (!V.Car || cancellationToken.IsCancellationRequested)
				{
					return;
				}
				V.LastUsedCar = V.Car;
				V.CarId = carInfo.CarID;
				AnalyticsHelper.SaveLastCarId(V.CarId);
				if (V.Car.TryGetComponent<VehicleData>(out enteredVehicle))
				{
					enteredVehicle.InteriorsComposition.TryRestoreSavedInterior();
					enteredVehicle.CarMirrorController?.SetMirror(FlagsStorage.ShowCarMirror);
				}
				if (!V.DriverPerson)
				{
					V.CoreGameManager.SetupCarDriver(enteredVehicle.gameObject);
				}
				PlaceCarOnSpot(spotId, enteredVehicle);
				FocusOnCarOnSpot(spotId);
				V.Car = enteredVehicle.gameObject;
			}
			if (enteredVehicle != null)
			{
				V.LastUsedCar = enteredVehicle.gameObject;
			}
			LoadOtherGarageCars().Forget();
		}

		public Dictionary<int, VehicleData> GetSpawnedCars()
		{
			return _spawnedCars;
		}

		protected virtual void InitVSlotId()
		{
			V.SlotID = SlotService.SlotCollection.LastSlotId;
			if (SlotService.IsSlotEmpty(V.SlotID) || (int)V.SlotID == -1)
			{
				V.SlotID = SlotService.GetNotEmptySlotId();
				SlotService.GetSlotCollectionBySlotId(V.SlotID).LastSlotId = V.SlotID;
			}
		}

		public void UpdateCarEnterSubscriptions(bool toSubscribe)
		{
			if (toSubscribe)
			{
				V.OnCarEntered += EnterCar;
				V.OnCarExit += ExitCar;
			}
			else
			{
				V.OnCarEntered -= EnterCar;
				V.OnCarExit -= ExitCar;
			}
		}

		private bool TryApplySavedPose(VehicleData vehicleData, int carSlotId)
		{
			string text = SlotService.GetSlot(carSlotId)?.CarFilePath;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			int currentGarageIndex = _garageSelectorService.CurrentGarageIndex;
			if (!_garagePoseService.TryGetPose(currentGarageIndex, text, out var pose))
			{
				return false;
			}
			if (!IsValidPosition(pose.Position))
			{
				_garagePoseService.RemovePose(currentGarageIndex, text);
				return false;
			}
			vehicleData.transform.SetPositionAndRotation(pose.Position, Quaternion.Euler(0f, pose.Yaw, 0f));
			SetSteerAndWheels(vehicleData, pose);
			return true;
		}

		private void SetSteerAndWheels(VehicleData vehicleData, SavedPoseDto savedPosition)
		{
			float num = Mathf.Clamp(savedPosition.SteerNorm, -1f, 1f);
			if (_carInputManager.GetPowertrain == vehicleData.GetPowertrain)
			{
				_carInputManager.SetSteerPositionInstantly(num);
			}
			vehicleData.GetPowertrain.SetSteer(num, useAssistent: false);
		}

		private bool IsValidPosition(Vector3 carPosition)
		{
			if (!(carPosition.y < 0f))
			{
				return !(carPosition.y > 5f);
			}
			return false;
		}

		public async UniTask LoadCarBySpotId(int spotId, bool focusOnCar)
		{
			int num = GarageSpots[spotId];
			if (num == -1)
			{
				return;
			}
			CarSlot slot = SlotService.GetSlot(num);
			if (slot == null || slot.IsEmpty)
			{
				return;
			}
			GarageSpot spot = _controller.GetGarageSpot(spotId);
			spot.isOccupied = true;
			try
			{
				await LoadCarBySpotIdAsync(spotId, focusOnCar);
			}
			finally
			{
				if (!_spawnedCars.ContainsKey(spotId))
				{
					spot.isOccupied = false;
				}
				if (ManagerUI.HasValue)
				{
					ManagerUI.Value.CloseWaitingWindow();
				}
			}
		}

		private async UniTask LoadCarBySpotIdAsync(int spotId, bool focusOnCar)
		{
			int slotIdOnSpot = GarageSpots[spotId];
			if (slotIdOnSpot == -1)
			{
				return;
			}
			if (TryReleaseStaleSlot(slotIdOnSpot))
			{
				_controller.GetGarageSpot(spotId).isOccupied = false;
				return;
			}
			Transform spotTransform = _controller.GetGarageSpot(spotId).spawnTransform;
			if (!spotTransform)
			{
				return;
			}
			GameObject car = (await CarLoadingUtility.LoadCarFromSlot(slotIdOnSpot, spotTransform.position, spotTransform.rotation, tryUnloadCurrent: false, waitBodyLoad: true, isMultiplayer: false, resetPos: false, focusOnCar)).Item2;
			await UniTask.Yield();
			await UniTask.Yield();
			if (!car)
			{
				return;
			}
			if (!spotTransform)
			{
				CarLoadingUtility.UnloadCar(car);
				return;
			}
			VehicleData component = car.GetComponent<VehicleData>();
			PlaceCarOnSpot(spotId, component);
			if (focusOnCar)
			{
				if (!V.DriverPerson && car.TryGetComponent<DriverController>(out var _))
				{
					V.LastUsedCar = (V.Car = car);
					V.SlotID = slotIdOnSpot;
					V.CarId = component.CarId;
					AnalyticsHelper.SaveLastCarId(V.CarId);
					component.InteriorsComposition.TryRestoreSavedInterior();
					component.CarMirrorController?.SetMirror(FlagsStorage.ShowCarMirror);
					V.CoreGameManager.SetupCarDriver(car);
				}
				else
				{
					V.CoreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car);
					V.CoreGameManager.CarOutInControl.InCar(V.DriverPerson, car).Forget();
				}
				FocusOnCarOnSpot(spotId);
			}
		}

		public async UniTaskVoid RemoveCarFromSpot(int spotId)
		{
			GarageSpot currentSpot = _controller.GetGarageSpot(spotId);
			if (!_spawnedCars.ContainsKey(spotId))
			{
				throw new Exception($"No car on spot {spotId}");
			}
			GameObject car = _spawnedCars[spotId].gameObject;
			if (car == V.Car && !FlagsStorage.OutOfCar && (bool)V.DriverPerson)
			{
				bool flag = false;
				for (int i = 0; i < _controller.SpawnPointsCount; i++)
				{
					GarageSpot garageSpot = _controller.GetGarageSpot(i);
					if (garageSpot != currentSpot && garageSpot.isOccupied)
					{
						GameObject currentCar = _spawnedCars[garageSpot.id].gameObject;
						await V.CoreGameManager.CarOutInControl.InCar(V.DriverPerson, currentCar);
						CameraManager.Value.ActivateCamera(RacingCameraType.GarageLookAround, currentCar.transform);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					V.CoreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car);
					_freeDriverService.Value?.SetupFreeDriver().Forget();
				}
			}
			CarLoadingUtility.UnloadCar(car);
			_spawnedCars.Remove(spotId);
			currentSpot.isOccupied = false;
		}

		public void PlaceCarOnSpot(int spotId, VehicleData vehicleData, bool turnOff = true, bool setIsOccupied = false)
		{
			GarageSpot garageSpot = _controller.GetGarageSpot(spotId);
			if (setIsOccupied)
			{
				garageSpot.isOccupied = true;
			}
			Transform spawnTransform = garageSpot.spawnTransform;
			Vector3 origin = spawnTransform.position + (vehicleData.isTruck ? (spawnTransform.forward * 2f) : Vector3.zero);
			Vector3 posByWheel = VectorExtensions.GetPosByWheel(vehicleData.gameObject, origin);
			vehicleData.transform.SetPositionAndRotation(posByWheel, spawnTransform.rotation);
			vehicleData.isGarageCar = true;
			_spawnedCars.Add(spotId, vehicleData);
			if (!vehicleData.TryGetComponent<CarLODControl>(out var _))
			{
				vehicleData.gameObject.AddComponent<CarLODControl>();
			}
			ResetSteerAndWheelsPosition(vehicleData);
			TryApplySavedPose(vehicleData, spotId);
			SettleCarPhysics(vehicleData);
			ClearSpaceForCar(vehicleData);
			PlayerDatas.CreateSkid(vehicleData.gameObject);
		}

		public void Clear()
		{
			for (int i = 0; i < _controller.SpawnPointsCount; i++)
			{
				GarageSpot garageSpot = _controller.GetGarageSpot(i);
				if (garageSpot != null)
				{
					garageSpot.isOccupied = false;
				}
			}
			_spawnedCars.Clear();
		}

		private void ScheduleCarStatic(VehicleData vehicleData)
		{
			if ((bool)vehicleData)
			{
				if (vehicleData.TryGetComponent<PhotonView>(out var component))
				{
					UnityEngine.Object.Destroy(component);
				}
				_controller.DisableCarPhysicsDelayed(vehicleData, 2f);
			}
		}

		private void SettleCarPhysics(VehicleData vehicleData)
		{
			if (!vehicleData)
			{
				return;
			}
			if (vehicleData.TryGetComponent<PhotonView>(out var component))
			{
				UnityEngine.Object.Destroy(component);
			}
			if (vehicleData.gameObject == V.Car)
			{
				StartSettle(vehicleData);
				return;
			}
			_pendingSettle.Enqueue(vehicleData);
			if (!_settlePumpRunning)
			{
				PumpSettleQueue().Forget();
			}
		}

		private void StartSettle(VehicleData vehicleData)
		{
			_controller.EnableCarPhysics(vehicleData, isEnabled: true);
			_controller.DisableCarPhysicsDelayed(vehicleData, 2f);
		}

		private async UniTaskVoid PumpSettleQueue()
		{
			_settlePumpRunning = true;
			try
			{
				while (_pendingSettle.Count > 0)
				{
					VehicleData vehicleData = _pendingSettle.Dequeue();
					if ((bool)vehicleData && vehicleData.gameObject != V.Car)
					{
						StartSettle(vehicleData);
					}
					await UniTask.NextFrame();
				}
			}
			finally
			{
				_settlePumpRunning = false;
			}
		}

		private void ShutdownEngines()
		{
			foreach (VehicleData item in from vehicle in _spawnedCars.Select((KeyValuePair<int, VehicleData> spawnedCar) =>
				{
					KeyValuePair<int, VehicleData> keyValuePair = spawnedCar;
					return keyValuePair.Value;
				})
				where vehicle
				where (bool)vehicle.GetPowertrain && (vehicle.GetPowertrain.IsEngineWorks || vehicle.GetPowertrain.Starter > 0f)
				select vehicle)
			{
				item.GetPowertrain.ShutdownEngine();
			}
		}

		public void ResetLastCarPosition(bool resetWheelsRotation = false)
		{
			ShutdownEngines();
			if ((bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<VehicleData>(out var component))
			{
				(Vector3 spotPosition, Vector3 spotRotation) spotPositionAndRotation = GetSpotPositionAndRotation(component);
				Vector3 item = spotPositionAndRotation.spotPosition;
				Vector3 item2 = spotPositionAndRotation.spotRotation;
				item = CorrectSpotPosition(component, item);
				component.GetPowertrain.ResetTransmission(resetGear: false);
				Vector3 position = component.transform.position;
				TryCorrectCarPosition(position, component, item, item2);
				if (resetWheelsRotation)
				{
					ResetSteerAndWheelsPosition(component);
				}
				GlobalEventSystem.InvokeCarForcePositioned(component);
				RemoveCarPoseIfExists(component);
				ClearSpaceForCar(component);
			}
		}

		private Vector3 CorrectSpotPosition(VehicleData carData, Vector3 spotPosition)
		{
			if (carData.transform.position.y < -400f)
			{
				spotPosition.y++;
			}
			return spotPosition;
		}

		private void TryCorrectCarPosition(Vector3 carPosition, VehicleData carData, Vector3 spotPosition, Vector3 spotRotation)
		{
			if (!carPosition.x.AlmostEquals(spotPosition.x, 1f) || !carPosition.z.AlmostEquals(spotPosition.z, 1f) || !carPosition.y.AlmostEquals(spotPosition.y, 3f) || !carData.transform.eulerAngles.y.AlmostEquals(spotRotation.y, 5f))
			{
				Vector3 posByWheel = VectorExtensions.GetPosByWheel(carData.gameObject, spotPosition);
				carData.transform.SetPositionAndRotation(posByWheel, Quaternion.Euler(spotRotation));
			}
		}

		private (Vector3 spotPosition, Vector3 spotRotation) GetSpotPositionAndRotation(VehicleData carData)
		{
			Vector3 item = Vector3.zero;
			Vector3 item2 = Vector3.zero;
			if (carData.slotId == -1 || SlotService.SlotCollection.Index != SlotService.GetSlotCollectionBySlotId(carData.slotId).Index)
			{
				int num = 0;
				for (int i = 0; i < _controller.SpawnPointsCount; i++)
				{
					GarageSpot garageSpot = _controller.GetGarageSpot(i);
					if ((bool)garageSpot.spawnTransform)
					{
						num++;
						item += garageSpot.spawnTransform.position;
					}
				}
				if (num > 0)
				{
					item /= (float)num;
				}
			}
			else
			{
				int spotBySlotIdOrEmpty = GarageSpots.GetSpotBySlotIdOrEmpty(carData.slotId, SlotService);
				Transform spawnTransform = _controller.GetGarageSpot(spotBySlotIdOrEmpty).spawnTransform;
				item = spawnTransform.position + (carData.isTruck ? (spawnTransform.forward * 2f) : Vector3.zero);
				item2 = spawnTransform.eulerAngles;
			}
			return (spotPosition: item, spotRotation: item2);
		}

		public void ResetAllCarsPositionsInCurrentGarage()
		{
			ShutdownEngines();
			foreach (KeyValuePair<int, VehicleData> spawnedCar in _spawnedCars)
			{
				VehicleData value = spawnedCar.Value;
				ResetSavedCarPosition(value);
			}
		}

		public void ResetSavedCarPosition(VehicleData vehicleData)
		{
			if ((bool)vehicleData)
			{
				(Vector3 spotPosition, Vector3 spotRotation) spotPositionAndRotation = GetSpotPositionAndRotation(vehicleData);
				Vector3 item = spotPositionAndRotation.spotPosition;
				Vector3 item2 = spotPositionAndRotation.spotRotation;
				item = CorrectSpotPosition(vehicleData, item);
				vehicleData.GetPowertrain.ResetTransmission();
				SettleCarPhysics(vehicleData);
				Vector3 position = vehicleData.transform.position;
				TryCorrectCarPosition(position, vehicleData, item, item2);
				ResetSteerAndWheelsPosition(vehicleData);
				RemoveCarPoseIfExists(vehicleData);
				ClearSpaceForCar(vehicleData);
			}
		}

		private void ResetSteerAndWheelsPosition(VehicleData vehicleData)
		{
			if ((bool)vehicleData.GetPowertrain)
			{
				vehicleData.GetPowertrain.SetSteer(0f, useAssistent: false);
				_carInputManager.OnLeftRightSteerReleased();
				_carInputManager.ResetSteerAndWheelsPosition();
				vehicleData.GetPowertrain.SetSteer(0f, useAssistent: false);
			}
		}

		public void ClearSpaceForCar(VehicleData vehicleData)
		{
			if (!vehicleData || !vehicleData.Collider)
			{
				return;
			}
			Transform transform = vehicleData.Collider.transform;
			Vector3 localScale = transform.localScale;
			try
			{
				transform.localScale = localScale * 0.9f;
				Physics.SyncTransforms();
				for (int i = 0; i < 16; i++)
				{
					bool flag = false;
					if (_spawnedCars != null)
					{
						foreach (VehicleData value in _spawnedCars.Values)
						{
							if ((bool)value && !(value == vehicleData))
							{
								Collider collider = value.Collider;
								if ((bool)collider && collider.enabled && Physics.ComputePenetration(vehicleData.Collider, transform.position, transform.rotation, collider, collider.transform.position, collider.transform.rotation, out var _, out var _))
								{
									ResetSavedCarPosition(value);
									flag = true;
								}
							}
						}
					}
					if (!flag)
					{
						break;
					}
				}
			}
			finally
			{
				transform.localScale = localScale;
				Physics.SyncTransforms();
			}
		}

		private bool IsInForbiddenZone(VehicleData vehicleData)
		{
			if (!vehicleData || !vehicleData.Collider)
			{
				return false;
			}
			IReadOnlyList<Collider> readOnlyList = (_forbiddenZonesProvider?.Current)?.Zones;
			if (readOnlyList == null || readOnlyList.Count == 0)
			{
				return false;
			}
			Collider collider = vehicleData.Collider;
			Transform transform = collider.transform;
			for (int i = 0; i < readOnlyList.Count; i++)
			{
				Collider collider2 = readOnlyList[i];
				if ((bool)collider2 && collider2.enabled)
				{
					Transform transform2 = collider2.transform;
					Vector3 position = transform2.position;
					Quaternion rotation = transform2.rotation;
					if (Physics.ComputePenetration(collider, transform.position, transform.rotation, collider2, position, rotation, out var _, out var _))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void RemoveCarPoseIfExists(VehicleData carData)
		{
			string text = SlotService.GetSlot(carData.slotId)?.CarFilePath;
			if (!string.IsNullOrEmpty(text))
			{
				_garagePoseService.RemovePose(text);
			}
		}

		public void MoveLastCarToSavedPosition(bool resetWheelsRotation)
		{
			ShutdownEngines();
			if (!V.LastUsedCar || !V.LastUsedCar.TryGetComponent<VehicleData>(out var component))
			{
				return;
			}
			if (component.slotId == -1 || SlotService.SlotCollection.Index != SlotService.GetSlotCollectionBySlotId(component.slotId).Index)
			{
				ResetLastCarPosition(resetWheelsRotation);
				ClearSpaceForCar(component);
				return;
			}
			int spotBySlotIdOrEmpty = GarageSpots.GetSpotBySlotIdOrEmpty(component.slotId, SlotService);
			if (TryApplySavedPose(component, spotBySlotIdOrEmpty))
			{
				ClearSpaceForCar(component);
				component.GetPowertrain.ResetTransmission(resetGear: false);
			}
			else
			{
				ResetLastCarPosition(resetWheelsRotation);
				ClearSpaceForCar(component);
			}
			ClearForbiddenZones(component);
		}

		public void SaveCarsPositions()
		{
			ShutdownEngines();
			foreach (KeyValuePair<int, VehicleData> spawnedCar in _spawnedCars)
			{
				VehicleData value = spawnedCar.Value;
				SaveCarPosition(value);
			}
			VehicleData vehicleData = null;
			if ((bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<VehicleData>(out var component) && !_spawnedCars.ContainsValue(component))
			{
				vehicleData = component;
				SaveCarPosition(component);
			}
			ClearForbiddenZones(vehicleData);
		}

		public void SaveCarPosition(VehicleData vehicleData)
		{
			if (IsInForbiddenZone(vehicleData))
			{
				return;
			}
			bool flag = IsWheelsOnGround(vehicleData, 4);
			if (!(vehicleData == null) && flag)
			{
				string text = SlotService.GetSlot(vehicleData.slotId)?.CarFilePath;
				if (!string.IsNullOrEmpty(text))
				{
					_garagePoseService.SetPose(_garageSelectorService.CurrentGarageIndex, text, vehicleData.GetPowertrain);
				}
				vehicleData.GetPowertrain.ResetTransmission(resetGear: false);
			}
		}

		private bool IsWheelsOnGround(VehicleData vehicleData, int wheelsCount)
		{
			int num = 0;
			foreach (WheelProvider allWheel in vehicleData.GetPowertrain.GetAxles.AllWheels)
			{
				if (allWheel.OnGroundDown)
				{
					num++;
				}
			}
			return num >= wheelsCount;
		}

		private void ClearForbiddenZones(VehicleData vehicleData, bool excludeVehicle = false)
		{
			foreach (KeyValuePair<int, VehicleData> spawnedCar in _spawnedCars)
			{
				VehicleData value = spawnedCar.Value;
				if ((bool)value && !IsSkipVehicle(vehicleData, excludeVehicle, value) && IsInForbiddenZone(value))
				{
					int spotBySlotIdOrEmpty = GarageSpots.GetSpotBySlotIdOrEmpty(value.slotId, SlotService);
					if (TryApplySavedPose(value, spotBySlotIdOrEmpty))
					{
						ClearSpaceForCar(value);
						ScheduleCarStatic(value);
					}
					else
					{
						ResetSavedCarPosition(value);
					}
				}
			}
			if (!((!vehicleData || !vehicleData.Collider) | excludeVehicle) && IsInForbiddenZone(vehicleData))
			{
				ResetSavedCarPosition(vehicleData);
			}
		}

		private bool IsSkipVehicle(VehicleData vehicleData, bool exclude, VehicleData spawnedVehicleData)
		{
			return ((bool)vehicleData && (bool)vehicleData.Collider && vehicleData == spawnedVehicleData) & exclude;
		}

		public VehicleData FocusOnCarOnSpot(int spotId, bool withCameraReset = true)
		{
			_spawnedCars.TryGetValue(spotId, out var value);
			CorrectCamera(value, withCameraReset);
			return value;
		}

		public VehicleData GetCarBySpot(int spotId)
		{
			if (!_spawnedCars.TryGetValue(spotId, out var value))
			{
				return null;
			}
			return value;
		}

		public VehicleData[] GetAllCars()
		{
			return _spawnedCars.Values.ToArray();
		}

		protected virtual async UniTask LoadOtherGarageCars()
		{
			FlagsStorage.IsLoadingGarageCars = true;
			try
			{
				int count = 0;
				List<UniTask> tasks = new List<UniTask>();
				for (int i = 0; i < _controller.SpawnPointsCount; i++)
				{
					GarageSpot garageSpot = _controller.GetGarageSpot(i);
					int num = count + 1;
					count = num;
					if (num > SlotService.MaxSlotCount)
					{
						break;
					}
					if (GarageSpots[garageSpot.id] != -1 && !garageSpot.isOccupied)
					{
						tasks.Add(LoadCarBySpotId(garageSpot.id, focusOnCar: false));
						await UniTask.Yield();
					}
				}
				await UniTask.WhenAll(tasks);
			}
			finally
			{
				FlagsStorage.IsLoadingGarageCars = false;
			}
			FlagsStorage.CanEnterGarage = true;
		}

		protected virtual void EnterCar(GameObject person, VehicleData vehicleData)
		{
			_controller.EnableCarPhysics(vehicleData, isEnabled: true);
		}

		protected virtual void ExitCar(GameObject person, VehicleData vehicleData)
		{
			SaveCarPosition(vehicleData);
			ClearForbiddenZones(vehicleData, excludeVehicle: true);
		}

		public abstract void CorrectCamera(VehicleData vehicleData, bool withCameraReset = true);

		public virtual UniTask Exit(CancellationToken cancellationToken)
		{
			FlagsStorage.IsLoadingGarageCars = false;
			PlayerDatas.TryUnloadResource();
			V.OnCarEntered -= EnterCar;
			V.OnCarExit -= ExitCar;
			CPMGraphicsSettingsManager.Instance.RenderDistanceController.RemoveSettings(GraphicsDistanceController.PriorityType.Garage);
			FlagsStorage.IsInputEnabledInThisWindow = true;
			return UniTask.CompletedTask;
		}
	}
}
