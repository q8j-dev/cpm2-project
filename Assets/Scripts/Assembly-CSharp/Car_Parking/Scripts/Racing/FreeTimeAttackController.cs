using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using Gley.TrafficSystem.Internal;
using MVPFree.MapIcons.ScrObjs.Scripts;
using Service;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using VContainer;
using VContainer.Unity;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Rally.Data;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;

namespace Car_Parking.Scripts.Racing
{
	public class FreeTimeAttackController : IInitializable, IDisposable
	{
		private const int SemaphoreDuration = 4;

		public const int DisqualCheckpointsCount = 3;

		private const float MaxDistanceFromTrack = 10f;

		private const int DisqualifyThresholdSecs = 10;

		public Action<bool, float, TimeAttackMode> OnRaceFinish;

		private Dictionary<int, RaceBestData> _bestResults;

		private int _currentTrackIndex;

		private int _currentTrajectoryIndex;

		private int _restartCount;

		private FreeRaceCar _raceCar;

		private UITimeBannerController _uiTimeBannerController;

		private ManagerUI _managerUI;

		private IPrefabFactory _prefabFactory;

		private CircleProgressBarItem _progressBarItem;

		private RaceTriggersValidator _raceTriggersValidator;

		private TimeRaceStatisticsData _raceStatisticsData = new TimeRaceStatisticsData();

		private LapRaceTrafficLightUI _trafficLightController;

		private TimeAttackDataSO _timeAttackDataSo;

		private OptionalDependency<TimeAttackDataSO> _timeAttackDataSoDep;

		private OptionalDependency<FreeDriveProvider> _freeDriveProviderDep;

		private TimeAttackData _currentTimeAttackData;

		private TimeBannersManager _timeBannersManager;

		private FreeDriveProvider _freeDriveProvider;

		private LevelScript _levelScript;

		private MenuLevelParent _menuLevelParent;

		private FreeDriveControl _freeDriveControl;

		private UiNotificationsManager _uiNotificationsManager;

		private TimeAttackEventController _timeAttackEventController;

		private float _startTime;

		private CancellationTokenSource _mainCTS;

		private bool _dontUseCheckpoints;

		private TimeAttackMode _timeAttackMode;

		private TimeAttackStatus _raceStatus;

		private RallyTriggersControl _rallyTriggersControl;

		private RallySequenceUIController _rallySequenceUIController;

		private RallyPilotSoundController _rallyPilotSoundController;

		private int _centerlineNearestIdx;

		private UniTask _rallyTriggersReady = UniTask.CompletedTask;

		private TimeAttackAnalyticHelper _timeAttackAnalyticHelper;

		private EventsAnalytic _eventsAnalytic;

		private EventRestrictionService _eventRestrictionService;

		private ISteamAchievementService _steamAchievementService;

		private RacingProvider _racingProvider;

		private SoundBankLoadService _soundBankLoadService;

		private bool _eventVoicesBankLoaded;

		private readonly List<RaceTriggersValidator> _registeredValidators = new List<RaceTriggersValidator>();

		public TimeAttackStatus RaceStatus => _raceStatus;

		public TimeAttackMode TimeAttackMode => _timeAttackMode;

		public int CurrentTrackIndex => _currentTrackIndex;

		public int CurrentTrajectoryIndex => _currentTrajectoryIndex;

		public TimeAttackData CurrentTimeAttackData => _currentTimeAttackData;

		public float StartTime => _startTime;

		public RaceTriggersValidator RaceTriggersValidator => _raceTriggersValidator;

		public TimeAttackAnalyticHelper TimeAttackAnalyticHelper => _timeAttackAnalyticHelper;

		public int RestartCount => _restartCount;

