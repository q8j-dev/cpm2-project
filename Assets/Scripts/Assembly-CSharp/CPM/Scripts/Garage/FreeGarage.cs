using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using Gley.TrafficSystem.Internal;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.VinylsAi;

namespace CPM.Scripts.Garage
{
	public class FreeGarage : GarageEntity, IGarageEntity
	{
		private const float DelayMoveGlobal = 0.2f;

		private SunAndSkyController _sunAndSkyController;

		private MapCamera _mapCamera;

		private CarCameras _carCameras;

		private PlayerVinylCensorChecker _playerVinylCensorChecker;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private LevelsProvider _levelsProvider;

		private IGarageSelectorService _garageSelectorService;

		private GameObject _enteredVehicle;

		private WwiseFreeContainer _wwiseFreeContainer;

		private EnvironmentSettings _freeEnvironmentSettings;

		private EnvironmentSettings _garageEnvironmentSettings;

		private CarFactoryService _carFactoryService;

		private const int DynamicCamIndex = 5;

		private const int InsideCamIndex = 0;

		private const float InsideCameraHoldSeconds = 3f;

		private const string InsideViewName = "driverViewCam";

		private Vector3 _exitCamLocalPosition;

		private Quaternion _exitCamLocalRotation;

		private bool _hasExitCamPose;

		private GameObject _carKeptForExit;

		[Inject]
		public void Construct(MapCamera mapCamera, CarCameras carCameras, PlayerVinylCensorChecker playerVinylCensorChecker, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, IGarageSelectorService garageSelectorService, CarFactoryService carFactoryService, WwiseFreeContainer wwiseFreeContainer)
		{
			_levelsProvider = levelsProvider;
			_freeDriveProvider = freeDriveProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_carCameras = carCameras;
			_mapCamera = mapCamera;
			_playerVinylCensorChecker = playerVinylCensorChecker;
			_garageSelectorService = garageSelectorService;
			_carFactoryService = carFactoryService;
			_wwiseFreeContainer = wwiseFreeContainer;
		}

