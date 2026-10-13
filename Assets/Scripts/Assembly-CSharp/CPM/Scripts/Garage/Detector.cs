using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Racing.CameraManagement;
using Service;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace CPM.Scripts.Garage
{
	[RequireComponent(typeof(BoxCollider))]
	public class Detector : MonoBehaviour
	{
		[SerializeField]
		private GarageDoor door = default;

		[SerializeField]
		private int GarageIndex = default;

		private GarageController _garageController;

		private CameraManager _cameraManager;

		private IGarageSelectorService _garageSelectorService;

		private IGarageBuyService _garageBuyService;

		private bool _isChecking;

		private FreeDriveProvider _freeDriveProvider;

		private CoreGameManager _coreGameManager;

		internal static readonly GarageWarmup Resident = new GarageWarmup();

		public static bool ResidencyEnabled = true;

		private const float ResidencyCheckInterval = 1f;

		private float _nextResidencyCheck;

		private const float ResidencyTakeoverMargin = 200f;

		private InputMapSetEvent _inputMapSetEvent;

		private void Start()
		{
			TryBecomeResident();
		}

		private void Update()
		{
			if (!(Time.unscaledTime < _nextResidencyCheck))
			{
				_nextResidencyCheck = Time.unscaledTime + 1f;
				TryBecomeResident();
			}
		}

		private void TryBecomeResident()
		{
			if (ResidencyEnabled && !_isChecking && _garageBuyService != null && _garageSelectorService != null && !FlagsStorage.InGarageFree && !FlagsStorage.IsInGarageTunnel && (!_garageBuyService.GetGarageStatus(GarageIndex) ? false : true) && RamDevice.AllowHeavyPreload())
			{
				int garageSceneIndex = _garageSelectorService.GetGarageSceneIndex(GarageIndex);
				if (garageSceneIndex >= 0 && !Resident.IsResidentFor(garageSceneIndex) && IsCloserThanResidentOwner())
				{
					Resident.EnsureResident(garageSceneIndex, new object[2] { _garageController, _cameraManager }, base.transform);
				}
			}
		}

		private bool IsCloserThanResidentOwner()
		{
			Transform owner = Resident.Owner;
			if (!owner || owner == base.transform)
			{
				return true;
			}
			Transform transform = (V.Car ? V.Car.transform : (V.DriverPerson ? V.DriverPerson.transform : null));
			if (!transform)
			{
				return false;
			}
			Vector3 position = transform.position;
			return Vector3.Distance(position, base.transform.position) + 200f < Vector3.Distance(position, owner.position);
		}

		[Inject]
		public void ConstructAlterInput(InputMapSetEvent inputMapSetEvent)
		{
			_inputMapSetEvent = inputMapSetEvent;
		}

		private async UniTaskVoid SwitchToPersonGroupAfterEjectAsync(CancellationToken ct)
		{
			if ((bool)_inputMapSetEvent && FlagsStorage.OutOfCar)
			{
				while (_inputMapSetEvent.CurrentInputGroup != _inputMapSetEvent.GameplayRootGroup)
				{
					await UniTask.Yield(PlayerLoopTiming.Update, ct);
				}
				if (FlagsStorage.OutOfCar)
				{
					_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Person, setRoot: true);
				}
			}
		}

		[Inject]
		public void Construct(GarageController garageController, CameraManager cameraManager, FreeDriveProvider freeDriveProvider, CoreGameManager coreGameManager, IGarageSelectorService garageSelectorService, IGarageBuyService garageBuyService)
		{
			_coreGameManager = coreGameManager;
			_freeDriveProvider = freeDriveProvider;
			_cameraManager = cameraManager;
			_garageController = garageController;
			_garageSelectorService = garageSelectorService;
			_garageBuyService = garageBuyService;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_isChecking && (other.CompareTag("Car") || other.CompareTag("OnlineCar")))
			{
				GameObject car = other.transform.parent.gameObject;
				CheckLoadGarage(car).Forget();
			}
		}

		private async UniTaskVoid CheckLoadGarage(GameObject car)
		{
			Vector3 rhs = car.transform.position - base.transform.position;
			VehicleData vehicleData = car.GetComponent<VehicleData>();
			if (Vector3.Dot(base.transform.forward, rhs) > 0f)
			{
				door.Open();
				return;
			}
			if (PhotonNetwork.IsConnected)
			{
				if (!vehicleData.VehicleMembersController.HaveMember(V.DriverPerson))
				{
					return;
				}
				if (_freeDriveProvider.FreeDriveControl.InvitationJustSend || _freeDriveProvider.RacingCar2 != null || FlagsStorage.RaceStarted)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("Detector.CompleteCurrentRace"), 3f);
					return;
				}
				if (car != _freeDriveProvider.FirstCarMultiPlayer)
				{
					if (vehicleData.photonView.IsMine)
					{
						V.CoreGameManager.SetMessage(new LocalizedString("Detector.EnterOnYourOwnCar"), 3f);
						return;
					}
					_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, car);
					SwitchToPersonGroupAfterEjectAsync(this.GetCancellationTokenOnDestroy()).Forget();
					return;
				}
				if (vehicleData.Driver != V.DriverPerson)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("Detector.YouHaveToBeDriver"), 3f);
					return;
				}
			}
			if (!FlagsStorage.CanEnterGarage)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("Detector.LeaveTunnelFirst"), 3f);
				return;
			}
			if (Global.TaskManager.Task != null)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("Detector.CompleteCurrentTask"), 3f);
				return;
			}
			if (vehicleData.Towing || vehicleData.TowingComposition.LoadCar != null)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("Detector.CanTEnterTowingCar"), 3f);
				return;
			}
			_isChecking = true;
			vehicleData.GetPowertrain.ResetTransmission(resetGear: false);
			if (PhotonNetwork.IsConnected)
			{
				for (int i = 0; i < 10; i++)
				{
					if (vehicleData.VehicleMembersController.Members.Count((GameObject x) => x != null) == 1)
					{
						break;
					}
					await Task.Delay(100);
				}
				if (vehicleData.VehicleMembersController.Members.Count((GameObject x) => x != null) > 1)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("Detector.CanTEnterWithPassengers"), 3f);
					_isChecking = false;
					return;
				}
			}
			if (!_garageBuyService.GetGarageStatus(GarageIndex) ? false : true)
			{
				vehicleData.GetPowertrain.Rigidbody.isKinematic = true;
				_garageSelectorService.ChangeGarageIndex(GarageIndex);
				FlagsStorage.IsInGarageTunnel = true;
				if ((bool)V.SceneControl)
				{
					await V.SceneControl.PrepareScenesForLoadGarage();
				}
				_garageController.transform.GetChild(0).gameObject.SetActive(value: false);
				_garageController.SetVisualActive(isActive: true);
				MoveElements._object.MoveGlobalPointOnGarageIn();
				V.CoreGameManager.FreeDriveProvider.QuickCarSettings.HideDoor();
				if (!Resident.IsResidentFor(_garageSelectorService.CurrentSceneIndex) || !Resident.TryActivate())
				{
					await _garageSelectorService.LoadCurrentGarageAsync(_garageController, _cameraManager);
				}
				await _garageController.Init(car);
				vehicleData.GetPowertrain.Rigidbody.isKinematic = false;
				_garageController.SetVisualActive(isActive: false);
				_garageController.transform.GetChild(0).gameObject.SetActive(value: true);
				door.Open();
				_isChecking = false;
			}
		}
	}
}
