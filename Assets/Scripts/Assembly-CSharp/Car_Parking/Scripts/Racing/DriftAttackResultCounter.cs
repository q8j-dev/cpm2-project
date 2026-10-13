using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;

namespace Car_Parking.Scripts.Racing
{
	public class DriftAttackResultCounter : IRaceResultCounter, IDisposable
	{
		private const int DriftComboTarget = 10;

		private FreeTimeAttackController _freeTimeAttackController;

		private EventRestrictionService _eventRestrictionService;

		private DriftController _driftController;

		private FreeRaceCar _freeRaceCar;

		private MultiPlayerPosControl _multiPlayerPosControl;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private Collider _collider;

		private ISteamAchievementService _steamAchievementService;

		private VehicleData _vehicleData;

		private static int EffectiveDriftComboTarget => 10;

		[UnityEngine.Scripting.Preserve]
		[Inject]
		public void Construct(FreeDriveControl freeDriveControl, FreeTimeAttackController timeAttackController, EventRestrictionService eventRestrictionService, OnlineCollectionsProvider onlineCollectionsProvider, ISteamAchievementService steamAchievementService)
		{
			_freeTimeAttackController = timeAttackController;
			_eventRestrictionService = eventRestrictionService;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_steamAchievementService = steamAchievementService;
		}

		public void OnStartRace()
		{
		}

		public void Init(FreeRaceCar freeRaceCar)
		{
			if (freeRaceCar.TryGetComponent<VehicleData>(out var component))
			{
				_vehicleData = component;
				_collider = component.Collider;
			}
			PersonMultiplayerDataSending.OnPersonDataApplied += OnAnotherUserConnected;
			_freeRaceCar = freeRaceCar;
			_driftController = freeRaceCar.GetComponent<DriftController>();
			_driftController.enabled = false;
			freeRaceCar.UserStartRace = (Action)Delegate.Combine(freeRaceCar.UserStartRace, new Action(ActivateDriftController));
			FreeTimeAttackController freeTimeAttackController = _freeTimeAttackController;
			freeTimeAttackController.OnRaceFinish = (Action<bool, float, TimeAttackMode>)Delegate.Combine(freeTimeAttackController.OnRaceFinish, new Action<bool, float, TimeAttackMode>(OnSteamRaceFinish));
			_multiPlayerPosControl = freeRaceCar.GetComponent<MultiPlayerPosControl>();
			_multiPlayerPosControl.ForceIgnoreCollisions = true;
			FreeTimeAttackController freeTimeAttackController2 = _freeTimeAttackController;
			freeTimeAttackController2.OnRaceFinish = (Action<bool, float, TimeAttackMode>)Delegate.Combine(freeTimeAttackController2.OnRaceFinish, new Action<bool, float, TimeAttackMode>(OnRaceFinishForAchievement));
		}

		private void OnRaceFinishForAchievement(bool success, float time, TimeAttackMode mode)
		{
			if (success && mode == TimeAttackMode.Drift && _driftController != null && _driftController.DriftAnalyticData != null && _driftController.DriftAnalyticData.DriftComboXMax >= EffectiveDriftComboTarget)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQNA", "drift_time_attack");
			}
		}

		private void OnAnotherUserConnected(MultiPersonControl mpc)
		{
			if ((bool)mpc && (bool)mpc.FirstAndOwnCar && (bool)_collider && mpc.FirstAndOwnCar.TryGetComponent<VehicleData>(out var component))
			{
				CarColliders.SetIgnore(_vehicleData, component, ignore: true);
			}
		}

		private void ActivateDriftController()
		{
			_driftController.enabled = true;
		}

		public float GetFinalResult()
		{
			_driftController.ForceEarnPoints();
			return _driftController.TotalDriftPoints;
		}

		public float GetCurrentResult()
		{
			return _driftController.TotalDriftPoints;
		}

		public void Restart()
		{
			_driftController.enabled = false;
			_driftController.ResetDrift();
		}

		public void OnCarToRoad(GameObject raceCarGameObject)
		{
			_driftController.StopDrift();
		}

		public TimeAttackAnalyticData GetAnalyticData()
		{
			TimeAttackAnalyticData timeAttackAnalyticData = new TimeAttackAnalyticData();
			timeAttackAnalyticData.EventVariant = _freeTimeAttackController.CurrentTimeAttackData.EventType.ToString();
			timeAttackAnalyticData.TrackLayoutId = $"{_freeTimeAttackController.CurrentTimeAttackData.EventType}_{_freeTimeAttackController.CurrentTrackIndex}";
			timeAttackAnalyticData.TrajectoryId = _freeTimeAttackController.CurrentTrajectoryIndex.ToString();
			timeAttackAnalyticData.DriftData = _driftController.DriftAnalyticData;
			List<VehicleClass> vehicleClasses = _eventRestrictionService.GetVehicleClasses(_freeTimeAttackController.CurrentTimeAttackData.EventType);
			if (vehicleClasses != null)
			{
				string requiredCarClasses = string.Join(",", vehicleClasses);
				timeAttackAnalyticData.RequiredCarClasses = requiredCarClasses;
			}
			return timeAttackAnalyticData;
		}

		public void Dispose()
		{
			PersonMultiplayerDataSending.OnPersonDataApplied -= OnAnotherUserConnected;
			_multiPlayerPosControl.ForceIgnoreCollisions = false;
			if ((bool)_multiPlayerPosControl && _multiPlayerPosControl.ignoreCollision)
			{
				PlayerDatas.IgnoreCollisionWithOtherCars(_freeRaceCar.gameObject, ignore: false, _onlineCollectionsProvider, out var _);
				_multiPlayerPosControl.ignoreCollision = false;
			}
			_driftController.DestroyComponent();
			FreeRaceCar freeRaceCar = _freeRaceCar;
			freeRaceCar.UserStartRace = (Action)Delegate.Remove(freeRaceCar.UserStartRace, new Action(ActivateDriftController));
			FreeTimeAttackController freeTimeAttackController = _freeTimeAttackController;
			freeTimeAttackController.OnRaceFinish = (Action<bool, float, TimeAttackMode>)Delegate.Remove(freeTimeAttackController.OnRaceFinish, new Action<bool, float, TimeAttackMode>(OnRaceFinishForAchievement));
			FreeTimeAttackController freeTimeAttackController2 = _freeTimeAttackController;
			freeTimeAttackController2.OnRaceFinish = (Action<bool, float, TimeAttackMode>)Delegate.Remove(freeTimeAttackController2.OnRaceFinish, new Action<bool, float, TimeAttackMode>(OnSteamRaceFinish));
		}

		private void OnSteamRaceFinish(bool success, float time, TimeAttackMode mode)
		{
			if (success && mode == TimeAttackMode.Drift && _driftController != null && _driftController.DriftAnalyticData != null && _driftController.DriftAnalyticData.DriftComboXMax >= 10)
			{
				_steamAchievementService?.Unlock("ACH_SIDEWAYS_SCIENCE");
			}
		}
	}
}
