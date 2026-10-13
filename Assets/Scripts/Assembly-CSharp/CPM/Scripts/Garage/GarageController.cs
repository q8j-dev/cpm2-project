using System;
using System.Collections.Generic;
using CPM.Scripts.Garage.FreeDriverService;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;

namespace CPM.Scripts.Garage
{
	public class GarageController : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> _tunnelVisuals = new List<GameObject>();

		public List<Transform> FreeExitPointsByGarages = new List<Transform>();

		public Transform DriverPos;

		public List<Vector3> RotationsForGarageSpawns;

		private IGarageEntity _currentGarage;

		private GarageSpotCollection _garageSpotCollection;

		private ISlotService _slotService;

		private Dictionary<VehicleData, IDisposable> _carPhysicsUpdateObservers = new Dictionary<VehicleData, IDisposable>();

		private ScreenFader _screenFader;

		[HideInInspector]
		public MenuGarageNavigationWindow MenuGarageNavigationWindow;

		[HideInInspector]
		public Transform CarSpawnPoint;

		[HideInInspector]
		public GameObject GarageTunnelVisuals;

		[HideInInspector]
		public Action<int> CarFocused;

		[HideInInspector]
		public Transform InnerGarageTunnelChild;

		[SerializeField]
		private GameObject[] ObjectsDisableInGarage;

		public Transform veryFirstDriverPos;

		private IStorageService _storageService;

		private CarParkTester _dynoChecker;

		private CarParkTester _exteriorChecker;

		private FreeDriveProvider _freeDriveProvider;

		private ICharacterEquipmentDataService _characterEquipmentDataService;

		private IGarageSelectorService _garageSelectorService;

		private OptionalDependency<IFreeDriverService> _freeDriverService;

		internal static readonly int[] DoubleColliderCarIds = new int[4] { 70, 101, 170, 171 };

		public GarageSpot[] SpawnPoints { private get; set; }

		public Vector3 TunnelRotationSide => RotationsForGarageSpawns[_garageSelectorService.CurrentGarageIndex];

		public GameObject TunnelVisual
		{
			get
			{
				if (_tunnelVisuals.Count == 0)
				{
					return null;
				}
				return _tunnelVisuals[_garageSelectorService.CurrentGarageIndex];
			}
		}

		public int SpawnPointsCount
		{
			get
			{
				GarageSpot[] spawnPoints = SpawnPoints;
				if (spawnPoints == null)
				{
					return 0;
				}
				return spawnPoints.Length;
			}
		}

		public IGarageEntity CurrentGarage => _currentGarage;

		public Transform FreePersonPosition { get; set; }

		public bool IsSwitchingCar { get; private set; }

		public bool IsCarOutsideCamMode { get; set; } = true;

		public bool IsInElevator { get; set; }

		public int LastSelectedCar { get; set; } = -1;

		public int LastSelecterCarInGarage { get; set; }

		public bool WasSubscribed { get; set; }

		public event Action OnGarageInited;

		public event Action OnGarageClosed;

		[Inject]
		public void Construct(IGarageEntity garageEntity, IStorageService storageService, ScreenFader screenFader, ICharacterEquipmentDataService characterEquipmentDataService, IGarageSelectorService garageSelectorService, ISlotService slotService, FreeDriveProvider freeDriveProvider, OptionalDependency<IFreeDriverService> freeDriverService)
		{
			_freeDriveProvider = freeDriveProvider;
			_storageService = storageService;
			_screenFader = screenFader;
			_currentGarage = garageEntity;
			_characterEquipmentDataService = characterEquipmentDataService;
			_garageSelectorService = garageSelectorService;
			_freeDriverService = freeDriverService;
			_slotService = slotService;
		}

		public async UniTask Init(GameObject enteredCar = null)
		{
			_garageSpotCollection = _storageService.Data.GarageSpotCollection;
			TrySubscribe();
			await _currentGarage.Init((enteredCar == null) ? null : enteredCar.GetComponent<VehicleData>(), this, base.destroyCancellationToken);
			OnGarageInited?.Invoke();
		}

		private void Start()
		{
			_garageSpotCollection = _storageService.Data.GarageSpotCollection;
			SetVisualActive(isActive: false);
		}

