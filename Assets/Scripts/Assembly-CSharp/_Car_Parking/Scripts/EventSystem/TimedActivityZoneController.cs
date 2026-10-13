using System;
using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.EventSystem
{
	public class TimedActivityZoneController : IInitializable, IDisposable
	{
		private const int ZonesCount = 18;

		public Action<bool> OnCurrentZoneExit;

		public Action UpdateMapState;

		private Dictionary<SceneType, List<EventActivityModel>> _eventZones = new Dictionary<SceneType, List<EventActivityModel>>();

		private ActivityZoneTrigger _currentPlayerZone;

		private IDisposable _zonesTimers;

		private EventZoneTimerComponent _timerComponent;

		private EventZoneTimerComponent _penaltyTimerComponent;

		private CPMGraphicsSettings.ColorCorrectionLut _defaultColorCorrectionLut;

		private ManagerUI _managerUI;

		private ZoneParticleMover _zoneParticleMover;

		private IEventUIWrapper _uiEventUIWrapper;

		private UiNotificationsManager _uiNotificationsManager;

		private IEventService _eventService;

		private Dictionary<int, int> _zonesProgress = new Dictionary<int, int>();

		private bool _isDisposed;

		private bool _isActivityStarted;

		private EventsAnalytic _eventsAnalytic;

		public EventZoneTimerComponent TimerComponent => _timerComponent;

		public EventZoneTimerComponent PenaltyTimerComponent => _penaltyTimerComponent;

		public Dictionary<int, EventActivityModel> AllEventActivityModel { get; private set; } = new Dictionary<int, EventActivityModel>();

		[Inject]
		[UnityEngine.Scripting.Preserve]
		private void Construct(ManagerUI managerUI, UiNotificationsManager notificationsManager, IEventService eventService, EventsAnalytic eventsAnalytic)
		{
			_eventsAnalytic = eventsAnalytic;
			_managerUI = managerUI;
			_uiNotificationsManager = notificationsManager;
			_eventService = eventService;
		}

		public void DeactivateZone(ActivityZoneTrigger trigger)
		{
			EventActivityModel model = GetModel(trigger.ZoneIndex, trigger.SceneType);
			if (model != null)
			{
				model.MapZone.ChangeState(ZoneState.Cooldown);
				trigger.DelayedResetCollectibles();
			}
		}

		public void ContinueActivity()
		{
			_penaltyTimerComponent.Reset();
			_timerComponent.Resume();
			OnCurrentZoneExit?.Invoke(obj: false);
			ChangeFilmEffect(_defaultColorCorrectionLut);
		}

		public (int, int) GetCurrentProgress()
		{
			(int, int) result = (0, 0);
			if (_currentPlayerZone != null)
			{
				return (_currentPlayerZone.CollectiblesCount, _currentPlayerZone.MaxCollectiblesCount);
			}
			return result;
		}

		public void StartPenaltyTimer()
		{
			OnCurrentZoneExit?.Invoke(obj: true);
			_timerComponent.Pause();
			_penaltyTimerComponent.Start();
			ChangeFilmEffect(CPMGraphicsSettings.ColorCorrectionLut.Carter);
			SendLogExit();
		}

		private void ChangeFilmEffect(CPMGraphicsSettings.ColorCorrectionLut ColorCorrectionLut)
		{
			CPMGraphicsSettingsManager.Instance.UseColorCorrectionLut = ColorCorrectionLut;
		}

		public void Dispose()
		{
			_isDisposed = true;
			_zonesTimers?.Dispose();
			_penaltyTimerComponent?.Dispose();
			EventZoneTimerComponent penaltyTimerComponent = _penaltyTimerComponent;
			penaltyTimerComponent.OnTimerFinished = (Action)Delegate.Remove(penaltyTimerComponent.OnTimerFinished, new Action(ActivityLeaveTimeExpired));
			EventZoneTimerComponent timerComponent = _timerComponent;
			timerComponent.OnTimerFinished = (Action)Delegate.Remove(timerComponent.OnTimerFinished, new Action(ActivityTimeExpired));
			_timerComponent?.Dispose();
			UnsubscribeActions();
			ChangeFilmEffect(_defaultColorCorrectionLut);
		}

		public void Initialize()
		{
			int defaultColorCorrectionLut = PlayerPrefs.GetInt("colorCorrection", 0);
			_defaultColorCorrectionLut = (CPMGraphicsSettings.ColorCorrectionLut)defaultColorCorrectionLut;
			_timerComponent = new EventZoneTimerComponent(this);
			_penaltyTimerComponent = new EventZoneTimerComponent(this);
			_uiEventUIWrapper = new SnowmanEventPopupController(_uiNotificationsManager, this, _managerUI);
			EventZoneTimerComponent timerComponent = _timerComponent;
			timerComponent.OnTimerFinished = (Action)Delegate.Combine(timerComponent.OnTimerFinished, new Action(ActivityTimeExpired));
			EventZoneTimerComponent penaltyTimerComponent = _penaltyTimerComponent;
			penaltyTimerComponent.OnTimerFinished = (Action)Delegate.Combine(penaltyTimerComponent.OnTimerFinished, new Action(ActivityLeaveTimeExpired));
			UpdateProgress();
		}

		private void OnCleanUiScreen(bool state)
		{
			_uiEventUIWrapper.ShowHud(!state);
		}

		private void ActivityLeaveTimeExpired()
		{
			_isActivityStarted = false;
			_uiEventUIWrapper.ShowHud(state: false);
			ShowResult(state: false);
			DeactivateZone(_currentPlayerZone);
			ChangeFilmEffect(_defaultColorCorrectionLut);
			SendLogFail();
			ResetCurrentZoneState();
			UnsubscribeActions();
		}

		private void ActivityTimeExpired()
		{
			_uiEventUIWrapper.ShowHud(state: false);
			ShowResult(state: false);
			_isActivityStarted = false;
			SendLogFail();
			DeactivateZone(_currentPlayerZone);
			ResetCurrentZoneState();
			UnsubscribeActions();
		}

		private void ShowResult(bool state)
		{
			_uiEventUIWrapper.ShowMessage(state, Utils.ConvertToTime(_timerComponent.CurrentTime) ?? "");
			_uiEventUIWrapper.ShowResult(state, _currentPlayerZone.CollectiblesCount, _currentPlayerZone.MaxCollectiblesCount);
		}

		public void OnZoneEntered()
		{
			_uiEventUIWrapper.ShowStartMessage();
		}

		private void ResetCurrentZoneState()
		{
			_currentPlayerZone = null;
			_timerComponent.Reset();
			_penaltyTimerComponent.Reset();
		}

		private EventActivityModel GetModel(int index, SceneType sceneType)
		{
			if (_eventZones.ContainsKey(sceneType))
			{
				EventActivityModel eventActivityModel = ((_eventZones[sceneType] != null) ? _eventZones[sceneType].FirstOrDefault((EventActivityModel m) => m.Index == index) : null);
				if (eventActivityModel == null)
				{
					return null;
				}
				return eventActivityModel;
			}
			return null;
		}

		private EventActivityModel RegisterInternal(int index, SceneType sceneType, ActivityZoneTrigger trigger = null, EventZoneMapElement map = null)
		{
			if (trigger != null)
			{
				trigger.gameObject.SetActive(value: true);
			}
			if (map != null)
			{
				map.gameObject.SetActive(value: true);
			}
			if (!_eventZones.TryGetValue(sceneType, out var value))
			{
				value = new List<EventActivityModel>();
				_eventZones.Add(sceneType, value);
			}
			EventActivityModel eventActivityModel = GetModel(index, sceneType);
			if (eventActivityModel == null)
			{
				eventActivityModel = new EventActivityModel
				{
					Index = index,
					Trigger = trigger,
					MapZone = map
				};
				value.Add(eventActivityModel);
				AllEventActivityModel.TryAdd(index, eventActivityModel);
			}
			else
			{
				if (trigger != null)
				{
					eventActivityModel.Trigger = trigger;
				}
				if (map != null)
				{
					eventActivityModel.MapZone = map;
				}
			}
			if (eventActivityModel.Trigger != null && eventActivityModel.MapZone != null)
			{
				eventActivityModel.Trigger.MapModel = eventActivityModel.MapZone;
			}
			return eventActivityModel;
		}

		public void RegisterParticle(ZoneParticleMover particleMover)
		{
			if (!_zoneParticleMover)
			{
				_zoneParticleMover = particleMover;
			}
		}

		public void RegisterZone(ActivityZoneTrigger trigger)
		{
			RegisterInternal(trigger.ZoneIndex, trigger.SceneType, trigger);
		}

		public async UniTaskVoid RegisterZoneOnMap(EventZoneMapElement map)
		{
			await UniTask.Yield();
			RegisterInternal(map.Index, map.SceneType, null, map);
		}

		private void SubscribeActions()
		{
			GlobalEventSystem.OnCleanUI = (Action<bool>)Delegate.Remove(GlobalEventSystem.OnCleanUI, new Action<bool>(OnCleanUiScreen));
			GlobalEventSystem.OnCleanUI = (Action<bool>)Delegate.Combine(GlobalEventSystem.OnCleanUI, new Action<bool>(OnCleanUiScreen));
		}

		private void UnsubscribeActions()
		{
			GlobalEventSystem.OnCleanUI = (Action<bool>)Delegate.Remove(GlobalEventSystem.OnCleanUI, new Action<bool>(OnCleanUiScreen));
		}

		public void SubscribeZone(ActivityZoneTrigger trigger)
		{
			SubscribeActions();
			_isActivityStarted = true;
			_currentPlayerZone = trigger;
			_uiEventUIWrapper.ShowHud(state: true);
			StartPlay(trigger);
			StartTimer();
			LogBegin();
		}

		private void StartTimer()
		{
			_penaltyTimerComponent.Reset();
			_timerComponent.Start();
		}

		public void StartPlay(ActivityZoneTrigger trigger)
		{
			EventActivityModel model = GetModel(trigger.ZoneIndex, trigger.SceneType);
			if (model != null && (bool)model.MapZone)
			{
				_timerComponent.Init(trigger.TargetPassTime, 67);
				_penaltyTimerComponent.Init(trigger.TargetPenaltyTime);
			}
		}

		public void FinishActivity()
		{
			UnsubscribeActions();
			if ((bool)_currentPlayerZone)
			{
				SendEventData(_currentPlayerZone.ZoneIndex).Forget();
				_uiEventUIWrapper.ShowHud(state: false);
				_isActivityStarted = false;
				ShowResult(state: true);
				DeactivateZone(_currentPlayerZone);
				SendLogSuccess();
				ResetCurrentZoneState();
			}
		}

		public void ShowZoneParticle(bool state)
		{
			if ((bool)_zoneParticleMover)
			{
				_zoneParticleMover.ChangeState(state);
			}
		}

		public void OnDestroy()
		{
		}

		public bool CheckZoneProgress(int index)
		{
			if (_zonesProgress.ContainsKey(index))
			{
				return _zonesProgress[index] == 1;
			}
			return false;
		}

		private void UpdateProgress(_Car_Parking.Scripts.EventSystem.Data.Event eventData = null)
		{
			if (eventData == null)
			{
				eventData = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
			}
			if (eventData != null)
			{
				List<int> listFromBitMask = BitMaskToListConverter.GetListFromBitMask(((SnowmanAttackEventData)eventData.Data).ZonesClearedMask, 18);
				_zonesProgress.Clear();
				for (int i = 0; i < listFromBitMask.Count; i++)
				{
					_zonesProgress.Add(i, listFromBitMask[i]);
				}
			}
			UpdateMapState?.Invoke();
		}

		private async UniTaskVoid SendEventData(int zoneIndex)
		{
			_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
			if (eventByName == null)
			{
				return;
			}
			ZoneEventData data = new ZoneEventData
			{
				ZoneIndex = zoneIndex
			};
			await _eventService.SubmitEventRecord(EventType.NewYearSnowManAttack, data);
			await _eventService.RetrieveCurrentEvents(forceRefresh: true);
			if (_isDisposed)
			{
				return;
			}
			eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
			if (eventByName != null)
			{
				if (eventByName.Status == EventStatus.Finished)
				{
					_eventService.RemoveCacheEventByName(eventByName.Type);
				}
				_uiEventUIWrapper.ShowEventProgressMessage($"{eventByName.Progress}/{eventByName.MaxProgress}");
			}
			UpdateProgress(eventByName);
		}

		public void LogStart(int maxCollectiblesCount, string sceneType, string zoneIndex)
		{
			_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
			if (eventByName != null && (bool)V.Car)
			{
				VehicleEntityProvider component = V.Car.GetComponent<VehicleEntityProvider>();
				if ((bool)component)
				{
					VehicleClass classVehicle = Utils.GetClassVehicle(component.CarEntity.PerformancePoints, component.GetVehicleData.CarId);
					string modeType = (FlagsStorage.IsMultiFree ? "multi_player" : "single_player");
					_eventsAnalytic.LogSnowmanStart(eventByName.Id, maxCollectiblesCount, sceneType, zoneIndex, V.CarId.ToString(), classVehicle.ToString(), modeType);
				}
			}
		}

		private void LogBegin()
		{
			_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
			if (eventByName != null)
			{
				_eventsAnalytic.LogSnowmanBegin(eventByName.Id, _currentPlayerZone.MaxCollectiblesCount);
			}
		}

		public void SendLogProgress()
		{
			if ((bool)_currentPlayerZone)
			{
				_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
				if (eventByName != null)
				{
					_eventsAnalytic.LogSnowmanProgress(eventByName.Id, _currentPlayerZone.CollectiblesCount, _timerComponent.CurrentTime, _timerComponent.TargetTime, _currentPlayerZone.SceneType.ToString());
				}
			}
		}

		public void SendLogSuccess()
		{
			if ((bool)_currentPlayerZone)
			{
				_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
				if (eventByName != null)
				{
					_eventsAnalytic.LogSnowmanSuccess(eventByName.Id, _currentPlayerZone.CollectiblesCount, _timerComponent.CurrentTime, _timerComponent.TargetTime, _currentPlayerZone.SceneType.ToString());
				}
			}
		}

		public void SendLogFail()
		{
			if ((bool)_currentPlayerZone)
			{
				_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
				if (eventByName != null)
				{
					_eventsAnalytic.LogSnowmanFail(eventByName.Id, _currentPlayerZone.CollectiblesCount, _timerComponent.CurrentTime, _timerComponent.TargetTime, _currentPlayerZone.SceneType.ToString());
				}
			}
		}

		public void SendLogExit()
		{
			if ((bool)_currentPlayerZone)
			{
				_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.NewYearSnowManAttack);
				if (eventByName != null)
				{
					_eventsAnalytic.LogSnowmanExit(eventByName.Id);
				}
			}
		}
	}
}
