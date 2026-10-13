using System;
using System.Linq;
using CPM.PlayerData;
using CPMEngine;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;

namespace Car_Parking.Scripts.Racing
{
	public class FreeRaceCar : MonoBehaviour, IRaceCar
	{
		private Powertrain _powertrain;

		private bool _isRaceStarted;

		private int _carId;

		private int _carClass;

		public Action<string, float, int, TimeAttackMode> UserFinishedRace;

		public Action UserStartRace;

		public Action<float, float, float> UserTimeAction;

		public Action<FailReason> UserExitRace;

		private PlayerDataStorage _playerDataStorage;

		private FreeTimeAttackController _freeTimeAttackController;

		private Rigidbody _rigidbody;

		private IRaceResultCounter _raceResultCounter;

		private IPrefabFactory _prefabFactory;

		public TimeAttackMode TimeAttackMode;

		private CarInputManager _carInputManager;

		public int CarClass => _carClass;

		public int CarId => _carId;

		public int CurrentTriggerIndex { get; set; } = -1;

		public float UsersTime => _raceResultCounter.GetCurrentResult();

		public bool IsRaceStarted => _isRaceStarted;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage, FreeTimeAttackController freeTimeAttackController, IPrefabFactory prefabFactory, CarInputManager carInputManager)
		{
			_playerDataStorage = playerDataStorage;
			_freeTimeAttackController = freeTimeAttackController;
			_prefabFactory = prefabFactory;
			_carInputManager = carInputManager;
		}

		private void Start()
		{
			_powertrain = GetComponent<Powertrain>();
			_rigidbody = GetComponent<Rigidbody>();
			VehicleEntityProvider component = GetComponent<VehicleEntityProvider>();
			_carClass = (int)Utils.GetClassVehicle(component.CarEntity.PerformancePoints, component.GetVehicleData.CarId);
			_carId = component.GetVehicleData.CarId;
		}

		public void InitTimeAttackMode(TimeAttackMode mode)
		{
			TimeAttackMode = mode;
			if (mode == TimeAttackMode.Drift)
			{
				_prefabFactory.InstantiateComponent<DriftController>(base.gameObject);
				DriftAttackResultCounter raceResultCounter = _prefabFactory.Create<DriftAttackResultCounter>(Array.Empty<object>());
				_raceResultCounter = raceResultCounter;
			}
			else
			{
				TimeAttackResultCounter raceResultCounter2 = _prefabFactory.Create<TimeAttackResultCounter>(Array.Empty<object>());
				_raceResultCounter = raceResultCounter2;
			}
			_raceResultCounter.Init(this);
		}

		public void StopRace(TimeAttackStatus[] triggerStatuses)
		{
			if (triggerStatuses != null && Enumerable.Contains(triggerStatuses, _freeTimeAttackController.RaceStatus))
			{
				UserExitRace?.Invoke(FailReason.LEAVE_DIRECTION);
				UnityEngine.Object.Destroy(this);
			}
		}

		public void UserWrongWay()
		{
			UserExitRace?.Invoke(FailReason.DISQUALIFIED);
			UnityEngine.Object.Destroy(this);
		}

		public TimeAttackAnalyticData GetAnalyticData()
		{
			return _raceResultCounter.GetAnalyticData();
		}

		public void StartRace()
		{
			_powertrain.FixVehicle(stop: false);
		}

		public void ResetRigidbody()
		{
			if ((bool)_rigidbody)
			{
				_rigidbody.velocity = Vector3.zero;
				_rigidbody.angularVelocity = Vector3.zero;
			}
		}

		private float GetFinalResult()
		{
			return _raceResultCounter.GetFinalResult();
		}

		public void OnCrossFinishLane()
		{
			if (_isRaceStarted)
			{
				FinishRace();
				return;
			}
			UserStartRace?.Invoke();
			_raceResultCounter.OnStartRace();
			_isRaceStarted = true;
		}

		public void FinishRace()
		{
			_isRaceStarted = false;
			UserFinishedRace?.Invoke(_playerDataStorage.PlayerID, GetFinalResult(), _carClass, TimeAttackMode);
			UnityEngine.Object.Destroy(this);
		}

		public void RestartRace()
		{
			CurrentTriggerIndex = -1;
			_isRaceStarted = false;
			_raceResultCounter.Restart();
			ResetRigidbody();
		}

		public void FreezeCar(bool state)
		{
			if (!_powertrain)
			{
				_powertrain = GetComponent<Powertrain>();
				_powertrain.ResetTransmission(resetGear: false);
			}
			_powertrain.FixVehicle(state);
		}

		private void OnDestroy()
		{
			_raceResultCounter?.Dispose();
			_powertrain.FixVehicle(stop: false);
		}

		public void OnCarToRoad(GameObject raceCarGameObject)
		{
			_raceResultCounter.OnCarToRoad(raceCarGameObject);
		}

		public void ToRoad()
		{
			if ((bool)_powertrain)
			{
				_powertrain.ResetTransmission(resetGear: false);
			}
			if ((bool)_carInputManager)
			{
				_carInputManager.ClearAllInputs();
			}
		}
	}
}
