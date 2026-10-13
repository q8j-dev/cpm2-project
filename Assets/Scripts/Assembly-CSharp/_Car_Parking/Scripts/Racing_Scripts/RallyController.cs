using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.GrantService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Analytics;
using _Car_Parking.Scripts.Rally;
using _Car_Parking.Scripts.Rally.Data;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyController : MonoBehaviour, IRaceController
	{
		private struct StageSnapshot
		{
			public RallyMedalType PrevMedal;

			public float PrevPersonalBest;

			public bool HasPrevBest;
		}

		public const float MissedCheckpointPenalty = 10f;

		public const float MarshalCollisionPenalty = 60f;

		public const float PeopleCollisionPenalty = 20f;

		private const float ToRoadPenalty = 10f;

		private const float FinishMarshalPenalty = 10f;

		private const float StopPointRadius = 6f;

		private const float StopFillFinishThreshold = 0.7f;

		public Action OnLocalProgressUpdated;

		private RallyState _rallyState;

		public static int LocationIndex;

		public static int TrajectoryIndex;

		public static DayTimeType DayTimeType;

		public static WeatherType WeatherType;

		public static bool LoadFromBanner;

		public RallySequenceUIController RallySequenceUIControllerPrefab;

		[SerializeField]
		private RallyCarControl RallyCarControl;

		[SerializeField]
		private RallyTriggersControl RallyTriggersControl;

		[SerializeField]
		private RallyPassengerNpc RallyPassengerNpc;

		[SerializeField]
		private RallyMapDataSO RallyMapDataSo;

		[SerializeField]
		private RallyCarsDatasSO RallyCarsDatasSo;

		[SerializeField]
		private RacingProgressUIController RacingProgressUIControllerPrefab;

		[SerializeField]
		private RallyTrajectoryWeatherDataSO RallyTrajectoryWeatherDataSO;

		private RacingProgressUIController _racingProgressUI;

		private RallyProgressUIDriver _progressUIDriver;

		private RallyMapTrajectorySO _rallySequenceDataSO;

		private RallySequenceData _racingSequenceData;

		private RacingCameraController _racingCameraController;

		private RacingProvider _racingProvider;

		private PlayerDatas.CarLoadingUtility _carLoadingUtility;

		private CircuitRacingWaitingWindow _circuitRacingWaitingWindow;

		private ManagerUI _managerUI;

		private bool _isDestroyed;

		private bool _startRaceTriggered;

		private bool _finishTriggered;

		private bool _usedToRoad;

		private LapRaceTrafficLightUI _lapRaceTrafficLightUI;

		private RallyPlayWindow _rallyPlayWindow;

		private RallyCar _rallyCar;

		private MenuLevelParent _menuLevelParent;

		private RaceModeController _raceModeController;

		private RallyDisqualifyTimers _timers;

		private RallyPilotSoundController _paceNotesSoundController;

		private V.placeToSpawn _currentPlaceToSpawn;

		private RallySequenceUIController _currentRallySequenceUIController;

		private IPrefabFactory _prefabFactory;

		private RallyProgressController _rallyProgressController;

		private RallyAnalytic _rallyAnalytic;

		private RallySessionTracker _rallySessionTracker;

		private bool _suppressNextStartLog;

		private UniTask _triggersReady = UniTask.CompletedTask;

		private CancellationTokenSource _raceCts;

		private RaceEnvironment.TimeSettings _defaultTimeSettings;

		public RallyCarData[] DebugCarsConfigs;

		private CurrencyManagerController _currencyManagerController;

		private RewardGranter _rewardGranter;

		private RallyData _rallyData;

		private AllCarsInfo _allCarsInfo;

		private SoundBankLoadService _soundBankLoadService;

		private RainSound _rainSound;

		private CarInputManager _carInputManager;

		private bool _rainPausedForLoading;

		private ISteamAchievementService _steamAchievementService;

		private string _lastCarClass = "unknown";

		public RallyState RallyState
		{
			get
			{
				return _rallyState;
			}
			private set
			{
				if (_rallyState != value)
				{
					_rallyState = value;
					OnStateChanged?.Invoke();
				}
			}
		}

		public Func<Transform> GhostTransformProvider { get; set; }

		public Func<CancellationToken, UniTask> GhostReadyProvider { get; set; }

		public RallySequenceData RallySequenceData => _racingSequenceData;

		public RallyProgressController RallyProgressController => _rallyProgressController;

		public RallyData RallyData => _rallyData;

		public RallyMapDataSO RallyMapDataSO => RallyMapDataSo;

		public int CurrentCarClassId
		{
			get
			{
				MainCarValues issuedRallyCarValues = GetIssuedRallyCarValues();
				if (!(issuedRallyCarValues != null))
				{
					return 0;
				}
				return (int)issuedRallyCarValues.Class;
			}
		}

		public event Action OnStateChanged;

		public event Action RunStaged;

		public event Action RunStarted;

		public event Action<float> RunFinished;

		public event Action RunAborted;

		[Inject]
		public void Construct(RacingCameraController racingCameraController, PlayerDatas.CarLoadingUtility carLoadingUtility, CircuitRacingWaitingWindow circuitRacingWaitingWindow, ManagerUI managerUI, LapRaceTrafficLightUI lapRaceTrafficLightUI, MenuLevelParent menuLevelParent, RaceModeController raceModeController, IPrefabFactory prefabFactory, RallyAnalytic rallyAnalytic, RallySessionTracker rallySessionTracker, CurrencyManagerController currencyManagerController, RallyData rallyData, ISteamAchievementService steamAchievementService, RewardGranter rewardGranter, AllCarsInfo allCarsInfo, SoundBankLoadService soundBankLoadService, RacingProvider racingProvider, RainSound rainSound, CarInputManager carInputManager)
		{
			_rainSound = rainSound;
			_carInputManager = carInputManager;
			_racingProvider = racingProvider;
			_racingProvider.RallyController = this;
			_racingCameraController = racingCameraController;
			_carLoadingUtility = carLoadingUtility;
			_circuitRacingWaitingWindow = circuitRacingWaitingWindow;
			_managerUI = managerUI;
			_lapRaceTrafficLightUI = lapRaceTrafficLightUI;
			_menuLevelParent = menuLevelParent;
			_raceModeController = raceModeController;
			_prefabFactory = prefabFactory;
			_rallyAnalytic = rallyAnalytic;
			_rallySessionTracker = rallySessionTracker;
			_currencyManagerController = currencyManagerController;
			_rallyData = rallyData;
			_rewardGranter = rewardGranter;
			_allCarsInfo = allCarsInfo;
			_soundBankLoadService = soundBankLoadService;
			_steamAchievementService = steamAchievementService;
		}

		public void SetSuppressNextStartLog()
		{
			_suppressNextStartLog = true;
		}

		public async UniTaskVoid InitRally(V.placeToSpawn placeToSpawn, int selectedLocationId, int selectedTrajectoryId, RallyMapTrajectorySO rallyMapTrajectorySo)
		{
			_currentPlaceToSpawn = placeToSpawn;
			_rallySequenceDataSO = rallyMapTrajectorySo;
			LocationIndex = selectedLocationId;
			TrajectoryIndex = selectedTrajectoryId;
			PauseRainWhileLoading();
			_circuitRacingWaitingWindow.gameObject.SetActive(value: true);
			await _raceModeController.InitializeScene(placeToSpawn);
			if (_isDestroyed)
			{
				ResumeRainAfterLoading();
			}
			else
			{
				SetGarage();
			}
		}

		private void Start()
		{
			_defaultTimeSettings = RaceEnvironment.CaptureTimeSettings();
			_timers = new RallyDisqualifyTimers(() =>
			{
				Disqualify();
			}, () =>
			{
				if (!_startRaceTriggered)
				{
					Disqualify(RallyDisqualifyType.NotStarted);
				}
			}, () =>
			{
				_paceNotesSoundController?.PlayCoPilotComments(CopilotCommentsType.NotMoving);
			});
			_rallyProgressController = new RallyProgressController(RallyMapDataSo);
			_rallyProgressController.Init(this, _managerUI, _currencyManagerController, _rallyData, _rewardGranter);
			RaceModeController raceModeController = _raceModeController;
			raceModeController.ShowMainMenuAction = (Action)Delegate.Combine(raceModeController.ShowMainMenuAction, new Action(ShowMainMenu));
			_currentRallySequenceUIController = _prefabFactory.Instantiate(RallySequenceUIControllerPrefab, _managerUI.transform);
			_currentRallySequenceUIController.Show(state: false);
			_rallyAnalytic?.LogModeEntered(RallyModeType.single_company);
			if (LoadFromBanner)
			{
				SpawnFirstWindow().Forget();
				LoadFromBanner = false;
			}
			else
			{
				InitRally(RallyMapDataSo.RallyLocationDatas[LocationIndex].PlaceToSpawn, LocationIndex, TrajectoryIndex, RallyMapDataSo.RallyLocationDatas[LocationIndex].TrajectoryData).Forget();
			}
			_soundBankLoadService?.TryLoadBank(SoundBankType.CpmEventVoices);
		}

		private async UniTaskVoid SpawnFirstWindow()
		{
			RallySelectLocationWindow obj = await _managerUI.CreateWindowGeneric<RallySelectLocationWindow>("RallySelectMapWindow", onTop: false);
			obj.gameObject.SetActive(value: false);
			obj.Init(this, _rallyProgressController);
			_circuitRacingWaitingWindow.gameObject.SetActive(value: false);
			obj.gameObject.SetActive(value: true);
		}

		public void OpenMapWindow()
		{
			_circuitRacingWaitingWindow.gameObject.SetActive(value: true);
			_racingCameraController?.SetTouchInputEnabled(value: true);
			DisposeProgressUI();
			ResetRaceCts();
			_timers.StopIdleCheck();
			_timers.StopStageCountdown();
			_timers.StopFinishCountdown();
			_startRaceTriggered = false;
			_finishTriggered = false;
			RallyState = RallyState.Garage;
			_managerUI.SetCarInputHardLock(locked: false);
			_paceNotesSoundController?.Dispose();
			_paceNotesSoundController = null;
			if (V.Car != null)
			{
				RallyPassengerNpc.DespawnPassengerNpc(V.Car);
				DestroyCarDeferred(V.Car).Forget();
				_rallyCar = null;
			}
			RallyTriggersControl.DestroyTrajectory();
			if (_currentRallySequenceUIController != null)
			{
				UnityEngine.Object.Destroy(_currentRallySequenceUIController.gameObject);
				_currentRallySequenceUIController = null;
			}
			if (_rallyPlayWindow != null)
			{
				_rallyPlayWindow.Close();
				_rallyPlayWindow = null;
			}
			if (_racingSequenceData != null)
			{
				_raceModeController.UnloadScene(_racingSequenceData.PlaceToSpawn).Forget();
			}
			SpawnFirstWindow().Forget();
		}

		private async UniTaskVoid DestroyCarDeferred(GameObject car)
		{
			await UniTask.DelayFrame(2);
			if (car != null)
			{
				UnityEngine.Object.Destroy(car);
			}
		}

		public void Restart()
		{
			if (_rallyAnalytic != null && _rallySessionTracker != null)
			{
				int restartCount = _rallySessionTracker.IncrementRestart();
				int attempt = _rallySessionTracker.GetAttempt(LocationIndex, TrajectoryIndex);
				_rallyAnalytic.LogStageRestarted(BuildStageInfo(), attempt, restartCount);
				_suppressNextStartLog = true;
			}
			_racingCameraController?.SetTouchInputEnabled(value: true);
			Time.timeScale = 1f;
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
			ResetRaceCts();
			DisposeProgressUI();
			DisableLapRaceTrafficLightUI();
			_timers.StopIdleCheck();
			_timers.StopStageCountdown();
			_timers.StopFinishCountdown();
			_startRaceTriggered = false;
			_finishTriggered = false;
			_paceNotesSoundController?.Reset();
			RecreateSequenceUIController();
			_rallyCar.ResetTimer();
			StartStage().Forget();
			RallyTriggersControl.RestartRace();
		}

		private void ResetRaceCts()
		{
			_raceCts?.Cancel();
			_raceCts?.Dispose();
			_raceCts = new CancellationTokenSource();
		}

		private void DisposeProgressUI()
		{
			_progressUIDriver?.Dispose();
			_progressUIDriver = null;
			if (_racingProgressUI != null)
			{
				UnityEngine.Object.Destroy(_racingProgressUI.gameObject);
				_racingProgressUI = null;
			}
		}

		private RallyWeatherData GetCurrentWeatherData()
		{
			if (!(RallyMapDataSo != null) || !RallyMapDataSo.TryGetConfigIds(LocationIndex, TrajectoryIndex, out var locationId, out var trajectoryId))
			{
				return RallyTrajectoryWeatherDataSO.GetWeatherData(LocationIndex, TrajectoryIndex);
			}
			return RallyTrajectoryWeatherDataSO.GetWeatherData(locationId, trajectoryId);
		}

		public void SetGarage()
		{
			_racingSequenceData = _rallySequenceDataSO.RallySequenceDataList[TrajectoryIndex];
			RallyWeatherData currentWeatherData = GetCurrentWeatherData();
			DayTimeType = currentWeatherData.DayTimeType;
			WeatherType = currentWeatherData.WeatherType;
			Init().Forget();
		}

		private void RecreateSequenceUIController()
		{
			if (_currentRallySequenceUIController != null)
			{
				UnityEngine.Object.Destroy(_currentRallySequenceUIController.gameObject);
			}
			_currentRallySequenceUIController = _prefabFactory.Instantiate(RallySequenceUIControllerPrefab, _managerUI.transform);
			_currentRallySequenceUIController.Show(state: false);
		}

		private async UniTaskVoid Init()
		{
			SetParkedSteeringLock(locked: true);
			RecreateSequenceUIController();
			RallyCarControl.Init(_racingSequenceData);
			await RallyCarControl.SpawnCarToGaragePoint();
			await UniTask.Delay(TimeSpan.FromSeconds(1.0));
			if (_isDestroyed || V.Car == null)
			{
				ResumeRainAfterLoading();
				return;
			}
			_rallyCar = V.Car.GetComponent<RallyCar>();
			V.CoreGameManager.SetupCarDriver(V.Car);
			_rallyCar.Init(this, RallyTriggersControl);
			_rallyCar.OnOffRoadWarning = () =>
			{
				_paceNotesSoundController?.PlayCoPilotComments(CopilotCommentsType.Offtrack);
			};
			_rallyCar.OnFrequentCrash = () =>
			{
				_paceNotesSoundController?.PlayCoPilotComments(CopilotCommentsType.FrequentCrash);
			};
			_rallyCar.OnCheckpointSkipped = () =>
			{
				_paceNotesSoundController?.PlayCoPilotComments(CopilotCommentsType.CheckpointSkip);
			};
			_rallyCar.OnWrongWay = (bool isWrong) =>
			{
				if (isWrong)
				{
					_paceNotesSoundController?.PlayCoPilotComments(CopilotCommentsType.WrongDirection);
				}
			};
			_rallyCar.FixCar(state: true);
			RallyTriggersControl.RallySequenceData = _racingSequenceData;
			_triggersReady = RallyTriggersControl.Init().Preserve();
			RallyWeatherData currentWeatherData = GetCurrentWeatherData();
			RaceEnvironment.Apply(currentWeatherData.WeatherType, currentWeatherData.DayTimeType);
			SetCamera().Forget();
		}

		private void SetParkedSteeringLock(bool locked)
		{
			_managerUI.SetCarInputHardLock(locked);
			if (locked)
			{
				_carInputManager.ResetSteerAndWheelsPosition();
			}
		}

		private async UniTaskVoid SetCamera()
		{
			RallyState = RallyState.Garage;
			_racingCameraController.Init();
			_racingCameraController.SetTouchInputEnabled(value: true);
			_racingCameraController.PrepareCarReferences();
			if (_isDestroyed)
			{
				ResumeRainAfterLoading();
				return;
			}
			_racingCameraController.SetCameraForGarage(_rallyCar.gameObject);
			_racingCameraController.SetTouchInputEnabled(value: true);
			_managerUI.ShowWindow("RallyMenuWindow").Forget();
			RallyPassengerNpc.SpawnPassengerNpc(_rallyCar.gameObject).Forget();
			await UniTask.Delay(TimeSpan.FromSeconds(2.0));
			if (_isDestroyed)
			{
				ResumeRainAfterLoading();
				return;
			}
			_circuitRacingWaitingWindow.gameObject.SetActive(value: false);
			ResumeRainAfterLoading();
		}

		private void PauseRainWhileLoading()
		{
			if (!_rainPausedForLoading)
			{
				_rainPausedForLoading = true;
				_rainSound.Pause();
			}
		}

		private void ResumeRainAfterLoading()
		{
			if (_rainPausedForLoading)
			{
				_rainPausedForLoading = false;
				_rainSound.Resume();
			}
		}

		public async UniTaskVoid StartStage(bool force = false)
		{
			RunStaged?.Invoke();
			_racingCameraController.DisableAllCamera();
			await UniTask.Yield();
			_racingCameraController.setCamera(V.Car);
			await UniTask.Yield();
			GlobalEventSystem.ActivateInSceneProps(show: true);
			ResetRaceCts();
			if (_suppressNextStartLog)
			{
				_suppressNextStartLog = false;
			}
			else if (_rallyAnalytic != null && _rallySessionTracker != null)
			{
				int attemptNumber = _rallySessionTracker.NextAttempt(LocationIndex, TrajectoryIndex);
				int restartCount = _rallySessionTracker.GetRestartCount();
				_rallyAnalytic.LogStageStarted(BuildStageInfo(), attemptNumber, restartCount);
			}
			_currentRallySequenceUIController.ScrollToStart();
			_currentRallySequenceUIController.Show(state: true);
			if (force)
			{
				RallyPassengerNpc.ForcePassengerSeat(_rallyCar.gameObject);
			}
			RallyState = RallyState.Stage;
			_circuitRacingWaitingWindow.gameObject.SetActive(value: true);
			await _triggersReady;
			_rallyCar.FixCar(state: false);
			SetParkedSteeringLock(locked: false);
			await RallyCarControl.MoveToStagePoint();
			await UniTask.WaitUntil(() => !ScenesHelper.IsMenuSceneActive()).TimeoutWithoutException(TimeSpan.FromSeconds(8.0));
			PlayerDatas.UnloadResourcesForce();
			await UniTask.DelayFrame(3);
			_racingCameraController.SetCameraForRacing();
			_racingCameraController.SetTouchInputEnabled(value: true);
			_circuitRacingWaitingWindow.gameObject.SetActive(value: false);
			await ShowRallyPlayWindow();
			string text = new LocalizedString("RallyMode.GoToStPoint");
			_managerUI.ShowNotificationWindow(text, 3f).Forget();
			RallyTriggersControl.ActivateTriggers();
			_paceNotesSoundController?.Dispose();
			_paceNotesSoundController = new RallyPilotSoundController(base.gameObject, _racingSequenceData);
			int count = PlayerPrefs.GetInt("PaceNotesValue", 2);
			_paceNotesSoundController?.ChangePacenotesQueueCount(count);
			_timers.StartIdleCheck((V.Car != null) ? V.Car.GetComponent<Rigidbody>() : null);
			await UniTask.Yield();
			RallyTriggersControl.OnStageRace(RallyTriggersControl.StartMarshall);
			_timers.StartStageCountdown();
		}

		public void Pause(bool state)
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, state ? SoundEventsDataHelper.OffReverState : SoundEventsDataHelper.OnReverState);
			_racingCameraController?.SetTouchInputEnabled(!state);
			if (state)
			{
				Time.timeScale = 0f;
				_Car_Parking.Scripts.Audio.Audio.MuteExcludeUI();
			}
			else
			{
				Time.timeScale = 1f;
				_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
				ShowRallyPlayWindow(resume: true).Forget();
			}
			_currentRallySequenceUIController?.Show(!state);
		}

		public async UniTask ShowRallyPlayWindow(bool resume = false)
		{
			if (!_rallyPlayWindow)
			{
				_rallyPlayWindow = await _managerUI.CreateWindowGeneric<RallyPlayWindow>("RallyPlayWindow", onTop: false);
				_rallyPlayWindow.Init(this, RallyTriggersControl, _rallyCar);
			}
			else
			{
				_rallyPlayWindow.SetActive(active: true);
				if (resume)
				{
					_rallyPlayWindow.Resume();
				}
			}
			if (resume)
			{
				return;
			}
			_currentRallySequenceUIController.Init(RallyTriggersControl, RallySequenceData);
			RallySequenceUIController currentRallySequenceUIController = _currentRallySequenceUIController;
			currentRallySequenceUIController.OnNoteActivated = (Action<RallySequenceItem>)Delegate.Combine(currentRallySequenceUIController.OnNoteActivated, (Action<RallySequenceItem>)((RallySequenceItem item) =>
			{
				if (!_isDestroyed)
				{
					_paceNotesSoundController?.AnnounceItem(item);
					if (RallyTriggersControl.TotalCheckpoints - 1 - RallyTriggersControl.CachedActiveIndex == 3)
					{
						_paceNotesSoundController?.OnRaceFinish();
					}
				}
			}));
		}

		public void OnCrossFinishLine()
		{
			_paceNotesSoundController.PlayStopControl();
			_rallyCar.StopTimer();
			RallyTriggersControl.OnStageRace(RallyTriggersControl.FinishMarshall);
			_currentRallySequenceUIController.ShowSequence(show: false);
			_currentRallySequenceUIController.RestartStopIconProgress();
			_currentRallySequenceUIController.ShowStopIcon(state: true);
			Finish();
		}

		public void Finish()
		{
			if (RallyState != RallyState.Stage && !_finishTriggered)
			{
				_finishTriggered = true;
				_timers.StartIdleCheck((V.Car != null) ? V.Car.GetComponent<Rigidbody>() : null);
				RallyTriggersControl.OnCarNeedToStop(isRaceStarted: false);
				List<RallySequenceItem> rallySequenceItems = _racingSequenceData.RallySequenceItems;
				RallySequenceItem rallySequenceItem = ((rallySequenceItems.Count > 0) ? rallySequenceItems[rallySequenceItems.Count - 1] : null);
				if (rallySequenceItem != null)
				{
					WaitForCarStopAtPoint(rallySequenceItem.Position, 6f, OnRaceFinished, _raceCts.Token).Forget();
				}
				else
				{
					OnRaceFinished();
				}
				_timers.StartFinishCountdown(OnFinishTimeout);
			}
		}

		private void OnFinishTimeout()
		{
			if (RallyState != RallyState.Finish)
			{
				ResetRaceCts();
				_timers.StopIdleCheck();
				_rallyCar.FixCar(state: true);
				RallyState = RallyState.Finish;
				_paceNotesSoundController?.PlayFinishBadResult();
				_currentRallySequenceUIController.ShowSequence(show: false);
				_currentRallySequenceUIController.ShowStopIcon(state: false);
				ShowRaceFinishWindowAsync(disqualify: true, RallyDisqualifyType.NotFinished).Forget();
			}
		}

		public void OnCarCrossedStopLine()
		{
			if (_finishTriggered)
			{
				WaitForCarStopAtStopLine(OnRaceFinished, _raceCts.Token).Forget();
			}
		}

		private async UniTaskVoid WaitForCarStopAtStopLine(Action onStopped, CancellationToken ct)
		{
			if (V.Car == null)
			{
				return;
			}
			Rigidbody rb = V.Car.GetComponent<Rigidbody>();
			while (rb != null && !(rb.velocity.sqrMagnitude < 0.09f))
			{
				if (await UniTask.Delay(TimeSpan.FromMilliseconds(150.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow())
				{
					return;
				}
			}
			if (!_isDestroyed)
			{
				onStopped?.Invoke();
			}
		}

		public void SetCarLights()
		{
			RallyCarControl.SetLights();
		}

		[ContextMenu("FinishRally")]
		public void FinishWithMarshalPenalty()
		{
			if (RallyState == RallyState.Race)
			{
				_managerUI.ShowNotificationWindow($"CollideWithMarshall penalty {60f}", 3f, NotificationType.Error).Forget();
				_rallyCar.StopTimer();
				_rallyCar.AddPenalty(60f);
				ForceStopAndFinish().Forget();
			}
			else
			{
				Disqualify(RallyDisqualifyType.TouchMarshall);
			}
			List<RallySequenceItem> rallySequenceItems = RallySequenceData.RallySequenceItems;
			if (rallySequenceItems != null && rallySequenceItems.Count > 0)
			{
				RallySequenceItem rallySequenceItem = rallySequenceItems[rallySequenceItems.Count - 1];
				_rallyCar.SetPosition(rallySequenceItem.Position + Vector3.back * 3f, rallySequenceItem.Rotation);
			}
			else
			{
				_rallyCar.SetPosition(RallySequenceData.CarStagePoint.Position, RallySequenceData.CarStagePoint.Rotation);
			}
		}

		private async UniTaskVoid ForceStopAndFinish()
		{
			_rallyCar.FixCar(state: true);
			Transform transform = RallyTriggersControl.FinishMarker.transform;
			Vector3 vector = transform.position - transform.forward;
			vector.y = transform.position.y;
			await UniTask.Delay(TimeSpan.FromMilliseconds(100.0));
			if (!_isDestroyed)
			{
				OnRaceFinished();
			}
		}

		public void OnRaceFinished()
		{
			if (RallyState != RallyState.Finish)
			{
				DisposeProgressUI();
				_timers.StopFinishCountdown();
				PlayFinish();
				_rallyCar.ApplyTailMissedCheckpointsPenalty();
				RunFinished?.Invoke((_rallyCar != null) ? _rallyCar.TotalTime : 0f);
				_timers.StopIdleCheck();
				RallyState = RallyState.Finish;
				_rallyCar.FixCar(state: true);
				RunStageCompletedFlowAsync().Forget();
				ShowRaceFinishWindowAsync().Forget();
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQEw", "rally_stage");
				if (_rallyCar.Penalty <= 0f)
				{
					GooglePlayAchievements.MarkCleanSlateRally("rally_stage");
				}
				if (!_usedToRoad)
				{
					GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQJg", "rally_stage");
				}
				if (_rallyCar.MissedCheckpoints <= 0)
				{
					GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQJw", "rally_stage");
				}
				CheckSteamAchievements();
			}
		}

		private void CheckSteamAchievements()
		{
			if (!_rallyCar)
			{
				return;
			}
			IReadOnlyList<PenaltyRecord> penaltyLog = _rallyCar.PenaltyLog;
			bool flag = false;
			for (int i = 0; i < penaltyLog.Count; i++)
			{
				if (penaltyLog[i].Type == PenaltyType.ReturnToRoad)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				_steamAchievementService?.Unlock("ACH_TRUST_THE_CODRIVER");
			}
			if (_rallyCar.MissedCheckpoints <= 0)
			{
				_steamAchievementService?.Unlock("ACH_NO_NOTES_MISSED");
			}
		}

		private void PlayFinish()
		{
			if (!(_rallyCar == null) && _paceNotesSoundController != null)
			{
				RallyProgress rallyProgress = _rallyProgressController?.GetProgress(LocationIndex, TrajectoryIndex);
				bool flag = rallyProgress == null || rallyProgress.PersonalBest <= 0f || _rallyCar.TotalTime < rallyProgress.PersonalBest;
				if (flag && _steamAchievementService != null)
				{
					_steamAchievementService.Unlock("ACH_CHECKERED_HELLO");
					_steamAchievementService.MarkCleanSlateRally();
				}
				_paceNotesSoundController.PlayFinishGoodResult(flag);
			}
		}

		private async UniTaskVoid ShowRaceFinishWindowAsync(bool disqualify = false, RallyDisqualifyType disqualifyType = RallyDisqualifyType.StopCar)
		{
			SetParkedSteeringLock(locked: true);
			DisposeProgressUI();
			_currentRallySequenceUIController?.ShowStopIcon(state: false);
			RallyFinishWindow rallyFinishWindow = ((!disqualify) ? (await _managerUI.CreateWindowGeneric<RallyFinishWindow>("RallyFinishWindow", onTop: false)) : (await _managerUI.CreateWindowGeneric<RallyFinishWindow>("RallyDisqualWindow", onTop: false)));
			if (!_isDestroyed)
			{
				_rallyPlayWindow.Close();
				rallyFinishWindow.Init(this, _rallyCar, disqualify);
				if (disqualify)
				{
					rallyFinishWindow.SetDisqualifiedReason(disqualifyType);
				}
			}
		}

		public void OnCarEnteredStartZone()
		{
			if (_startRaceTriggered)
			{
				return;
			}
			_timers.StopIdleCheck();
			_currentRallySequenceUIController.ShowStopIcon(state: true);
			WaitForCarStop(() =>
			{
				if (!_startRaceTriggered)
				{
					_startRaceTriggered = true;
					StartRace().Forget();
				}
			}, _raceCts.Token).Forget();
		}

		public void OnMarshalHit()
		{
			if (_startRaceTriggered)
			{
				Disqualify(RallyDisqualifyType.TouchMarshall);
				return;
			}
			_startRaceTriggered = true;
			_rallyCar.FixCar(state: true);
			StartRace().Forget();
		}

		public async UniTaskVoid StartRace()
		{
			CancellationToken ct = _raceCts.Token;
			_timers.StopStageCountdown();
			_timers.StopIdleCheck();
			_rallyCar.FixCar(state: true);
			_currentRallySequenceUIController.ShowStopIcon(state: false);
			_currentRallySequenceUIController.OnRaceStarted();
			RallyState = RallyState.Race;
			_usedToRoad = false;
			_lapRaceTrafficLightUI.gameObject.SetActive(value: true);
			RallyTriggersControl.OnStartRace();
			if ((GhostReadyProvider == null || (!(await GhostReadyProvider(ct).SuppressCancellationThrow()) && !_isDestroyed)) && !(await _lapRaceTrafficLightUI.StartLights().AttachExternalCancellation(ct).SuppressCancellationThrow()) && !_isDestroyed)
			{
				_rallyCar.FixCar(state: false);
				_rallyCar.StartTimer();
				RunStarted?.Invoke();
				DisposeProgressUI();
				if (RacingProgressUIControllerPrefab != null)
				{
					_racingProgressUI = _prefabFactory.Instantiate(RacingProgressUIControllerPrefab, _managerUI.transform);
					_progressUIDriver = new RallyProgressUIDriver(_racingProgressUI, _rallyCar, RallyTriggersControl, GhostTransformProvider);
				}
				_paceNotesSoundController?.OnRaceStart();
				_lapRaceTrafficLightUI.gameObject.SetActive(value: false);
				_timers.StartIdleCheck((V.Car != null) ? V.Car.GetComponent<Rigidbody>() : null);
				if (!(await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow()) && !_isDestroyed)
				{
					_rallyPlayWindow?.OnRaceStarted();
					_currentRallySequenceUIController.ShowSequence(show: true);
				}
			}
		}

		public void MoveToPoint(bool start)
		{
			Vector3 position = RallyTriggersControl.StartMarkerPlace.transform.position;
			position.y = _rallyCar.transform.position.y + 0.1f;
			if (start)
			{
				_rallyCar.SetPosition(position, RallyTriggersControl.StartMarkerPlace.transform.rotation);
			}
			else
			{
				RallyCarControl.MoveToStopPoint();
			}
		}

		private async UniTaskVoid WaitForCarStop(Action onStopped, CancellationToken ct)
		{
			if (V.Car == null)
			{
				return;
			}
			Rigidbody rb = V.Car.GetComponent<Rigidbody>();
			Vector3 startPoint = RallyTriggersControl.StartMarkerPlace.transform.position;
			startPoint.y = 0f;
			bool isAnimPlayed = false;
			while (rb != null)
			{
				Vector3 position = V.Car.transform.position;
				position.y = 0f;
				bool num = (position - startPoint).sqrMagnitude < 36f;
				bool flag = rb.velocity.sqrMagnitude < 0.09f;
				if (num)
				{
					if (!isAnimPlayed)
					{
						isAnimPlayed = true;
						RallyTriggersControl.OnCarNeedToStop(isRaceStarted: true);
					}
					if (flag)
					{
						break;
					}
				}
				if (await UniTask.Delay(TimeSpan.FromMilliseconds(150.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow())
				{
					return;
				}
			}
			if (!_isDestroyed)
			{
				onStopped?.Invoke();
			}
		}

		private bool IsInsideStopZone(Vector3 flatCarPos, Vector3 flatPoint, float radius)
		{
			if ((flatCarPos - flatPoint).sqrMagnitude < radius * radius)
			{
				return true;
			}
			RallySequenceUIController currentRallySequenceUIController = _currentRallySequenceUIController;
			if (currentRallySequenceUIController != null)
			{
				return currentRallySequenceUIController.StopIconFill >= 0.7f;
			}
			return false;
		}

		private async UniTaskVoid WaitForCarStopAtPoint(Vector3 point, float radius, Action onStopped, CancellationToken ct)
		{
			if (V.Car == null)
			{
				return;
			}
			Rigidbody rb = V.Car.GetComponent<Rigidbody>();
			point.y = 0f;
			while (rb != null)
			{
				Vector3 position = V.Car.transform.position;
				position.y = 0f;
				bool num = IsInsideStopZone(position, point, radius);
				bool flag = rb.velocity.sqrMagnitude < 0.09f;
				if (num)
				{
					RallyTriggersControl.OnCarNeedToStop(isRaceStarted: false);
				}
				if (num & flag)
				{
					break;
				}
				if (await UniTask.Delay(TimeSpan.FromMilliseconds(150.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow())
				{
					return;
				}
			}
			if (!_isDestroyed)
			{
				onStopped?.Invoke();
			}
		}

		public void ShowMainMenu()
		{
			_managerUI.ShowConfirmWindow(() =>
			{
				_menuLevelParent.OpenMainMenu();
			}, new LocalizedString("DragRacingUI.DoYouWantToExit"), new LocalizedString("DragRacingUI.AllYourProgressWillBeLost"), Resume).Forget();
		}

		public void Disqualify(RallyDisqualifyType type = RallyDisqualifyType.StopCar, bool moveToStartPoint = false)
		{
			if (!_finishTriggered)
			{
				_finishTriggered = true;
				RunAborted?.Invoke();
				DisposeProgressUI();
				_timers.StopIdleCheck();
				_rallyCar.StopTimer();
				_rallyCar.FixCar(state: true);
				if (moveToStartPoint)
				{
					_rallyCar.SetPosition(RallySequenceData.CarStagePoint.Position, RallySequenceData.CarStagePoint.Rotation);
				}
				_paceNotesSoundController?.PlayFinishBadResult();
				_currentRallySequenceUIController.ShowSequence(show: false);
				_currentRallySequenceUIController.ShowStopIcon(state: false);
				RallyState = RallyState.Finish;
				if (_rallyAnalytic != null && _rallySessionTracker != null)
				{
					int attempt = _rallySessionTracker.GetAttempt(LocationIndex, TrajectoryIndex);
					int restartCount = _rallySessionTracker.GetRestartCount();
					_rallyAnalytic.LogStageFailed(BuildStageInfo(), RallyAnalyticsHelper.MapDisqualify(type), attempt, restartCount);
				}
				ShowRaceFinishWindowAsync(disqualify: true, type).Forget();
			}
		}

		private void OnDestroy()
		{
			DisposeProgressUI();
			RaceEnvironment.RestoreTimeSettings(_defaultTimeSettings);
			_raceCts?.Cancel();
			_raceCts?.Dispose();
			_timers.Dispose();
			_paceNotesSoundController?.Dispose();
			RaceModeController raceModeController = _raceModeController;
			raceModeController.ShowMainMenuAction = (Action)Delegate.Remove(raceModeController.ShowMainMenuAction, new Action(ShowMainMenu));
			if (_managerUI != null)
			{
				_managerUI.SetCarInputHardLock(locked: false);
			}
			_isDestroyed = true;
			ResumeRainAfterLoading();
			_soundBankLoadService?.TryUnloadBank(SoundBankType.CpmEventVoices);
			if (_racingProvider != null && _racingProvider.RallyController == this)
			{
				_racingProvider.RallyController = null;
			}
		}

		public void ToRoad()
		{
			if (RallyState != RallyState.Race)
			{
				_rallyCar?.SetPosition(RallySequenceData.CarStagePoint.Position, RallySequenceData.CarStagePoint.Rotation);
				return;
			}
			_usedToRoad = true;
			_rallyCar?.ReturnToRoad(10f, PenaltyType.ReturnToRoad);
		}

		public void ExitToMainMenu()
		{
			_menuLevelParent.OpenMainMenu();
			RainWetnessDriver.ClearForGarage();
		}

		public void GoToGarage()
		{
			Time.timeScale = 1f;
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
			DisposeProgressUI();
			ResetRaceCts();
			DisableLapRaceTrafficLightUI();
			_timers.StopIdleCheck();
			_timers.StopStageCountdown();
			_timers.StopFinishCountdown();
			_startRaceTriggered = false;
			_finishTriggered = false;
			_paceNotesSoundController?.Reset();
			RecreateSequenceUIController();
			_rallyCar.ResetTimer();
			RallyTriggersControl.RestartRace();
			RallyState = RallyState.Garage;
			_rallyCar.FixCar(state: true);
			SetParkedSteeringLock(locked: true);
			_rallyCar.SetPosition(RallySequenceData.CarStartGaragePoint.Position, RallySequenceData.CarStartGaragePoint.Rotation);
			_racingCameraController.SetTouchInputEnabled(value: true);
			_racingCameraController.SetCameraForGarage(_rallyCar.gameObject);
			_racingCameraController.SetTouchInputEnabled(value: true);
			_managerUI.ShowWindow("RallyMenuWindow").Forget();
		}

		public void DisableLapRaceTrafficLightUI()
		{
			_lapRaceTrafficLightUI.gameObject.SetActive(value: false);
		}

		public RallyMapDataSO GetMapData()
		{
			return RallyMapDataSo;
		}

		public void Resume()
		{
			Pause(state: false);
		}

		public RallyStageInfo BuildStageInfo()
		{
			string trajectoryId = LocationIndex.ToString();
			string locationName = string.Empty;
			if (RallyMapDataSo != null && LocationIndex >= 0 && LocationIndex < RallyMapDataSo.RallyLocationDatas.Count && RallyMapDataSo.RallyLocationDatas[LocationIndex] != null)
			{
				RallyLocationData rallyLocationData = RallyMapDataSo.RallyLocationDatas[LocationIndex];
				locationName = rallyLocationData.AnalyticsId;
				string nameKey = rallyLocationData.NameKey;
				if (!string.IsNullOrEmpty(nameKey))
				{
					int num = nameKey.LastIndexOf('.');
					trajectoryId = ((num >= 0 && num + 1 < nameKey.Length) ? nameKey.Substring(num + 1) : nameKey);
				}
			}
			string rallyCarId = GetRallyCarId();
			string currentCarClass = GetCurrentCarClass();
			return new RallyStageInfo
			{
				ModeType = RallyModeType.single_company,
				LocationIndex = LocationIndex,
				TrajectoryIndex = TrajectoryIndex,
				StageIndex = (_racingSequenceData?.Index ?? 0),
				IsNight = (DayTimeType == DayTimeType.Night),
				TrajectoryId = trajectoryId,
				LocationName = locationName,
				TyreType = GetCurrentTyre(),
				CarId = rallyCarId,
				CarClass = currentCarClass,
				CheckpointPenaltyTotal = ((_rallyCar != null) ? _rallyCar.Penalty : 0f),
				NgPlusLevel = (int)(_rallyProgressController?.RallyData?.NgCount).GetValueOrDefault(),
				Special = (_racingSequenceData?.IsSpecial ?? false)
			};
		}

		private string GetRallyCarId()
		{
			if (V.Car != null && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				return component.CarId.ToString();
			}
			MainCarValues issuedRallyCarValues = GetIssuedRallyCarValues();
			if (!(issuedRallyCarValues != null))
			{
				return string.Empty;
			}
			return issuedRallyCarValues.CarID.ToString();
		}

		private MainCarValues GetIssuedRallyCarValues()
		{
			if (RallyCarsDatasSo == null || RallyMapDataSo == null)
			{
				return null;
			}
			if (!RallyMapDataSo.TryGetConfigIds(LocationIndex, TrajectoryIndex, out var locationId, out var trajectoryId))
			{
				return null;
			}
			return RallyCarsDatasSo.GetRallyCarData(locationId, trajectoryId, _allCarsInfo)?.MainCarValues;
		}

		public bool IsDrivingIssuedRallyCar()
		{
			MainCarValues issuedRallyCarValues = GetIssuedRallyCarValues();
			if (issuedRallyCarValues == null || V.Car == null || !V.Car.TryGetComponent<VehicleData>(out var component))
			{
				return false;
			}
			MainCarValues carValues = component.CarValues;
			if (carValues != null && carValues.CarID == issuedRallyCarValues.CarID)
			{
				return carValues.Class == issuedRallyCarValues.Class;
			}
			return false;
		}

		private string GetCurrentCarClass()
		{
			MainCarValues issuedRallyCarValues = GetIssuedRallyCarValues();
			if (issuedRallyCarValues != null)
			{
				_lastCarClass = issuedRallyCarValues.Class.ToString();
			}
			return _lastCarClass;
		}

		private string GetCurrentTyre()
		{
			if (V.Car != null && V.Car.TryGetComponent<VehicleEntityProvider>(out var component) && component.CarEntity != null)
			{
				return RallyAnalyticsHelper.TyreTypeToCanonical(component.CarEntity.FrontWheel.tyreType);
			}
			return "touring";
		}

		private StageSnapshot CaptureStageSnapshot()
		{
			RallyProgress progress = _rallyProgressController.GetProgress(LocationIndex, TrajectoryIndex);
			return new StageSnapshot
			{
				PrevMedal = RallyAnalyticsHelper.MapMedalTier(progress?.RallyTier ?? 0),
				PrevPersonalBest = (progress?.PersonalBest ?? 0f),
				HasPrevBest = (progress != null && progress.PersonalBest > 0f)
			};
		}

		private async UniTaskVoid RunStageCompletedFlowAsync()
		{
			StageSnapshot snapshot = CaptureStageSnapshot();
			if (_rallyAnalytic != null && _rallySessionTracker != null && _racingSequenceData != null)
			{
				RallyStageInfo info = BuildStageInfo();
				RallyStageResult result = BuildStageResult(snapshot, out var improved);
				int attempt = _rallySessionTracker.GetAttempt(LocationIndex, TrajectoryIndex);
				int restart = _rallySessionTracker.GetRestartCount();
				float totalTime = _rallyCar.TotalTime;
				RallyProgressResult raceResult = null;
				try
				{
					raceResult = await _rallyProgressController.SetProgress(totalTime);
				}
				catch
				{
				}
				bool awardSpecific = raceResult != null && raceResult.AwardSpecific == 1;
				result.RewardId = (improved ? RallyAnalyticsHelper.BuildRewardJson(snapshot.PrevMedal, result.MedalType, _racingSequenceData.RallyTimes, awardSpecific, _racingSequenceData.Reward) : "[]");
				_rallyAnalytic.LogStageCompleted(info, result, attempt, restart);
			}
		}

		private RallyStageResult BuildStageResult(StageSnapshot snapshot, out bool improved)
		{
			int missedCheckpoints = _rallyCar.MissedCheckpoints;
			float raceTime = _rallyCar.RaceTime;
			float penalty = _rallyCar.Penalty;
			float num = raceTime + penalty;
			RallyMedalType rallyMedalType = RallyAnalyticsHelper.MapLapTimeToMedal(num, _racingSequenceData.RallyTimes);
			bool isPersonalRecord = !snapshot.HasPrevBest || num < snapshot.PrevPersonalBest;
			float requiredValue = ((_racingSequenceData.RallyTimes != null && _racingSequenceData.RallyTimes.Length > 2) ? _racingSequenceData.RallyTimes[2].TargetTime : 0f);
			improved = rallyMedalType > snapshot.PrevMedal;
			return new RallyStageResult
			{
				LapTime = raceTime,
				RequiredValue = requiredValue,
				IsPersonalRecord = isPersonalRecord,
				MedalType = rallyMedalType,
				PrevMedalType = snapshot.PrevMedal,
				CheckpointMissed = missedCheckpoints,
				CheckpointPenaltyTotal = penalty
			};
		}
	}
}