		public FreeTimeAttackController()
		{
			_mainCTS = new CancellationTokenSource();
		}

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(ManagerUI managerUI, IPrefabFactory prefabFactory, EventsAnalytic eventsAnalytic, MenuLevelParent menuLevelParent, LevelScript levelScript, FreeDriveControl freeDriveControl, UiNotificationsManager uiNotificationsManager, TimeBannersManager timeBannersManager, EventRestrictionService eventRestrictionService, RacingProvider racingProvider, SoundBankLoadService soundBankLoadService, OptionalDependency<TimeAttackDataSO> timeAttackDataSo, OptionalDependency<FreeDriveProvider> freeDriveProvider, ISteamAchievementService steamAchievementService)
		{
			_racingProvider = racingProvider;
			_soundBankLoadService = soundBankLoadService;
			_freeDriveControl = freeDriveControl;
			_managerUI = managerUI;
			_prefabFactory = prefabFactory;
			_timeAttackDataSoDep = timeAttackDataSo;
			_eventsAnalytic = eventsAnalytic;
			_menuLevelParent = menuLevelParent;
			_freeDriveProviderDep = freeDriveProvider;
			_levelScript = levelScript;
			_uiNotificationsManager = uiNotificationsManager;
			_timeBannersManager = timeBannersManager;
			_eventRestrictionService = eventRestrictionService;
			_steamAchievementService = steamAchievementService;
		}

		public void Initialize()
		{
			_timeAttackDataSo = _timeAttackDataSoDep?.Value;
			_freeDriveProvider = _freeDriveProviderDep?.Value;
			_timeAttackEventController = _prefabFactory.Create<TimeAttackEventController>(Array.Empty<object>());
			_bestResults = new Dictionary<int, RaceBestData>();
			_timeAttackAnalyticHelper = new TimeAttackAnalyticHelper(_eventsAnalytic, _eventRestrictionService);
			_racingProvider.FreeTimeAttackController = this;
			if ((bool)_freeDriveProvider.FreeDriveControl)
			{
				FreeDriveControl freeDriveControl = _freeDriveProvider.FreeDriveControl;
				freeDriveControl.OnCarToRoad = (Action<GameObject>)Delegate.Combine(freeDriveControl.OnCarToRoad, new Action<GameObject>(CarToRoadCallback));
			}
		}

		public void Dispose()
		{
			ReleaseEventVoicesBank();
			CommonExtensions.CancelAndDisposeToken(ref _mainCTS);
			if (_racingProvider.FreeTimeAttackController == this)
			{
				_racingProvider.FreeTimeAttackController = null;
			}
			if ((bool)_freeDriveProvider.FreeDriveControl)
			{
				FreeDriveControl freeDriveControl = _freeDriveProvider.FreeDriveControl;
				freeDriveControl.OnCarToRoad = (Action<GameObject>)Delegate.Remove(freeDriveControl.OnCarToRoad, new Action<GameObject>(CarToRoadCallback));
			}
		}

		public bool IsRaceInProgress()
		{
			if (_raceStatus != TimeAttackStatus.Stage)
			{
				return _raceStatus == TimeAttackStatus.Race;
			}
			return true;
		}

		public void Init(int raceIndex, bool dontUseCheckpoints, int trajectoryIndex)
		{
			_dontUseCheckpoints = dontUseCheckpoints;
			_raceStatus = TimeAttackStatus.Stage;
			_currentTrackIndex = raceIndex;
			_currentTrajectoryIndex = trajectoryIndex;
			_currentTimeAttackData = null;
			if (raceIndex < _timeAttackDataSo.TimeAttackDatas.Length)
			{
				_currentTimeAttackData = _timeAttackDataSo.TimeAttackDatas[raceIndex];
			}
		}

		private async UniTaskVoid CreateUITrafficLight(CancellationToken cancellationToken)
		{
			if (!_trafficLightController)
			{
				GameObject obj = await _managerUI.LoadWindowPrefab("UITrafficLight");
				if (!cancellationToken.IsCancellationRequested)
				{
					GameObject gameObject = _prefabFactory.Instantiate(obj, _managerUI.GetWindowsParent());
					_trafficLightController = gameObject.GetComponent<LapRaceTrafficLightUI>();
					_trafficLightController.gameObject.SetActive(value: false);
				}
			}
		}

		private void ActivateTrafficLight(bool state)
		{
			if ((bool)_trafficLightController)
			{
				_trafficLightController.gameObject.SetActive(state);
			}
		}

		private void DestroyUITrafficLight()
		{
			if ((bool)_trafficLightController)
			{
				UnityEngine.Object.Destroy(_trafficLightController.gameObject);
			}
		}