		public void SetVisualActive(bool isActive)
		{
			if (_tunnelVisuals == null)
			{
				return;
			}
			foreach (GameObject tunnelVisual in _tunnelVisuals)
			{
				tunnelVisual.SetActive(isActive);
			}
		}

		private void OnDestroy()
		{
			TryUnsubscribe();
			if (ScenesHelper.IsFreeDriveSceneActive())
			{
				UnsubscribeFromExitCar();
				FlagsStorage.InGarageFree = false;
				_currentGarage = null;
				_screenFader = null;
				_garageSpotCollection = null;
			}
			else
			{
				OnGarageClose();
				_currentGarage = null;
				_screenFader = null;
				_garageSpotCollection = null;
			}
		}

		private void UnsubscribeFromExitCar()
		{
			if (_currentGarage != null && FlagsStorage.InGarageFree)
			{
				_currentGarage.UpdateCarEnterSubscriptions(toSubscribe: false);
			}
		}

		private void TrySubscribe()
		{
			if (!WasSubscribed)
			{
				WasSubscribed = true;
				_garageSpotCollection.OnGarageSpotOccupied += GarageSpotOccupiedHandle;
				_garageSpotCollection.OnGarageSpotEmptied += GarageSpotEmptiedHandle;
				GlobalEventSystem.OnCarWheelsInstalled += CarWheelsInstalledHandle;
			}
		}

		private void TryUnsubscribe()
		{
			if (WasSubscribed)
			{
				Debug.Log("GarageController: TryUnsubscribe");
				WasSubscribed = false;
				_garageSpotCollection.OnGarageSpotOccupied -= GarageSpotOccupiedHandle;
				_garageSpotCollection.OnGarageSpotEmptied -= GarageSpotEmptiedHandle;
				GlobalEventSystem.OnCarWheelsInstalled -= CarWheelsInstalledHandle;
			}
		}

		private void CarWheelsInstalledHandle(VehicleData vehicleData)
		{
			if ((bool)vehicleData && vehicleData.isGarageCar && (bool)vehicleData.GetPowertrain && vehicleData.GetPowertrain.Rigidbody.isKinematic)
			{
				EnableCarPhysics(vehicleData, isEnabled: true);
				DisableCarPhysicsDelayed(vehicleData, 2f);
			}
		}

		private void GarageSpotOccupiedHandle(int spotId)
		{
			_currentGarage.LoadCarBySpotId(spotId, focusOnCar: true).Forget();
		}

		private void GarageSpotEmptiedHandle(int spotId)
		{
			_currentGarage.RemoveCarFromSpot(spotId).Forget();
		}

		public void OnGarageClose()
		{
			TryUnsubscribe();
			if (_currentGarage == null)
			{
				return;
			}
			_currentGarage.Exit(base.destroyCancellationToken).Forget();
			if (_freeDriveProvider != null)
			{
				if (PhotonNetwork.IsConnected && (bool)_freeDriveProvider.FirstCarMultiPlayer && _freeDriveProvider.FirstCarMultiPlayer.TryGetPhotonView(out var photonView))
				{
					photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
				}
				OnGarageClosed?.Invoke();
			}
		}

		public void ResetCarPosition(bool resetWheelsRotation = false)
		{
			_screenFader.FadeOut(0.5f, 0f);
			_currentGarage.ResetLastCarPosition(resetWheelsRotation);
		}

		public void ResetCarToSavedPosition(bool resetWheelsRotation = false)
		{
			_screenFader.FadeOut(0.5f, 0f);
			_currentGarage.MoveLastCarToSavedPosition(resetWheelsRotation);
		}

		public void SaveCarsPositions()
		{
			_screenFader.FadeOut(0.5f, 0f);
			_currentGarage.SaveCarsPositions();
		}

		public void ResetAllCarsPositions()
		{
			_screenFader.FadeOut(0.5f, 0f);
			_currentGarage.ResetAllCarsPositionsInCurrentGarage();
		}

		public void ClearSpaceForCar(VehicleData vehicleData)
		{
			_currentGarage.ClearSpaceForCar(vehicleData);
		}

		public GarageSpot GetGarageSpot(int index)
		{
			if (index < 0)
			{
				index = 0;
			}
			return SpawnPoints[index % SpawnPoints.Length];
		}

		public void UpdateCarEnterSubscriptions(bool toSubscribe)
		{
			if (_currentGarage != null)
			{
				_currentGarage.UpdateCarEnterSubscriptions(toSubscribe);
			}
		}

