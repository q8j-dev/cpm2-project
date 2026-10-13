using System.Collections.Generic;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	public class TimeAttackEventController
	{
		private EventRestrictionService _eventRestrictionService;

		private IEventService _eventService;

		private int _eventBeatCount = -1;

		private TimeBannersManager _timeBannersManager;

		private FreeTimeAttackController _timeAttackController;

		private UiNotificationsManager _uiNotificationsManager;

		private FreeRaceCar _raceCar;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(EventRestrictionService eventRestrictionService, TimeBannersManager timeBannersManager, FreeTimeAttackController freeTimeAttackController, UiNotificationsManager uiNotificationsManager, IEventService eventService)
		{
			_eventRestrictionService = eventRestrictionService;
			_eventService = eventService;
			_timeBannersManager = timeBannersManager;
			_timeAttackController = freeTimeAttackController;
			_uiNotificationsManager = uiNotificationsManager;
		}

		public void SetRaceCar(FreeRaceCar car)
		{
			_raceCar = car;
		}

		public async UniTaskVoid SendEventData(float result, float cachedBestResult, int classId, float checkpointPenaltyTotal)
		{
			TrySendFinishRaceAnalytics(result, cachedBestResult > result || cachedBestResult == 0f, checkpointPenaltyTotal);
			if (_timeAttackController.CurrentTimeAttackData == null)
			{
				return;
			}
			bool num = _eventRestrictionService.IsVehicleAllowed(_timeAttackController.CurrentTimeAttackData.EventType, (VehicleClass)classId);
			Event eventData = _eventService.GetEventByName(_timeAttackController.CurrentTimeAttackData.EventType);
			_eventBeatCount = eventData?.Progress ?? (-1);
			if (num && eventData != null)
			{
				await _eventService.SubmitEventRecord(_timeAttackController.CurrentTimeAttackData.EventType, GetTimeAttackPayload(eventData.Type, result, classId));
				await _eventService.RetrieveCurrentEvents(forceRefresh: true);
				if (eventData.Progress > _eventBeatCount)
				{
					ShowAdvancedPopup(eventData, classId);
					ShowEventBeatPopup(classId);
				}
			}
		}

		private void ShowAdvancedPopup(Event eventData, int classId)
		{
			if (eventData.Type == EventType.TimeAttackNurb)
			{
				PopupData popupData = new PopupData();
				popupData.PopupKey = "TimeAttackCorrectClassPopup";
				string arg = new LocalizedString("Newcarswindow.Class");
				popupData.Title = $"{arg}: <color=green> {(VehicleClass)classId}";
				popupData.AudioKey = SoundEventsDataHelper.UITimeAttackBestResultShowSwitch;
				_uiNotificationsManager.ShowPopup(popupData);
			}
		}

		private PayloadData GetTimeAttackPayload(EventType mode, float result, int classId)
		{
			return mode switch
			{
				EventType.TimeAttack => new RaceData
				{
					LapTime = result,
					CarClass = classId
				}, 
				EventType.DriftTimeAttack => new DriftData
				{
					Points = result,
					CarClass = classId
				}, 
				EventType.TimeAttackNurb => new NurbRaceData
				{
					LapTime = result,
					CarClass = classId
				}, 
				_ => null, 
			};
		}

		private (string, bool) GetProgress(Event @event)
		{
			string text = "";
			bool flag = false;
			if (@event.Type == EventType.DriftTimeAttack)
			{
				DriftAttackEventData driftAttackEventData = (DriftAttackEventData)@event.Data;
				RaceBestData bestTime = _timeAttackController.GetBestTime(_timeAttackController.CurrentTrackIndex, TimeAttackMode.Drift);
				text = string.Format("{0} {1}", new LocalizedString("Event.ShowDriftPointsPopup"), driftAttackEventData.Points);
				flag = bestTime.BestResult >= driftAttackEventData.Points;
			}
			else
			{
				text = string.Format("{0} {1} / {2}", new LocalizedString("Userbalancewindow.Progress"), @event.Progress, @event.MaxProgress);
				flag = @event.Progress >= @event.MaxProgress;
			}
			return (text, flag);
		}

		private bool IsSuccessful(Event @event)
		{
			bool flag = false;
			if (@event.Type == EventType.DriftTimeAttack)
			{
				DriftAttackEventData driftAttackEventData = (DriftAttackEventData)@event.Data;
				return _timeAttackController.GetBestTime(_timeAttackController.CurrentTrackIndex, TimeAttackMode.Drift).BestResult >= driftAttackEventData.Points;
			}
			return @event.Progress >= @event.MaxProgress;
		}

		public void ShowEventBeatPopup(int carClass)
		{
			Event eventByName = _eventService.GetEventByName(_timeAttackController.CurrentTimeAttackData.EventType);
			if (eventByName != null && _eventRestrictionService.IsVehicleAllowed(eventByName.Type, (VehicleClass)carClass))
			{
				_timeBannersManager.GetTimeBannersManager(_timeAttackController.CurrentTrackIndex).InitHorizontalBanner(string.Format("{0} {1}/{2}", new LocalizedString("EventViewWindow.Qualification"), eventByName.Progress, eventByName.MaxProgress), BannerContentType.EventQualification);
				ShowEventPopup(eventByName);
				if (eventByName.Status == EventStatus.Finished)
				{
					_eventService.RemoveCacheEventByName(eventByName.Type);
				}
			}
		}

		private void ShowEventPopup(Event @event)
		{
			(string, bool) progress = GetProgress(@event);
			bool item = progress.Item2;
			PopupData popupData = new PopupData();
			popupData.PopupKey = (item ? "TimeBeatPopup" : "TimeNotBeatPopup");
			popupData.Title = progress.Item1;
			popupData.AudioKey = (item ? SoundEventsDataHelper.UITimeAttackBestResultShowSwitch : SoundEventsDataHelper.UITimeAttackDefaultResultShowSwitch);
			_uiNotificationsManager.ShowPopup(popupData);
			if (item)
			{
				EventViewWindow.AdditionalData value = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(@event.Type));
				PopupData popupData2 = new PopupData();
				popupData2.PopupKey = "ButtonPopup";
				popupData2.Title = new LocalizedString("EventButton_Reward");
				popupData2.AudioKey = SoundEventsDataHelper.UITimeAttackEventCanClaimSwitch;
				popupData2.NextWindowData = new NextWindowData("EventViewWindow", JsonConvert.SerializeObject(value));
				_uiNotificationsManager.ShowPopup(popupData2);
			}
		}

		public void TrySendStartRaceAnalytics()
		{
			if (_timeAttackController.CurrentTimeAttackData != null)
			{
				Event eventByName = _eventService.GetEventByName(_timeAttackController.CurrentTimeAttackData.EventType);
				if (eventByName != null && _eventRestrictionService.IsVehicleAllowed(eventByName.Type, (VehicleClass)_raceCar.CarClass))
				{
					_timeAttackController.TimeAttackAnalyticHelper.SendStartRace(_raceCar.CarId, (VehicleClass)_raceCar.CarClass, eventByName);
				}
			}
		}

		public void TrySendRestartRaceAnalytics()
		{
			if (_timeAttackController.CurrentTimeAttackData != null)
			{
				Event eventByName = _eventService.GetEventByName(_timeAttackController.CurrentTimeAttackData.EventType);
				if (eventByName != null && _eventRestrictionService.IsVehicleAllowed(eventByName.Type, (VehicleClass)_raceCar.CarClass))
				{
					_timeAttackController.TimeAttackAnalyticHelper.SendRestart(_timeAttackController.RestartCount);
				}
			}
		}

		public void TrySendFinishRaceAnalytics(float time, bool isRecord, float penaltyTotal)
		{
			if (_timeAttackController.CurrentTimeAttackData == null)
			{
				return;
			}
			Event eventByName = _eventService.GetEventByName(_timeAttackController.CurrentTimeAttackData.EventType);
			if (eventByName != null)
			{
				bool isSuccessful = IsSuccessful(eventByName);
				List<CheckpointData> allCheckpointsData = _timeAttackController.RaceTriggersValidator.GetAllCheckpointsData(_timeAttackController.CurrentTrackIndex, _timeAttackController.CurrentTrajectoryIndex);
				string checkpoints = JsonConvert.SerializeObject(allCheckpointsData);
				int missCheckpointsCount = 0;
				_timeAttackController.RaceTriggersValidator.GetMissingCheckpointsCount(_timeAttackController.CurrentTrackIndex, _timeAttackController.CurrentTrajectoryIndex, out missCheckpointsCount, out var _);
				_timeAttackController.TimeAttackAnalyticHelper.SendEndRace(_raceCar.CarId, (VehicleClass)_raceCar.CarClass, eventByName, time, isSuccessful, isRecord, _timeAttackController.RestartCount, checkpoints, allCheckpointsData.Count, missCheckpointsCount, missCheckpointsCount < 3, 3, penaltyTotal);
				if (eventByName.Status == EventStatus.Finished)
				{
					_timeAttackController.TimeAttackAnalyticHelper.SendClaimReward(eventByName, rewardRecieved: false);
				}
			}
		}

		public void OnFailedRace(FailReason failReason)
		{
			if (_timeAttackController.CurrentTimeAttackData != null)
			{
				Event eventByName = _eventService.GetEventByName(_timeAttackController.CurrentTimeAttackData.EventType);
				if (eventByName != null)
				{
					string trackName = $"{_timeAttackController.CurrentTimeAttackData.EventType}_{_timeAttackController.CurrentTrackIndex}";
					_timeAttackController.TimeAttackAnalyticHelper.SendCancelRace(eventByName, failReason, trackName, _timeAttackController.CurrentTrajectoryIndex.ToString(), _timeAttackController.RestartCount);
				}
			}
		}
	}
}