		public void StopRaceImmediately(FailReason failReason)
		{
			UnsubscribeUser();
			if (_raceStatus != TimeAttackStatus.Finish)
			{
				_raceStatus = TimeAttackStatus.Finish;
				_managerUI.CloseMultiplayerRacePlayWindow();
				_managerUI.ShowNotificationWindow(new LocalizedString($"FreeRace.{_timeAttackMode}RaceStopped"), 3f).Forget();
				_raceCar.FinishRace();
				_raceTriggersValidator.DeactivateAll();
			}
			_timeAttackEventController.OnFailedRace(failReason);
			_raceStatus = TimeAttackStatus.Finish;
			DestroyUITrafficLight();
			if ((bool)_rallyTriggersControl)
			{
				DestroyRallySequenceUI();
				_rallyPilotSoundController?.StopAll();
				_rallyPilotSoundController = null;
			}
			ReleaseEventVoicesBank();
		}

		public void StopPrepareRace()
		{
			_raceStatus = TimeAttackStatus.Finish;
			ReleaseEventVoicesBank();
		}

		private void RequestEventVoicesBank()
		{
			if (!_eventVoicesBankLoaded && TimeAttackMode == TimeAttackMode.Rally)
			{
				_eventVoicesBankLoaded = true;
				_soundBankLoadService?.TryLoadBank(SoundBankType.CpmEventVoices);
			}
		}

		private void ReleaseEventVoicesBank()
		{
			if (_eventVoicesBankLoaded)
			{
				_eventVoicesBankLoaded = false;
				_soundBankLoadService?.TryUnloadBank(SoundBankType.CpmEventVoices);
			}
		}

		public void RegisterTriggerValidator(RaceTriggersValidator raceTriggersValidator)
		{
			_raceTriggersValidator = raceTriggersValidator;
			if ((bool)raceTriggersValidator && !_registeredValidators.Contains(raceTriggersValidator))
			{
				_registeredValidators.Add(raceTriggersValidator);
			}
			if ((bool)_raceTriggersValidator.RallyTriggersControl)
			{
				RegisterRallyTriggersControl(_raceTriggersValidator.RallyTriggersControl);
			}
		}

		public void RegisterTriggerValidatorPassive(RaceTriggersValidator raceTriggersValidator)
		{
			if ((bool)raceTriggersValidator)
			{
				if (!_registeredValidators.Contains(raceTriggersValidator))
				{
					_registeredValidators.Add(raceTriggersValidator);
				}
				if (!_raceTriggersValidator)
				{
					RegisterTriggerValidator(raceTriggersValidator);
				}
			}
		}

		public void RegisterRallyTriggersControlPassive(RallyTriggersControl rallyTriggersControl)
		{
			if (!_rallyTriggersControl)
			{
				_rallyTriggersControl = rallyTriggersControl;
			}
		}

		public RaceTriggersValidator ResolveValidatorFor(int trackIndex, int trajectoryIndex, Scene callerScene)
		{
			RaceTriggersValidator raceTriggersValidator = null;
			foreach (RaceTriggersValidator registeredValidator in _registeredValidators)
			{
				if ((bool)registeredValidator && registeredValidator.HasRacePropsData(trackIndex, trajectoryIndex))
				{
					if (registeredValidator.gameObject.scene == callerScene)
					{
						return registeredValidator;
					}
					if (!raceTriggersValidator)
					{
						raceTriggersValidator = registeredValidator;
					}
				}
			}
			return raceTriggersValidator;
		}

		public void RegisterRallyTriggersControl(RallyTriggersControl rallyTriggersControl)
		{
			_rallyTriggersControl = rallyTriggersControl;
		}

		public void PrepareRace()
		{
			_raceStatus = TimeAttackStatus.Prepare;
			RequestEventVoicesBank();
			if ((bool)_rallyTriggersControl && TimeAttackMode == TimeAttackMode.Rally)
			{
				_rallyTriggersReady = _rallyTriggersControl.Init(spawnTrajectory: false).Preserve();
			}
			CreateUITrafficLight(_mainCTS.Token).Forget();
		}

		private void SpawnRallySequenceUI()
		{
			DestroyRallySequenceUI();
			if (TimeAttackMode == TimeAttackMode.Rally)
			{
				_rallyPilotSoundController?.Dispose();
				_rallyPilotSoundController = new RallyPilotSoundController(_raceTriggersValidator.gameObject, _rallyTriggersControl.RallySequenceData);
				int count = PlayerPrefs.GetInt("PaceNotesValue", 2);
				_rallyPilotSoundController?.ChangePacenotesQueueCount(count);
				_rallySequenceUIController = _prefabFactory.Instantiate(_rallyTriggersControl.RallySequenceUIControllerPrefab, _managerUI.transform);
				_rallySequenceUIController.Init(_rallyTriggersControl, _rallyTriggersControl.RallySequenceData);
				_rallySequenceUIController.Show(state: false);
			}
		}