		public void FocusOnCarInMenu(int slotId)
		{
			if (!IsSwitchingCar && GarageMenuWindow.SelectedSlotId != slotId && (bool)V.LastUsedCar)
			{
				if (V.LastUsedCar.TryGetComponent<VehicleData>(out var component))
				{
					DisableCarPhysicsDelayed(component, 2f);
				}
				GarageMenuWindow.SelectedSlotId = slotId;
				FocusOnCarOnSlot(slotId).Forget();
			}
		}

		public async UniTaskVoid FocusOnCarOnSlot(int slotId)
		{
			IsSwitchingCar = true;
			int spotId = -1;
			if (slotId != -1)
			{
				spotId = _garageSpotCollection.GetSpotBySlotId(slotId, _slotService);
			}
			VehicleData vehicleData = _currentGarage.FocusOnCarOnSpot(spotId, withCameraReset: false);
			if ((int)V.SlotID != slotId && (bool)V.DriverPerson && (bool)vehicleData)
			{
				if (V.Car.TryGetComponent<VehicleData>(out var component) && Vector3.Distance(vehicleData.transform.position, component.transform.position) > 10f)
				{
					_screenFader.FadeOut(0.75f, 0.01f);
				}
				if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component2))
				{
					_currentGarage.SaveCarPosition(component2);
				}
				V.CoreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car);
				await V.CoreGameManager.CarOutInControl.InCar(V.DriverPerson, vehicleData.gameObject);
			}
			IsSwitchingCar = false;
			CarFocused?.Invoke(slotId);
		}

		public void CorrectCamera(VehicleData vehicleData)
		{
			_currentGarage.CorrectCamera(vehicleData);
		}

		public void EnableCarPhysics(VehicleData vehicleData, bool isEnabled)
		{
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 2f);
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Dyno, 2f);
			if (_carPhysicsUpdateObservers.ContainsKey(vehicleData))
			{
				_carPhysicsUpdateObservers[vehicleData]?.Dispose();
			}
			vehicleData.GetPowertrain.SetKinematic(!isEnabled);
		}

		public void DisableCarPhysicsDelayed(VehicleData vehicleData, float delay, Action callback = null)
		{
			if (_carPhysicsUpdateObservers.ContainsKey(vehicleData))
			{
				_carPhysicsUpdateObservers[vehicleData]?.Dispose();
			}
			IObservable<long> source = (from _ in Observable.EveryUpdate()
				where (bool)vehicleData && vehicleData.GetPowertrain.GetAxles.AllWheels.TrueForAll((WheelProvider w) => w.OnGroundDown)
				select _).Take(1).SelectMany((long _) => Observable.Timer(TimeSpan.FromSeconds(delay)));
			_carPhysicsUpdateObservers[vehicleData] = source.Subscribe((long _) =>
			{
				if ((bool)vehicleData)
				{
					EnableCarPhysics(vehicleData, isEnabled: false);
					callback?.Invoke();
					_carPhysicsUpdateObservers.Remove(vehicleData);
				}
			}).AddTo(vehicleData);
		}

		public void SetChecker(CarParkTester checker)
		{
			if (!(checker == null))
			{
				switch (checker.TesterType)
				{
				case TesterType.Exterior:
					_exteriorChecker = checker;
					break;
				case TesterType.Dyno:
					_dynoChecker = checker;
					break;
				}
			}
		}

		public void SetOffCheckers()
		{
			SetOffChecker(TesterType.Exterior);
			SetOffChecker(TesterType.Dyno);
		}

		private void SetOffChecker(TesterType testerType)
		{
			switch (testerType)
			{
			case TesterType.Exterior:
				if (!(_exteriorChecker == null))
				{
					_exteriorChecker.SetOffDynochecker(TesterType.Exterior);
				}
				break;
			case TesterType.Dyno:
				if (!(_dynoChecker == null))
				{
					_dynoChecker.SetOffDynochecker(TesterType.Dyno);
				}
				break;
			}
		}

		public void SetActiveOnlyFreeParts(bool enable)
		{
			GameObject[] objectsDisableInGarage = ObjectsDisableInGarage;
			foreach (GameObject gameObject in objectsDisableInGarage)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(enable);
				}
			}
		}
	}
}
