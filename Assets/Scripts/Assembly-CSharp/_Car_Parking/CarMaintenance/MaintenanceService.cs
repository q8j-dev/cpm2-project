using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.CarMaintenance
{
	public class MaintenanceService : MonoBehaviour
	{
		[Header("Maintenance service setup")]
		[SerializeField]
		[Tooltip("All services that this centre provide")]
		private MaintenanceServiceBaseSO[] _carMaintenanceServices = new MaintenanceServiceBaseSO[0];

		[SerializeField]
		[Tooltip("Zone where car need enter")]
		private TriggerZone _triggerZone;

		[Tooltip("How many seconds should services' execution last")]
		[SerializeField]
		private float _executionDuration = 1f;

		[SerializeField]
		[Tooltip("Final car destination where ends services")]
		private Transform _movePoint;

		[SerializeField]
		[Tooltip("Id of that service (id have to be unique)")]
		private int _id;

		[SerializeField]
		[FormerlySerializedAs("_cylinders")]
		[Tooltip("Visual of the car wash (rotating cylinders, particles, etc.)")]
		private CarWashVisualBase _washVisual;

		public const byte k_triggerZoneEventCode = 1;

		public const byte k_startWashEventCode = 2;

		private const string CarIsCleanLocalizeKey = "MaintenanceService.CarIsClean";

		private GameObject _currentCar;

		private VehicleData _currentVehicleData;

		private float _carMoveSpeed;

		private IPricesService _pricesService;

		private CurrencyManagerController _currencyManagerController;

		private FreeDriveProvider _freeDriveProvider;

		private CarInputManager _carInputManager;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _setActionsActiveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.SetActionsActive);

		public GameObject CurrentCar => _currentCar;

		public VehicleData CurrentVehicleData => _currentVehicleData;

		public float ExecutionDuration => _executionDuration;

		public Transform MoveDestination => _movePoint;

		public int ServiceId => _id;

		[Inject]
		public void Construct(IPricesService pricesService, CurrencyManagerController currencyManagerController, EventRegistry eventRegistry, FreeDriveProvider freeDriveProvider, CarInputManager carInputManager)
		{
			_freeDriveProvider = freeDriveProvider;
			_pricesService = pricesService;
			_currencyManagerController = currencyManagerController;
			_carInputManager = carInputManager;
			_eventRegistry = eventRegistry;
		}

		private void Awake()
		{
			if (_triggerZone == null)
			{
				UnityEngine.Object.Destroy(this);
				throw new UnityException(base.gameObject.name + " is missing trigger zone, deleting script");
			}
		}

		private void OnEnable()
		{
			_triggerZone.OnZoneTriggerred += CheckForAvailability;
			PhotonNetwork.NetworkingClient.EventReceived += OnDeactivateTrigger;
			PhotonNetwork.NetworkingClient.EventReceived += OnStartCarWash;
		}

		private void OnDisable()
		{
			_triggerZone.OnZoneTriggerred -= CheckForAvailability;
			PhotonNetwork.NetworkingClient.EventReceived -= OnDeactivateTrigger;
			PhotonNetwork.NetworkingClient.EventReceived -= OnStartCarWash;
			_setActionsActiveHandle.Get(_eventRegistry)?.Raise(value: true);
		}

		private void OnDestroy()
		{
			_setActionsActiveHandle.Get(_eventRegistry)?.Raise(value: true);
		}

		private void CheckForAvailability(GameObject car)
		{
			_currentCar = car;
			_currentVehicleData = car.GetComponent<VehicleData>();
			bool flag = false;
			ServiceMessages serviceMessages = ServiceMessages.None;
			for (int i = 0; i < _carMaintenanceServices.Length; i++)
			{
				_carMaintenanceServices[i].Init(this);
				flag |= _carMaintenanceServices[i].IsServiceConditionsMet(out var messages);
				serviceMessages |= messages;
			}
			if (flag)
			{
				ExecuteServices();
			}
			else
			{
				CancelServiceWithMessage(serviceMessages);
			}
		}

		private void CancelServiceWithMessage(ServiceMessages messages)
		{
			if (messages.HasFlag(ServiceMessages.NotEnoughMoney))
			{
				_freeDriveProvider.FreeDriveControl.ShowMoneyPurchaseMenu2();
			}
			else
			{
				V.CoreGameManager.SetMessage(new LocalizedString("MaintenanceService.CarIsClean"), 3f);
			}
		}

		private void ExecuteServices()
		{
			StopCar();
			if (PhotonNetwork.IsConnected)
			{
				object[] eventContent = new object[1] { _id };
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.All
				};
				PhotonNetwork.RaiseEvent(1, eventContent, raiseEventOptions, SendOptions.SendReliable);
			}
			else
			{
				DeactivateTriggerZoneRoutineAsync().Forget();
			}
			TemporaryDisableControlRoutineAsync().Forget();
			for (int i = 0; i < _carMaintenanceServices.Length; i++)
			{
				if (_carMaintenanceServices[i].IsServiceConditionsMet(out var _))
				{
					_carMaintenanceServices[i].Execute(_pricesService, _currencyManagerController, _freeDriveProvider.FreeDriveDB);
				}
			}
		}

		private async UniTask TemporaryDisableControlRoutineAsync()
		{
			CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
			UpdateControllerIsEnabled(isEnabled: false);
			_setActionsActiveHandle.Get(_eventRegistry).Raise(value: false);
			_freeDriveProvider.FreeDriveControl.DisableToRoadButton();
			if (!(await UniTask.Delay(TimeSpan.FromSeconds(_executionDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy).SuppressCancellationThrow()))
			{
				UpdateControllerIsEnabled(isEnabled: true);
				_setActionsActiveHandle.Get(_eventRegistry).Raise(value: true);
				if ((bool)_freeDriveProvider?.FreeDriveControl)
				{
					_freeDriveProvider.FreeDriveControl.EnableToRoadButton();
				}
			}
		}

		private void StopCar()
		{
			_currentVehicleData.StopCarCompletely(_executionDuration * 0.5f);
		}

		private void UpdateControllerIsEnabled(bool isEnabled)
		{
			_carInputManager.EnableControl(isEnabled, !isEnabled);
		}

		private void OnDeactivateTrigger(EventData photonEvent)
		{
			if (photonEvent.Code == 1 && (int)((object[])photonEvent.CustomData)[0] == _id)
			{
				DeactivateTriggerZoneRoutineAsync().Forget();
			}
		}

		private async UniTask DeactivateTriggerZoneRoutineAsync()
		{
			CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
			if ((bool)_triggerZone)
			{
				_triggerZone.gameObject.SetActive(value: false);
				if (!(await UniTask.Delay(TimeSpan.FromSeconds(_executionDuration * 2f), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy).SuppressCancellationThrow()) && (bool)_triggerZone)
				{
					_triggerZone.gameObject.SetActive(value: true);
				}
			}
		}

		private void OnStartCarWash(EventData photonEvent)
		{
			if (photonEvent.Code == 2)
			{
				object[] array = (object[])photonEvent.CustomData;
				if ((int)array[0] == _id)
				{
					StartCarWash((int)array[1]);
				}
			}
		}

		public void StartCarWash(int viewId = -1)
		{
			if (PhotonNetwork.IsConnected)
			{
				_currentCar = PhotonView.Find(viewId).gameObject;
			}
			if (!(_washVisual == null))
			{
				_washVisual.PlayAsync(_currentCar, _executionDuration, this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		public void ExecuteDirtReduction(int spentMoney)
		{
			ReduceDirtRoutineAsync(spentMoney, this.GetCancellationTokenOnDestroy()).Forget();
		}

		public void ShowWashCompletedMessage(int spentMoney)
		{
			LocalizedString localizedString = ((spentMoney == 0) ? new LocalizedString("MaintenanceService.CarIsClean") : new LocalizedString("MaintenanceService.CarIsClean", (-spentMoney).ToString(), newLineRequired: true));
			V.CoreGameManager.SetMessage(localizedString, 3f);
		}

		private async UniTask ReduceDirtRoutineAsync(int spentMoney, CancellationToken ct = default(CancellationToken))
		{
			try
			{
				float currentDuration = _executionDuration * 0.75f;
				float num = Time.deltaTime * 10f;
				float reductionSpeed = currentDuration * num;
				float _dirtReductionValue = 0.95f * num;
				while (currentDuration > 0f)
				{
					_currentVehicleData.DirtComposition.ReduceCarDirtIntensity(_dirtReductionValue);
					currentDuration -= reductionSpeed;
					await UniTask.Delay(TimeSpan.FromSeconds(reductionSpeed), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
				ShowWashCompletedMessage(spentMoney);
			}
			catch (OperationCanceledException)
			{
			}
		}
	}
}