		private void DestroyRallySequenceUI()
		{
			if (_rallySequenceUIController != null)
			{
				if (_rallyPilotSoundController != null)
				{
					RallySequenceUIController rallySequenceUIController = _rallySequenceUIController;
					rallySequenceUIController.OnNoteActivated = (Action<RallySequenceItem>)Delegate.Remove(rallySequenceUIController.OnNoteActivated, new Action<RallySequenceItem>(_rallyPilotSoundController.AnnounceItem));
				}
				UnityEngine.Object.Destroy(_rallySequenceUIController.gameObject);
				_rallySequenceUIController = null;
			}
		}

		private void CarToRoadCallback(GameObject car)
		{
			if (RaceStatus == TimeAttackStatus.Stage && V.Car == car && car.TryGetComponent<FreeRaceCar>(out var _))
			{
				StopRaceImmediately(FailReason.DISQUALIFIED);
			}
		}

		private void SubscribeUser()
		{
			FreeRaceCar raceCar = _raceCar;
			raceCar.UserStartRace = (Action)Delegate.Combine(raceCar.UserStartRace, new Action(StartRace));
			FreeRaceCar raceCar2 = _raceCar;
			raceCar2.UserFinishedRace = (Action<string, float, int, TimeAttackMode>)Delegate.Combine(raceCar2.UserFinishedRace, new Action<string, float, int, TimeAttackMode>(Finish));
			FreeRaceCar raceCar3 = _raceCar;
			raceCar3.UserExitRace = (Action<FailReason>)Delegate.Combine(raceCar3.UserExitRace, new Action<FailReason>(StopRaceImmediately));
		}

		private void UnsubscribeUser()
		{
			FreeRaceCar raceCar = _raceCar;
			raceCar.UserStartRace = (Action)Delegate.Remove(raceCar.UserStartRace, new Action(StartRace));
			FreeRaceCar raceCar2 = _raceCar;
			raceCar2.UserFinishedRace = (Action<string, float, int, TimeAttackMode>)Delegate.Remove(raceCar2.UserFinishedRace, new Action<string, float, int, TimeAttackMode>(Finish));
			FreeRaceCar raceCar3 = _raceCar;
			raceCar3.UserExitRace = (Action<FailReason>)Delegate.Remove(raceCar3.UserExitRace, new Action<FailReason>(StopRaceImmediately));
		}

		public void RegisterCar(FreeRaceCar car)
		{
			_raceCar = car;
			_timeAttackEventController.SetRaceCar(_raceCar);
			_timeAttackAnalyticHelper.SetRaceCar(car);
			SubscribeUser();
			_freeDriveProvider.FreeDriveControl.InvitationRejectIgnore(withIgnore: false);
			_managerUI.CloseWindow("SubmitCircle");
			_managerUI.CloseWindow("RequestWindow");
			StartRaceCounter(_mainCTS.Token).Forget();
		}

		public void SetTimeAttackMode(TimeAttackMode timeAttackMode)
		{
			_timeAttackMode = timeAttackMode;
		}

		public void StartRace()
		{
			if ((bool)_rallyTriggersControl && TimeAttackMode == TimeAttackMode.Rally)
			{
				_rallyTriggersControl.ActivateTriggers();
				if (_rallySequenceUIController != null)
				{
					_rallySequenceUIController.Show(state: true);
					_rallySequenceUIController.ShowSequence(show: true);
					if (_rallyPilotSoundController != null)
					{
						RallySequenceUIController rallySequenceUIController = _rallySequenceUIController;
						rallySequenceUIController.OnNoteActivated = (Action<RallySequenceItem>)Delegate.Combine(rallySequenceUIController.OnNoteActivated, new Action<RallySequenceItem>(_rallyPilotSoundController.AnnounceItem));
					}
				}
				_rallyPilotSoundController?.OnRaceStart();
			}
			_startTime = Time.time;
			DestroyUITrafficLight();
			FillRaceStatisticsData();
			_raceTriggersValidator.Init(_currentTrackIndex, _currentTrajectoryIndex);
			_raceStatus = TimeAttackStatus.Race;
			_raceCar.StartRace();
			_timeAttackEventController.TrySendStartRaceAnalytics();
			CheckDistanceFromTrackLoop(_mainCTS.Token).Forget();
			if (TrafficManager.Exists && TrafficManager.Instance.Initialized)
			{
				API.SetTrafficDensity(0);
				TrafficManager.Instance.gameObject.SetActive(value: false);
			}
		}

