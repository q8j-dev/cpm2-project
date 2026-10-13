using System.Collections.Generic;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public class TimeAttackAnalyticHelper
	{
		public const string AttemptNumberKey = "attempt_number_";

		private EventsAnalytic _eventsAnalytic;

		private FreeRaceCar _raceCar;

		private EventRestrictionService _eventRestrictionService;

		public TimeAttackAnalyticHelper(EventsAnalytic eventsAnalytic, EventRestrictionService eventRestrictionService)
		{
			_eventsAnalytic = eventsAnalytic;
			_eventRestrictionService = eventRestrictionService;
		}

		public void SetRaceCar(FreeRaceCar car)
		{
			_raceCar = car;
		}

		public void SendStartRace(int carId, VehicleClass vehicleClass, _Car_Parking.Scripts.EventSystem.Data.Event eventData)
		{
			if (!(_raceCar == null))
			{
				SetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.PROGRESS, eventData.Id);
				TimeAttackAnalyticData analyticData = _raceCar.GetAnalyticData();
				TimeAttackContext timeAttackContext = new TimeAttackContext
				{
					Status = _Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.PROGRESS,
					CarId = carId.ToString(),
					GameEventId = eventData.Id,
					CarClass = vehicleClass.ToString(),
					ModeType = (FlagsStorage.IsMultiFree ? OpenWorldType.Multiplayer : OpenWorldType.Single),
					EventVariant = analyticData.EventVariant,
					AttemptNumber = GetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.PROGRESS, eventData.Id),
					TrackLayoutId = analyticData.TrackLayoutId,
					TrajectoryId = analyticData.TrajectoryId,
					RequiredCarClasses = analyticData.RequiredCarClasses
				};
				_eventsAnalytic.LogPlayerStartRace(timeAttackContext, eventData);
			}
		}

		public void SendRestart(int restartCount)
		{
			if (!(_raceCar == null))
			{
				TimeAttackAnalyticData analyticData = _raceCar.GetAnalyticData();
				TimeAttackContext timeAttackContext = new TimeAttackContext
				{
					Status = _Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.RESTARTED,
					CarId = _raceCar.CarId.ToString(),
					RestartCount = restartCount,
					EventVariant = analyticData.EventVariant,
					CarClass = ((VehicleClass)_raceCar.CarClass/*cast due to constrained. prefix*/).ToString(),
					ModeType = (FlagsStorage.IsMultiFree ? OpenWorldType.Multiplayer : OpenWorldType.Single),
					TrackLayoutId = analyticData.TrackLayoutId,
					TrajectoryId = analyticData.TrajectoryId
				};
				_eventsAnalytic.LogPlayerReStartRace(timeAttackContext);
			}
		}

		public void SendEndRace(int carId, VehicleClass vehicleClass, _Car_Parking.Scripts.EventSystem.Data.Event eventData, float lapTime, bool isSuccessful, bool isRecord, int restartCount, string checkpoints, int checkpointsCount, int missCheckpointsCount, bool finishValid, int validationVersion, float checkpointPenaltyTotal)
		{
			if (!(_raceCar == null))
			{
				SetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.COMPLETED, eventData.Id);
				TimeAttackAnalyticData analyticData = _raceCar.GetAnalyticData();
				analyticData.RequiredValue = GetRequiredValue(eventData, (VehicleClass)_raceCar.CarClass);
				TimeAttackContext timeAttackContext = new TimeAttackContext
				{
					Status = _Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.COMPLETED,
					CarId = carId.ToString(),
					GameEventId = eventData.Id,
					CarClass = vehicleClass.ToString(),
					ModeType = (FlagsStorage.IsMultiFree ? OpenWorldType.Multiplayer : OpenWorldType.Single),
					LapTime = lapTime,
					Ttd_code = analyticData.TtdCode,
					IsSuccessful = isSuccessful,
					IsPersonalRecord = isRecord,
					RestartCount = restartCount,
					RequiredCarClasses = analyticData.RequiredCarClasses,
					RequiredValue = analyticData.RequiredValue,
					CheckPoints = checkpoints,
					CheckpointsCount = checkpointsCount,
					CheckpointMissed = missCheckpointsCount,
					FinishValid = finishValid,
					ValidationRuleCode = validationVersion,
					CheckpointPenaltyTotal = checkpointPenaltyTotal,
					AttemptNumber = GetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.COMPLETED, eventData.Id),
					RewardRecieved = (eventData.Status == EventStatus.Claimed),
					EventVariant = analyticData.EventVariant,
					TrackLayoutId = analyticData.TrackLayoutId,
					TrajectoryId = analyticData.TrajectoryId
				};
				if (eventData.Type == _Car_Parking.Scripts.EventSystem.EventType.DriftTimeAttack && analyticData.DriftData != null)
				{
					timeAttackContext.LapTime = 0f;
					timeAttackContext.DriftScoreTotal = analyticData.DriftData.DriftScoreTotal;
					timeAttackContext.DriftBaseScoreTotal = analyticData.DriftData.DriftBaseScoreTotal;
					timeAttackContext.DriftComboXMax = analyticData.DriftData.DriftComboXMax;
					timeAttackContext.DriftChangeComboCount = analyticData.DriftData.DriftChangeComboCount;
					timeAttackContext.DriftChainBreakCounts = JsonConvert.SerializeObject(analyticData.DriftData.DriftBreakData);
				}
				_eventsAnalytic.LogPlayerEndRace(timeAttackContext, eventData);
			}
		}

		private float GetRequiredValue(_Car_Parking.Scripts.EventSystem.Data.Event eventData, VehicleClass vehicleClass)
		{
			float result = 0f;
			switch (eventData.Type)
			{
			case _Car_Parking.Scripts.EventSystem.EventType.DriftTimeAttack:
				result = ((DriftAttackEventData)eventData.Data)?.Points ?? 0f;
				break;
			case _Car_Parking.Scripts.EventSystem.EventType.TimeAttack:
				result = ((float?)((TimeAttackEventData)eventData.Data)?.LapTime) ?? 0f;
				break;
			case _Car_Parking.Scripts.EventSystem.EventType.TimeAttackNurb:
			{
				TimeAttackNurburgEventData timeAttackNurburgEventData = (TimeAttackNurburgEventData)eventData.Data;
				List<VehicleClass> vehicleClasses = _eventRestrictionService.GetVehicleClasses(eventData.Type);
				if (timeAttackNurburgEventData != null && vehicleClasses != null)
				{
					int num = vehicleClasses.IndexOf(vehicleClass);
					if (num != -1 && num < timeAttackNurburgEventData.MaxLapTimeSet.Length)
					{
						result = timeAttackNurburgEventData.MaxLapTimeSet[num];
					}
				}
				break;
			}
			default:
				result = 0f;
				break;
			}
			return result;
		}

		public void SendClaimReward(_Car_Parking.Scripts.EventSystem.Data.Event eventData, bool rewardRecieved)
		{
			if (!(_raceCar == null))
			{
				TimeAttackAnalyticData analyticData = _raceCar.GetAnalyticData();
				TimeAttackContext timeAttackContext = new TimeAttackContext
				{
					Status = _Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.REWARD_CLAIMED,
					GameEventId = eventData.Id,
					RewardRecieved = rewardRecieved,
					RequiredCarClasses = analyticData.RequiredCarClasses,
					TrackLayoutId = analyticData.TrackLayoutId
				};
				_eventsAnalytic.LogClaimReward(timeAttackContext);
			}
		}

		public void SendCancelRace(_Car_Parking.Scripts.EventSystem.Data.Event eventData, FailReason failReason, string trackName, string trajectoryId, int restartCount)
		{
			SetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.FAILED, eventData.Id);
			TimeAttackContext timeAttackContext = new TimeAttackContext
			{
				Status = _Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.FAILED,
				FailReason = failReason.ToString(),
				GameEventId = eventData.Id,
				TrackLayoutId = trackName,
				TrajectoryId = trajectoryId,
				EventVariant = eventData.Type.ToString(),
				RestartCount = restartCount,
				AttemptNumber = GetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus.FAILED, eventData.Id),
				ModeType = (FlagsStorage.IsMultiFree ? OpenWorldType.Multiplayer : OpenWorldType.Single)
			};
			_eventsAnalytic.LogFailRace(timeAttackContext);
		}

		private void SetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus status, string eventId)
		{
			string key = string.Format("{0}{1}_{2}", "attempt_number_", status, eventId);
			int value = PlayerPrefs.GetInt(key, 0) + 1;
			PlayerPrefs.SetInt(key, value);
		}

		private int GetAttemtNumber(_Car_Parking.Scripts.Analytics.Types.TimeAttackStatus status, string eventId)
		{
			return PlayerPrefs.GetInt(string.Format("{0}{1}_{2}", "attempt_number_", status, eventId), 1);
		}
	}
}