		public override async UniTask Init(VehicleData enteredVehicle, GarageController controller, CancellationToken cancellationToken)
		{
			_sunAndSkyController = GlobalEventSystem.RequestSunController();
			await SetEnvironmentSettings(cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			CPMGraphicsSettingsManager.Instance.ApplyCameraShadowsQuality();
			_enteredVehicle = enteredVehicle.gameObject;
			FlagsStorage.CanEnterGarage = false;
			FlagsStorage.InGarageFree = true;
			GarageSpots = StorageService.Data.GarageSpotCollection;
			UpdateFreeState(isActive: false);
			_mapCamera.EnableDisableInteriorMap(enable: false);
			ScreenFader.FadeOut();
			UpdateCarEnterSubscriptions(toSubscribe: true);
			if (PhotonNetwork.IsConnected && (bool)enteredVehicle.photonView)
			{
				V.OnNewCarCreated += NewCarCreatedHandler;
				V.OnNewPersonCreated += NewPersonCreatedHandler;
				enteredVehicle.photonView.RpcToDefaultGroup("DisableCarRPC", RpcTarget.OthersBuffered, false);
				enteredVehicle.photonView.Synchronization = ViewSynchronization.Off;
				V.DriverPerson.GetPhotonView().Synchronization = ViewSynchronization.Off;
				MultiPlayerPosControl component = enteredVehicle.GetComponent<MultiPlayerPosControl>();
				MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
				foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
				{
					if (!(multiPlayerPosControl == null) && !(multiPlayerPosControl == component))
					{
						multiPlayerPosControl.forceDisableCar = true;
					}
				}
				MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
				foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
				{
					if (!(multiPersonControl == null))
					{
						multiPersonControl.forceDisablePerson = true;
					}
				}
			}
			if (enteredVehicle.slotId != -1)
			{
				enteredVehicle.isGarageCar = true;
				if (SlotService.SlotCollection.Index == SlotService.GetSlotCollectionBySlotId(enteredVehicle.slotId).Index)
				{
					_spawnedCars.Add(enteredVehicle.slotId, enteredVehicle);
					int spotBySlotIdOrEmpty = GarageSpots.GetSpotBySlotIdOrEmpty(enteredVehicle.slotId, SlotService);
					controller.GetGarageSpot(spotBySlotIdOrEmpty).isOccupied = true;
				}
			}
			await base.Init(enteredVehicle, controller, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				_carCameras.ChangeForceCameraForSecond();
				TransformCarToGarageTunnel(enteredVehicle);
				CPMGraphicsSettingsManager.Instance.ApplyExposure();
				CPMGraphicsSettingsManager.Instance.ApplyRealtimeReflections();
				if (!SceneControl.KeepLocationLoadedForGarage)
				{
					PlayerDatas.UnloadResourcesForce();
				}
				_carCameras.ChangeForceCameraForSecond();
				_controller.SetActiveOnlyFreeParts(enable: false);
				V.CoreGameManager.FreeDriveProvider.QuickCarSettings.ShowDoor();
				FlagsStorage.IsInGarageTunnel = false;
				FlagsStorage.CanEnterGarage = true;
				if ((bool)_wwiseFreeContainer)
				{
					_wwiseFreeContainer.WindFreeSound.StopSound();
				}
			}
		}

		private void TransformCarToGarageTunnel(VehicleData enteredVehicle)
		{
			Transform transform = _controller.TunnelVisual.transform;
			Transform transform2 = _controller.GarageTunnelVisuals.transform;
			Transform transform3 = enteredVehicle.transform;
			Vector3 position = transform.InverseTransformPoint(transform3.position);
			if (_garageSelectorService.CurrentGarageIndex == 1)
			{
				position.z = 0f - position.z;
			}
			Vector3 position2 = transform2.TransformPoint(position);
			Quaternion quaternion = Quaternion.Inverse(transform.rotation) * transform3.rotation;
			quaternion = Quaternion.Euler(_controller.TunnelRotationSide) * quaternion;
			Quaternion rotation = transform2.rotation * quaternion;
			transform3.SetPositionAndRotation(position2, rotation);
		}

		private async UniTask SetEnvironmentSettings(CancellationToken cancellationToken)
		{
			_freeEnvironmentSettings = EnvironmentSettings.FromCurrent();
			SetFreeAtmosphereActive(isActive: false);
			Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(_garageSelectorService.CurrentSceneIndex);
			Scene freeScene = SceneManager.GetSceneByBuildIndex(13);
			SceneManager.SetActiveScene(sceneByBuildIndex);
			await UniTask.DelayFrame(2, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (cancellationToken.IsCancellationRequested)
			{
				SetFreeAtmosphereActive(isActive: true);
				return;
			}
			_garageEnvironmentSettings = EnvironmentSettings.FromCurrent();
			SceneManager.SetActiveScene(freeScene);
			await UniTask.DelayFrame(2, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (cancellationToken.IsCancellationRequested)
			{
				SetFreeAtmosphereActive(isActive: true);
			}
			else
			{
				_garageEnvironmentSettings.Apply();
			}
		}

		private void SetFreeAtmosphereActive(bool isActive)
		{
			if ((bool)_sunAndSkyController)
			{
				_sunAndSkyController.gameObject.SetActive(isActive);
			}
		}

		private void ResetEnvironmentSettings()
		{
			if (_freeEnvironmentSettings != null)
			{
				_freeEnvironmentSettings.Apply();
			}
		}

		protected override void InitVSlotId()
		{
		}

		private void NewPersonCreatedHandler(MultiPersonControl person)
		{
			if (FlagsStorage.InGarageFree)
			{
				person.forceDisablePerson = true;
			}
		}

		public override async UniTask Exit(CancellationToken cancellationToken)
		{
			ResetEnvironmentSettings();
			SetFreeAtmosphereActive(isActive: true);
			FlagsStorage.IsInGarageTunnel = true;
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				V.SlotID = component.slotId;
			}
			RememberExitCameraPose();
			UpdateFreeState(isActive: true);
			ScreenFader.FadeOut(10f, 0f);
			UpdateCarEnterSubscriptions(toSubscribe: false);
			if (PhotonNetwork.IsConnected)
			{
				V.OnNewCarCreated -= NewCarCreatedHandler;
				V.OnNewPersonCreated -= NewPersonCreatedHandler;
				PhotonNetwork.Destroy(V.DriverPerson);
			}
			_carKeptForExit = ((!FlagsStorage.IsMultiFree && !PhotonNetwork.IsConnected && (bool)V.Car) ? V.Car : null);
			foreach (KeyValuePair<int, VehicleData> spawnedCar in _spawnedCars)
			{
				VehicleData value = spawnedCar.Value;
				if ((bool)value && value.gameObject == _carKeptForExit)
				{
					_controller.GetGarageSpot(spawnedCar.Key).isOccupied = false;
				}
				else if (!(value == null) && !(_enteredVehicle == value.gameObject))
				{
					CarLoadingUtility.UnloadCar(value.gameObject, clearResources: false);
					_controller.GetGarageSpot(spawnedCar.Key).isOccupied = false;
				}
			}
			if ((bool)_enteredVehicle)
			{
				if (_enteredVehicle.TryGetComponent<Powertrain>(out var component2))
				{
					FlagsStorage.IsAutomatic = component2.Automatic;
					FlagsStorage.LoadedFromGarage = true;
				}
				if (_enteredVehicle != _carKeptForExit)
				{
					CarLoadingUtility.UnloadCar(_enteredVehicle, clearResources: false);
				}
			}
			_controller.SetVisualActive(isActive: true);
			await LastSpawn(cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			_carCameras.ChangeForceCameraForSecond();
			if (PhotonNetwork.IsConnected)
			{
				MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
				foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
				{
					if ((bool)multiPlayerPosControl && !multiPlayerPosControl.VehicleData.isGarageCar)
					{
						multiPlayerPosControl.forceDisableCar = false;
					}
				}
				MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
				foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
				{
					if ((bool)multiPersonControl)
					{
						multiPersonControl.SetActive(active: true);
						multiPersonControl.forceDisablePerson = false;
					}
				}
			}
			FlagsStorage.InGarageFree = false;
			Clear();
			_spawnedCars.Clear();
			await base.Exit(cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			Scene scene;
			if (Detector.Resident.IsResidentFor(_garageSelectorService.CurrentSceneIndex))
			{
				Detector.Resident.Deactivate();
			}
			else if (ScenesHelper.TryGetLoadedScene(_garageSelectorService.CurrentSceneIndex, out scene))
			{
				if (SceneControl.KeepLocationLoadedForGarage)
				{
					await SceneManager.UnloadSceneAsync(scene).ToUniTask(null, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
				else
				{
					SceneManager.UnloadSceneAsync(scene);
				}
			}
			if (!SceneControl.KeepLocationLoadedForGarage)
			{
				await SceneManager.LoadSceneAsync(ScenesHelper.GetFreeFromGarageIndex(_garageSelectorService.CurrentSceneIndex), LoadSceneMode.Additive).ToUniTask(null, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			else if ((bool)V.SceneControl)
			{
				V.SceneControl.ShowWorldAfterGarage();
			}
			_controller.SetActiveOnlyFreeParts(enable: true);
			ScreenFader.FadeIn();
			_controller.SetVisualActive(isActive: false);
			if (SceneControl.KeepLocationLoadedForGarage)
			{
				LightmapSettings.lightmapsMode = SceneControl.PreGarageLightmapsMode;
				LightmapSettings.lightmaps = SceneControl.PreGarageLightmaps;
				LightmapSettings.lightProbes = SceneControl.PreGarageLightProbes;
			}
			else
			{
				LightmapSettings.lightmaps = null;
				LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
				LightmapSettings.lightProbes = null;
			}
			if (!SceneControl.KeepLocationLoadedForGarage)
			{
				PlayerDatas.UnloadResourcesForce();
			}
			_mapCamera.EnableDisableInteriorMap(enable: true);
			CPMGraphicsSettingsManager.Instance.ApplyExposure();
			CPMGraphicsSettingsManager.Instance.ApplyRealtimeReflections();
			await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (!cancellationToken.IsCancellationRequested)
			{
				if ((bool)MoveElements._object)
				{
					MoveElements._object.MoveGlobalPointOnGarageOut();
				}
				FlagsStorage.IsInGarageTunnel = false;
				if ((bool)_wwiseFreeContainer)
				{
					_wwiseFreeContainer.WindFreeSound.PlaySound();
				}
				if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component3) && (bool)component3.Collider)
				{
					component3.Collider.gameObject.SetActive(value: true);
				}
				if (V.CurrentCamIndex == 0 && (bool)V.Car)
				{
					V.CoreGameManager.carCamerasController.SetCamera(V.CurrentCamIndex, V.Car.transform, newTarget: false);
				}
			}
		}

		private void RememberExitCameraPose()
		{
			_hasExitCamPose = false;
			if (V.CurrentCamIndex == 0 && (bool)V.Car && V.Car.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
			{
				Transform transform = component.camInsideView.transform;
				_exitCamLocalPosition = transform.localPosition;
				_exitCamLocalRotation = transform.localRotation;
				_hasExitCamPose = true;
			}
		}

		private async UniTaskVoid ApplyExitCameraPose(GameObject car, CancellationToken cancellationToken)
		{
			if (!_hasExitCamPose || !car.TryGetComponent<DriverController>(out var driverController))
			{
				return;
			}
			if (!driverController.camInsideView)
			{
				driverController.camInsideView = new GameObject("driverViewCam");
				driverController.camInsideView.transform.SetParent(car.transform, worldPositionStays: false);
			}
			float deadline = Time.time + 3f;
			while ((bool)driverController && V.CurrentCamIndex == 0 && Time.time < deadline && !cancellationToken.IsCancellationRequested)
			{
				if ((bool)driverController.camInsideView)
				{
					driverController.camInsideView.transform.SetLocalPositionAndRotation(_exitCamLocalPosition, _exitCamLocalRotation);
					driverController.camPosInitialInsideView = _exitCamLocalPosition;
					driverController.camPosDefaultInsideView = _exitCamLocalPosition;
					_carCameras.Target = driverController.camInsideView.transform;
				}
				await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			}
		}

		public override void CorrectCamera(VehicleData vehicleData, bool withCameraReset = true)
		{
		}

		protected override void EnterCar(GameObject person, VehicleData vehicleData)
		{
			base.EnterCar(person, vehicleData);
			if (vehicleData.isGarageCar)
			{
				_freeDriveProvider.FirstCarMultiPlayer = vehicleData.gameObject;
				if (FlagsStorage.InGarageFree && FlagsStorage.IsMultiFree)
				{
					_playerVinylCensorChecker.CheckCurrentCar();
				}
			}
		}

		private async UniTask LastSpawn(CancellationToken cancellationToken)
		{
			Vector3 pos = _controller.FreeExitPointsByGarages[_garageSelectorService.CurrentGarageIndex].position;
			Quaternion rot = Quaternion.Euler(_controller.FreeExitPointsByGarages[_garageSelectorService.CurrentGarageIndex].eulerAngles + (FlagsStorage.EnteredForwardInGarageTunnel ? Vector3.zero : new Vector3(0f, 180f, 0f)));
			VehicleData vehicleData = V.Car.GetComponent<VehicleData>();
			CarInfo carInfo = vehicleData.carInfo;
			_ = Time.realtimeSinceStartup;
			GameObject keptCar = _carKeptForExit;
			_carKeptForExit = null;
			if ((bool)keptCar)
			{
				V.Car = keptCar;
				PrepareKeptCarForExit(vehicleData);
			}
			else if ((int)V.SlotID == -1)
			{
				V.Car = (await CarLoadingUtility.LoadCarFromTemp(carInfo, pos, rot, tryUnloadCurrent: false, waitBodyLoad: true, FlagsStorage.IsMultiFree)).Item2;
				vehicleData.IsTraded = new AdvancedBoolean(v: true);
			}
			else
			{
				V.Car = (await CarLoadingUtility.LoadCarFromSlot(V.SlotID, pos, rot, tryUnloadCurrent: false, waitBodyLoad: true, FlagsStorage.IsMultiFree)).Item2;
			}
			if (!cancellationToken.IsCancellationRequested && (bool)V.Car)
			{
				if (V.Car.TryGetComponent<Powertrain>(out var component) && !component.IsEngineWorks)
				{
					component.StartEngine(starterSound: false);
				}
				V.Car.transform.parent = V.CoreGameManager.globalPoint.transform;
				_freeDriveProvider.FirstCarMultiPlayer = V.Car;
				if (!keptCar && V.Car.TryGetComponent<VehicleData>(out var component2))
				{
					RestoreInteriorWhenBodyLoaded(component2, component2.GetCancellationTokenOnDestroy()).Forget();
				}
				Powertrain component3 = V.Car.GetComponent<Powertrain>();
				Rigidbody component4 = V.Car.GetComponent<Rigidbody>();
				Axles component5 = V.Car.GetComponent<Axles>();
				V.CarId = vehicleData.CarId;
				AnalyticsHelper.SaveLastCarId(V.CarId);
				if ((bool)_freeDriveProvider.QuickCarSettings)
				{
					_freeDriveProvider.QuickCarSettings.PreSetValues();
				}
				if (!V.Car.TryGetComponent<CarTriggers>(out var component6))
				{
					component6 = V.Car.AddComponent<CarTriggers>();
				}
				component6.LevelScript = _levelsProvider.LevelScript;
				component6.FreeDriveProvider = _freeDriveProvider;
				component6.LevelsProvider = _levelsProvider;
				if (!V.Car.TryGetComponent<Dragy>(out var _))
				{
					V.Car.AddComponent<Dragy>();
				}
				PlayerDatas.CreateSkid(V.Car);
				_freeDriveProvider.CarCameras.carRigidbody = component4;
				_freeDriveProvider.CarCameras.dr = component3;
				_freeDriveProvider.CarCameras.axles = component5;
				_freeDriveProvider.CarCameras.mtarget = V.Car.transform;
				_freeDriveProvider.CarCameras.Target = V.Car.transform;
				if (!FlagsStorage.IsMultiFree && SystemInfo.systemMemorySize > 1100 && PlayerPrefs.GetInt("CarQuantity", 50) > 0 && !V.Car.GetComponent<PlayerComponent>())
				{
					V.Car.AddComponent<PlayerComponent>();
				}
				component4.isKinematic = true;
				component4.velocity = Vector3.zero;
				component4.angularVelocity = Vector3.zero;
				component4.detectCollisions = true;
				TryTemporarilyTurnOffTheCollider(vehicleData);
				_freeDriveProvider.FreeDriveControl.fristSpawnCar = false;
				if (V.CurrentCamIndex == 5)
				{
					V.CoreGameManager.carCamerasController.SetCachedCar(V.DriverPerson, vehicleData);
				}
				RigidbodyInterpolation interpolation = component4.interpolation;
				if ((bool)keptCar)
				{
					component4.interpolation = RigidbodyInterpolation.None;
					component4.position = pos;
					component4.rotation = rot;
				}
				V.Car.transform.SetPositionAndRotation(pos, rot);
				Physics.SyncTransforms();
				component4.velocity = Vector3.zero;
				component4.angularVelocity = Vector3.zero;
				component4.isKinematic = false;
				if ((bool)keptCar)
				{
					component4.interpolation = interpolation;
				}
				ApplyExitCameraPose(V.Car, cancellationToken).Forget();
				V.CoreGameManager.SetupCarDriver(V.Car);
			}
		}

		protected override async UniTask LoadOtherGarageCars()
		{
			await base.LoadOtherGarageCars();
			foreach (VehicleData value in GetSpawnedCars().Values)
			{
				if ((bool)value && value.gameObject != _enteredVehicle)
				{
					RestoreInteriorWhenBodyLoaded(value, value.GetCancellationTokenOnDestroy()).Forget();
				}
			}
		}

		private async UniTaskVoid RestoreInteriorWhenBodyLoaded(VehicleData vehicleData, CancellationToken cancellationToken)
		{
			await UniTask.WaitUntil(() => !vehicleData || vehicleData.IsCarGraphicLoaded, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (!cancellationToken.IsCancellationRequested && (bool)vehicleData)
			{
				if (CPMGraphicsSettingsManager.Instance.AdditionalEffects != CPMGraphicsSettings.AdditionalEffects.Low)
				{
					await vehicleData.InteriorsComposition.ForceSetSavedInterior(cancellationToken);
				}
				else
				{
					vehicleData.InteriorsComposition.TryRestoreSavedInterior();
				}
			}
		}

		private void PrepareKeptCarForExit(VehicleData car)
		{
			car.isGarageCar = false;
			_controller.EnableCarPhysics(car, isEnabled: true);
			if (car.TryGetComponent<CarLODControl>(out var component))
			{
				UnityEngine.Object.Destroy(component);
			}
			car.BodyLoadingComposition.ShowMainModelOnly();
		}

		private void TryTemporarilyTurnOffTheCollider(VehicleData vehicleData)
		{
			if (!vehicleData || vehicleData.isTraffiCar || vehicleData.isTruck || V.Car != vehicleData.gameObject || !FlagsStorage.IsMultiFree)
			{
				return;
			}
			vehicleData.Collider.gameObject.SetActive(value: false);
			Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				if ((bool)vehicleData && (bool)vehicleData.Collider)
				{
					vehicleData.Collider.gameObject.SetActive(value: true);
				}
			}).AddTo(vehicleData.gameObject);
		}

		private void NewCarCreatedHandler(MultiPlayerPosControl mppc)
		{
			if (FlagsStorage.InGarageFree)
			{
				mppc.forceDisableCar = true;
			}
		}

		private void UpdateFreeState(bool isActive)
		{
			if ((bool)_sunAndSkyController)
			{
				_sunAndSkyController.gameObject.SetActive(isActive);
			}
			_freeDriveProvider.FreeDriveControl.HideBigMap();
			_mapCamera.EnableDisableSmallMap(isActive);
			if ((bool)ManagerUI.Value?.CircleNotification)
			{
				ManagerUI.Value.CircleNotification.RejectAndClose();
			}
			if ((bool)ManagerUI.Value?.RequestWindowTemplate)
			{
				ManagerUI.Value.RequestWindowTemplate.Close();
			}
			if (FlagsStorage.IsMultiFree)
			{
				return;
			}
			if (isActive)
			{
				int num = PlayerPrefs.GetInt("CarQuantity", 50);
				if (TrafficManager.Exists && TrafficManager.Instance.Initialized)
				{
					API.SetTrafficDensity(num);
					TrafficManager.Instance.gameObject.SetActive(num > 0);
				}
			}
			else if (TrafficManager.Exists && TrafficManager.Instance.Initialized)
			{
				API.SetTrafficDensity(0);
			}
		}
	}
}