		public void TryHideStartTriggersInLocation(V.placeToSpawn location)
		{
			foreach (RaceTriggersValidator registeredValidator in _registeredValidators)
			{
				if ((bool)registeredValidator)
				{
					registeredValidator.TryHideStartTriggersInLocation(location);
				}
			}
		}

		public void TryShowHiddenStartTriggers()
		{
			foreach (RaceTriggersValidator registeredValidator in _registeredValidators)
			{
				if ((bool)registeredValidator)
				{
					registeredValidator.TryShowLastHiddenStartTriggersSafe();
				}
			}
		}

		private void FillRaceStatisticsData()
		{
			_raceStatisticsData.TrackId = _currentTrackIndex;
		}

		private async UniTaskVoid StartRaceCounter(CancellationToken cancellationToken)
		{
			await UniTask.Yield();
			if (_raceStatus != TimeAttackStatus.Stage || cancellationToken.IsCancellationRequested)
			{
				return;
			}
			_freeDriveControl.TryCancelInvitation();
			await _rallyTriggersReady.SuppressCancellationThrow();
			if (_raceStatus != TimeAttackStatus.Stage || cancellationToken.IsCancellationRequested)
			{
				return;
			}
			PrepareUI();
			ActivateTrafficLight(state: true);
			_raceCar.FreezeCar(state: true);
			for (int i = 0; i < 4; i++)
			{
				_trafficLightController.ShowRedLights(1f).Forget();
				_raceTriggersValidator.StartTrafficLightObject.ShowRedLights(1f).Forget();
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (_raceStatus != TimeAttackStatus.Stage || cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			if (_raceStatus == TimeAttackStatus.Stage)
			{
				_trafficLightController.SetGreenLight(state: true);
				_raceTriggersValidator.StartTrafficLightObject.ShowGreenLight();
				_raceCar.FreezeCar(state: false);
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (_raceStatus == TimeAttackStatus.Stage && !cancellationToken.IsCancellationRequested)
				{
					_raceTriggersValidator.StartTrafficLightObject.Reset();
					_trafficLightController.SetGreenLight(state: false);
					_trafficLightController.Reset();
					DestroyUITrafficLight();
					_raceTriggersValidator.ShowStartTriggers(CurrentTrackIndex, _currentTrajectoryIndex, state: false);
					_raceTriggersValidator.EnableStartTriggers(CurrentTrackIndex, _currentTrajectoryIndex, state: true);
					CheckCanRace();
				}
			}
		}

		private void CheckCanRace()
		{
			if (Global.TaskManager.Task != null || FlagsStorage.IsPhotoMode || _menuLevelParent.isDroneEnabled)
			{
				V.CoreGameManager.SetMessage("Free.FinishCurrentMission", 3f);
				StopRaceImmediately(FailReason.DISQUALIFIED);
			}
		}

		private async UniTaskVoid CheckDistanceFromTrackLoop(CancellationToken cancellationToken)
		{
			if (_timeAttackMode != TimeAttackMode.Rally || _rallyTriggersControl == null)
			{
				return;
			}
			List<Vector3> centerline = _rallyTriggersControl.RoadCenterline;
			if (centerline == null || centerline.Count < 2)
			{
				return;
			}
			float threshold = _rallyTriggersControl.RoadHalfWidth + 10f;
			_centerlineNearestIdx = 0;
			int secondsOutOfBounds = 0;
			while (!cancellationToken.IsCancellationRequested && _raceStatus == TimeAttackStatus.Race)
			{
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (cancellationToken.IsCancellationRequested || _raceStatus != TimeAttackStatus.Race)
				{
					break;
				}
				Vector3 position = _raceCar.transform.position;
				position.y = 0f;
				int num = Mathf.Max(0, _centerlineNearestIdx - 30);
				int num2 = Mathf.Min(centerline.Count - 1, _centerlineNearestIdx + 40);
				float num3 = float.MaxValue;
				for (int i = num; i <= num2; i++)
				{
					Vector3 b = centerline[i];
					b.y = 0f;
					b += V.GlobalOffset;
					float num4 = Vector3.Distance(position, b);
					if (num4 < num3)
					{
						num3 = num4;
						_centerlineNearestIdx = i;
					}
				}
				if (num3 > threshold)
				{
					secondsOutOfBounds++;
					if (secondsOutOfBounds >= 10)
					{
						StopRaceImmediately(FailReason.DISQUALIFIED);
						break;
					}
				}
				else
				{
					secondsOutOfBounds = 0;
				}
			}
		}

		private void PrepareUI()
		{
			_managerUI.ClosePhoneWindow();
			_managerUI.CloseSettingsWindow();
			_levelScript.UnCleanScreen();
			if (_rallyTriggersControl != null && TimeAttackMode == TimeAttackMode.Rally)
			{
				SpawnRallySequenceUI();
			}
		}

		public void TryCarToRoad()
		{
			if ((bool)_raceCar)
			{
				Transform nearestToRoadPoint = _raceTriggersValidator.GetNearestToRoadPoint(_raceCar.transform, _currentTrackIndex, _currentTrajectoryIndex);
				if ((bool)nearestToRoadPoint)
				{
					_raceCar.ResetRigidbody();
					_raceCar.ToRoad();
					Vector3 posByWheel = VectorExtensions.GetPosByWheel(_raceCar.gameObject, nearestToRoadPoint.position);
					_raceCar.transform.SetPositionAndRotation(posByWheel, nearestToRoadPoint.rotation);
					_raceCar.OnCarToRoad(_raceCar.gameObject);
				}
				else
				{
					_freeDriveControl.ToRoad();
				}
			}
		}

		public void Finish(string userName, float time, int classId, TimeAttackMode timeAttackMode)
		{
			UnsubscribeUser();
			_raceStatus = TimeAttackStatus.Finish;
			int missCheckpointsCount = 0;
			int missCheckpointMultiplierSecs = 0;
			_raceTriggersValidator.GetMissingCheckpointsCount(_currentTrackIndex, _currentTrajectoryIndex, out missCheckpointsCount, out missCheckpointMultiplierSecs);
			if (missCheckpointsCount >= 3)
			{
				OnRaceFinish?.Invoke(arg1: false, time, timeAttackMode);
				_managerUI.ShowNotificationWindow(new LocalizedString($"FreeRace.{_timeAttackMode}StoppedNotCorrect"), 4f).Forget();
				_timeAttackEventController.OnFailedRace(FailReason.LEAVE_DIRECTION);
			}
			else
			{
				float num = missCheckpointsCount * missCheckpointMultiplierSecs;
				time += num;
				CreateCheckpointsPopup(missCheckpointsCount, num);
				OnRaceFinish?.Invoke(arg1: true, time, timeAttackMode);
				_raceStatisticsData.Time = _raceCar.UsersTime;
				_raceStatisticsData.CarClassId = classId;
				TimeBannerController timeBannersManager = _timeBannersManager.GetTimeBannersManager(_currentTrackIndex);
				RaceBestData raceBestData = GetBestTime(_currentTrackIndex);
				if (raceBestData == null)
				{
					raceBestData = new RaceBestData
					{
						BestResult = 0f,
						TimeAttackType = timeAttackMode
					};
				}
				if (IsResultBeated(time, raceBestData))
				{
					SetBestTime(time, _currentTrackIndex);
					timeBannersManager.OnFinishUpdateTable(userName, time, raceBestData.BestResult);
				}
				_timeAttackEventController.SendEventData(time, raceBestData.BestResult, classId, num).Forget();
			}
			if (TimeAttackMode == TimeAttackMode.TimeAttack)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQHg", "time_attack");
			}
			if (TimeAttackMode == TimeAttackMode.TimeAttack)
			{
				_steamAchievementService?.Unlock("ACH_TIMEATTACK_RACE");
			}
			if ((bool)_rallyTriggersControl && TimeAttackMode == TimeAttackMode.Rally)
			{
				DestroyRallySequenceUI();
				_rallyTriggersControl.DestroyTrajectory();
				_rallyPilotSoundController?.Dispose();
				_rallyPilotSoundController = null;
			}
			ReleaseEventVoicesBank();
			_raceTriggersValidator.DeactivateAll();
			_restartCount = 0;
		}

		public RaceBestData GetBestTime(int raceIndex, TimeAttackMode timeAttackMode = TimeAttackMode.TimeAttack)
		{
			if (_bestResults.TryGetValue(raceIndex, out var value))
			{
				return value;
			}
			RaceBestData raceBestData = new RaceBestData
			{
				BestResult = 0f,
				TimeAttackType = timeAttackMode
			};
			_bestResults.Add(raceIndex, raceBestData);
			return raceBestData;
		}

		public bool IsResultBeated(float result, RaceBestData bestResult)
		{
			if (bestResult.BestResult == 0f)
			{
				return true;
			}
			switch (bestResult.TimeAttackType)
			{
			case TimeAttackMode.TimeAttack:
			case TimeAttackMode.Rally:
				return result < bestResult.BestResult;
			case TimeAttackMode.Drift:
				return result > bestResult.BestResult;
			default:
				return true;
			}
		}

		public void Restart()
		{
			_restartCount++;
			CreateUITrafficLight(_mainCTS.Token).Forget();
			_raceStatus = TimeAttackStatus.Stage;
			_managerUI.CloseMultiplayerRacePlayWindow();
			_raceCar.RestartRace();
			RotateCarToStart();
			StartRaceCounter(_mainCTS.Token).Forget();
			_timeAttackEventController.TrySendRestartRaceAnalytics();
			if ((bool)_rallyTriggersControl && TimeAttackMode == TimeAttackMode.Rally)
			{
				if (_rallySequenceUIController != null && _rallyPilotSoundController != null)
				{
					RallySequenceUIController rallySequenceUIController = _rallySequenceUIController;
					rallySequenceUIController.OnNoteActivated = (Action<RallySequenceItem>)Delegate.Remove(rallySequenceUIController.OnNoteActivated, new Action<RallySequenceItem>(_rallyPilotSoundController.AnnounceItem));
				}
				_rallyPilotSoundController?.Reset();
				_rallyTriggersControl.RestartRace();
			}
		}

		public void RotateCarToStart()
		{
			try
			{
				Transform randomStartPoint = _raceTriggersValidator.GetRandomStartPoint(_currentTrackIndex, _currentTrajectoryIndex);
				_raceCar.transform.SetPositionAndRotation(randomStartPoint.transform.position, randomStartPoint.transform.rotation);
			}
			catch (Exception)
			{
			}
		}

		public void SetBestTime(float bestTime, int trackIndex)
		{
			_bestResults[trackIndex].BestResult = bestTime;
		}

		public void EnableComponents(bool state)
		{
			_timeBannersManager.gameObject.SetActive(state);
		}

		public void OnResume(bool state)
		{
			if ((bool)_rallySequenceUIController)
			{
				_rallySequenceUIController.Show(state);
			}
		}

		private void CreateCheckpointsPopup(int missCheckpointsCount, float time)
		{
			if (!_dontUseCheckpoints)
			{
				PopupData popupData = new PopupData();
				popupData.Delay = 1.5f;
				popupData.Title = new LocalizedString("TimeAttack.Checkpoints");
				int allCheckpointsCount = _raceTriggersValidator.GetAllCheckpointsCount(CurrentTrackIndex, CurrentTrajectoryIndex);
				int num = allCheckpointsCount - missCheckpointsCount;
				bool flag = num == allCheckpointsCount;
				string text = (flag ? "green" : "red");
				string text2 = (flag ? "" : ("<color=" + text + ">+" + Utils.ConvertToTime(time) + "</color=" + text + ">"));
				popupData.Description = $"<color={text}>{num}</color={text}>/{allCheckpointsCount} {text2}";
				popupData.PopupKey = (flag ? "CheckPointBeatPopup" : "CheckPointNotBeatPopup");
				popupData.AudioKey = (flag ? SoundEventsDataHelper.UITimeAttackEventCanClaimSwitch : SoundEventsDataHelper.UITimeAttackBestResultShowSwitch);
				_uiNotificationsManager.ShowPopup(popupData);
			}
		}
	}
}
