using System;
using System.Threading;
using CPM.Checkpoints;
using CPM.Connection;
using CPM.Fallout;
using CPM.Obstacles;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Base;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Arena.UI;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.ToRoad;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceArenaMode : IArenaModeController, IArenaToRoadPolicy, IToRoadOverridable
	{
		private const int NotOnPodium = 0;

		private ObstacleRaceGameModeData _data;

		private IArenaContext _ctx;

		private TriggerRelay _finishRelay;

		private TriggerRelay _startRelay;

		private ObstacleRaceTrack _track;

		private ObstacleRaceGameplayWindow _window;

		private ObstacleRaceResultWindow _resultWindow;

		private readonly LobbyRoster _roster;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly CommonProvider _commonProvider;

		private readonly OnlineCollectionsProvider _online;

		private ObstacleDriver _obstacleDriver;

		private CheckpointCourse _course;

		private ObstacleRaceStandingsDriver _standings;

		private ObstacleRaceSpectatorDriver _spectator;

		private ObstacleRacePodiumMirror _podiumMirror;

		private ArenaRagdollSound _ragdollSound;

		private readonly SurvivalPlacementLedger _ledger;

		private readonly ArenaRaceFinishReporter _finish;

		private readonly ObstacleRaceMusic _music;

		private readonly ToRoadOverrideSlot _returnPoint = new ToRoadOverrideSlot();

		private readonly ObstacleRaceRunTracker _analyticsRun;

		private readonly ObstacleRaceEventProgressWatcher _eventProgress;

		private int? _lastProgress;

		private UniTaskCompletionSource _resolved;

		private bool _finished;

		private bool _startTouched;

		private bool _outcomePlayed;

		private int _podiumPlace;

		private int _myActor;

		public ArenaModeType ModeType => ArenaModeType.ObstacleRace;

		public ObstacleRaceArenaMode(LobbyRoster roster, FreeDriveProvider freeDriveProvider, CommonProvider commonProvider, OnlineCollectionsProvider online, FalloutRaceSession raceSession, ObstacleRaceMusic music, ObstacleRaceRunTracker analyticsRun, IEventService eventService)
		{
			_roster = roster;
			_freeDriveProvider = freeDriveProvider;
			_commonProvider = commonProvider;
			_online = online;
			_music = music;
			_analyticsRun = analyticsRun;
			_ledger = new SurvivalPlacementLedger(roster);
			_finish = new ArenaRaceFinishReporter(raceSession, ModeType);
			_eventProgress = new ObstacleRaceEventProgressWatcher(eventService);
		}

		public void Enter()
		{
			_ragdollSound = new ArenaRagdollSound();
			CPM.Connection.MultiplayerSettings.UsePersonSnapshotInterpolation = true;
			RaceEnvironment.RequestForNextScene(WeatherType.Clear, DayTimeType.Day);
		}

		public async UniTask Init(IArenaContext ctx, CancellationToken ct)
		{
			_ctx = ctx;
			_ledger.ClearOwnClaim(ctx);
			if ((bool)V.SceneControl)
			{
				V.SceneControl.checkEnvironmentForLoad = false;
			}
			await V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true);
			await UniTask.WaitUntil(() => (bool)V.DriverPerson && FlagsStorage.OutOfCar && FlagsStorage.FirstMainSceneLoaded.Value, PlayerLoopTiming.Update, ct);
			ArenaWorldRestricts.Suppress();
			await UniTask.WaitUntil(() => _data, PlayerLoopTiming.Update, ct);
			await UniTask.WaitUntil(() => _data.FinishTrigger, PlayerLoopTiming.Update, ct);
			ArenaLocalPerson.Freeze(frozen: true);
			(ObstacleRaceTrack, CheckpointCourse) tuple = await ObstacleRaceCourseAssembler.AssembleAsync(ctx, _data, _commonProvider?.PrefabFactory, ct);
			_track = tuple.Item1;
			_course = tuple.Item2;
			_finishRelay = _data.FinishTrigger.gameObject.EnsureComponent<TriggerRelay>();
			if ((bool)_data.StartTrigger)
			{
				_startRelay = _data.StartTrigger.gameObject.EnsureComponent<TriggerRelay>();
			}
			Obstacle[] componentsInChildren = _data.GetComponentsInChildren<Obstacle>();
			if (componentsInChildren.Length != 0)
			{
				double matchStart = 0.0;
				_obstacleDriver = new ObstacleDriver(componentsInChildren, () =>
				{
					if (matchStart <= 0.0)
					{
						matchStart = ctx.MatchStartTime;
					}
					return (!(matchStart > 0.0)) ? 0.0 : (ctx.ServerTime - matchStart);
				});
			}
			ArenaLocalPerson.PlaceOnSpawn(_data.SpawnLayoutProvider, ctx, resyncNetwork: true);
		}

		public async UniTask PreStartAsync(IArenaContext ctx, float duration, CancellationToken ct)
		{
			float showWindowTime = Time.time;
			_window = (ObstacleRaceGameplayWindow)(await V.CoreGameManager.ManagerUI.ShowWindow("ObstacleRaceGameplayWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, null, withAnimate: false, updateBackButton: true, trackForCleanup: false));
			V.CoreGameManager.ManagerUI.CloseWaitingWindow();
			RefreshCheckpointProgress();
			if ((bool)_window)
			{
				_window.SetStandings(Array.Empty<ChartListRowData>(), 1, ctx.ParticipantActorNumbers.Count);
				_window.SetCollapseTimer(-1f);
			}
			duration -= Time.time - showWindowTime;
			if ((bool)_window && duration > 0f)
			{
				await _window.PlayCountdownAsync(duration, ct);
			}
			_music.Play();
			if ((bool)_window)
			{
				_window.ShowProgressAndChartListWithDelay();
			}
		}

		public void Clear()
		{
			_analyticsRun.Leave(_standings?.LastProgress ?? _lastProgress);
			_ledger.ClearOwnClaim(_ctx);
			_music.Stop();
			BaseCommonExtensions.DisposeAndClear(ref _ragdollSound);
			CPM.Connection.MultiplayerSettings.UsePersonSnapshotInterpolation = false;
			RaceEnvironment.CancelPendingRequest();
			ArenaWorldRestricts.Restore();
			if ((bool)V.CoreGameManager)
			{
				V.CoreGameManager.ManagerUI.CloseWaitingWindow();
			}
			ArenaLocalPerson.Freeze(frozen: false);
			BaseCommonExtensions.DisposeAndClear(ref _track);
			BaseCommonExtensions.DisposeAndClear(ref _obstacleDriver);
			BaseCommonExtensions.DisposeAndClear(ref _course);
			BaseCommonExtensions.DisposeAndClear(ref _standings);
			BaseCommonExtensions.DisposeAndClear(ref _spectator);
			_podiumMirror = null;
			_data = null;
			_ctx = null;
			_finishRelay = null;
			_startRelay = null;
			_window = null;
			_resultWindow = null;
			_ledger.Reset();
			_analyticsRun.Reset();
			_eventProgress.Reset();
			_lastProgress = null;
			_finish.Reset();
			_returnPoint.Clear();
			_finished = false;
			_startTouched = false;
			_outcomePlayed = false;
			_podiumPlace = 0;
		}

		public void SetData(ObstacleRaceGameModeData data)
		{
			_data = data;
		}

		public void SetOverride(IToRoadHandler handler)
		{
			_returnPoint.Set(handler);
		}

		public bool TryResolveToRoad(in ToRoadRequest request, out ToRoadResult result)
		{
			result = ToRoadResult.NotHandled();
			if (!request.MovesPerson || !_data)
			{
				return false;
			}
			if (!_finished)
			{
				_analyticsRun.Respawn();
			}
			if (_finished && _ctx != null)
			{
				if ((bool)_data.PodiumOnline)
				{
					PositionsAndRotation positionsAndRotation = EnterPodium(_ctx);
					result = ToRoadResult.Place(positionsAndRotation.Position, positionsAndRotation.Rotation, loadEnvironment: false);
					return true;
				}
			}
			else if (_returnPoint.TryResolve(in request, out result))
			{
				return true;
			}
			if (!ArenaLocalPerson.TryGetSpawnPose(_data.SpawnLayoutProvider, _ctx, out var pose))
			{
				return false;
			}
			result = ToRoadResult.Place(pose.Position, pose.Rotation, loadEnvironment: false);
			return true;
		}

		public async UniTask RunAsync(IArenaContext ctx, CancellationToken ct)
		{
			Debug.Log("[Arena] ObstacleRaceArenaMode started (InProgress).");
			ArenaLocalPerson.Freeze(frozen: false);
			_myActor = PhotonNetwork.LocalPlayer.ActorNumber;
			_resolved = new UniTaskCompletionSource();
			_ledger.Begin(ctx, _myActor, ct);
			_analyticsRun.Go(ctx, BuildTrackInfo(ctx), _course?.Tracker);
			_eventProgress.Begin();
			if ((bool)_data && (bool)_data.PodiumOnline)
			{
				_podiumMirror = new ObstacleRacePodiumMirror(ctx, _online, _data.PodiumOnline, () => _ledger.ReadBoard(ctx), _myActor);
			}
			if ((bool)_track?.StandingsRoute && (bool)_window)
			{
				_standings = new ObstacleRaceStandingsDriver(_track.StandingsRoute, ctx, _roster, _online, _myActor, () => _ledger.ReadBoard(ctx), _ledger.ClaimTimeOf);
				_standings.Updated += OnStandingsUpdated;
			}
			CheckpointTracker tracker = _course?.Tracker;
			ctx.OnModeStateChanged += OnStateChanged;
			_finishRelay.OnTriggerEntered += OnFinishTriggerEntered;
			if ((bool)_startRelay)
			{
				_startRelay.OnTriggerEntered += OnStartTriggerEntered;
			}
			RagdollCPM.LocalRagdollStarted += _music.PlayerHit;
			if (tracker != null)
			{
				tracker.Advanced += OnCheckpointAdvanced;
			}
			OnStateChanged();
			try
			{
				await _resolved.Task.AttachExternalCancellation(ct);
				if ((bool)_freeDriveProvider.FreeDriveControl && (bool)_freeDriveProvider.FreeDriveControl.ParkingMenuUI)
				{
					_freeDriveProvider.FreeDriveControl.ParkingMenuUI.Hide(hideChat: false);
				}
				if ((bool)_window)
				{
					_window.Close();
				}
				_resultWindow = (ObstacleRaceResultWindow)(await V.CoreGameManager.ManagerUI.ShowWindow("ObstacleRaceResultWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false));
				if ((bool)_resultWindow)
				{
					_resultWindow.Init(_ctx.RaceId, (_ctx.Config as ObstacleRaceConfig).GetRewards(), _ctx.IsFriendsRoom);
					_resultWindow.SetRows(_ledger.BuildStandings(ctx));
					_spectator = new ObstacleRaceSpectatorDriver(ctx, _roster, _online, EnsureSpectatorCamera(), _resultWindow.Spectator, () => _ledger.ReadBoard(ctx), RestoreCameraAfterSpectate, _standings);
					WindowScriptAsyncResult<ArenaResultAction>.ResultData result = await _resultWindow.GetResultAsync(ct);
					if (_eventProgress.TryGetGain(out var gained))
					{
						ObstacleRaceEventProgressScreen obstacleRaceEventProgressScreen = (ObstacleRaceEventProgressScreen)(await V.CoreGameManager.ManagerUI.ShowWindow("ObstacleRaceResultEventProgressWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false));
						if ((bool)obstacleRaceEventProgressScreen)
						{
							obstacleRaceEventProgressScreen.Init(gained);
							await obstacleRaceEventProgressScreen.GetResultAsync(ct);
						}
					}
					if ((bool)_resultWindow)
					{
						_resultWindow.Close();
					}
					if (result.Payload == ArenaResultAction.Retry)
					{
						ctx.RequestRestart();
					}
				}
			}
			finally
			{
				_lastProgress = _standings?.LastProgress ?? _lastProgress;
				BaseCommonExtensions.DisposeAndClear(ref _spectator);
				BaseCommonExtensions.DisposeAndClear(ref _obstacleDriver);
				if (_standings != null)
				{
					_standings.Updated -= OnStandingsUpdated;
				}
				BaseCommonExtensions.DisposeAndClear(ref _standings);
				ctx.OnModeStateChanged -= OnStateChanged;
				if ((bool)_finishRelay)
				{
					_finishRelay.OnTriggerEntered -= OnFinishTriggerEntered;
				}
				if ((bool)_startRelay)
				{
					_startRelay.OnTriggerEntered -= OnStartTriggerEntered;
				}
				RagdollCPM.LocalRagdollStarted -= _music.PlayerHit;
				if (tracker != null)
				{
					tracker.Advanced -= OnCheckpointAdvanced;
				}
			}
			ctx.Complete();
			void OnCheckpointAdvanced()
			{
				RefreshCheckpointProgress();
				RefreshMusicIntensity();
			}
			void OnFinishTriggerEntered(Collider other, TriggerRelay source)
			{
				if (IsLocalPerson(other) && !_finished)
				{
					_finished = true;
					ArenaLocalPerson.Freeze(frozen: true);
					ClaimPlace(ctx);
					if ((bool)_data && (bool)_data.PodiumOnline)
					{
						PositionsAndRotation positionsAndRotation = EnterPodium(ctx);
						ArenaLocalPerson.PlaceAt(positionsAndRotation.Position, positionsAndRotation.Rotation, updateCamera: false, resyncNetwork: true);
					}
					_resolved?.TrySetResult();
				}
			}
			void OnStandingsUpdated(ObstacleRaceStandingsUpdate update)
			{
				if ((bool)_window)
				{
					_window.ApplyStandings(in update);
				}
			}
			void OnStartTriggerEntered(Collider other, TriggerRelay source)
			{
				if (!_startTouched && IsLocalPerson(other))
				{
					_startTouched = true;
					_music.SetLowIntensity();
				}
			}
			void OnStateChanged()
			{
				_ledger.Reconcile(ctx);
				TakePodiumSlot(ctx);
				if ((bool)_resultWindow)
				{
					_resultWindow.SetRows(_ledger.BuildStandings(ctx));
				}
				_spectator?.OnStateChanged();
				_podiumMirror?.OnStateChanged();
				_finish.TryReport(ctx, () => _ledger.PlaceOf(ctx));
				_analyticsRun.TryFinish(() => _ledger.PlaceOf(ctx));
				PlayOutcomeMusic(ctx);
			}
		}

		private static bool IsLocalPerson(Collider other)
		{
			return (other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject) == V.DriverPerson;
		}

		private void RefreshCheckpointProgress()
		{
			if ((bool)_window)
			{
				CheckpointTracker checkpointTracker = _course?.Tracker;
				_window.SetCheckpointProgress(checkpointTracker?.Passed ?? 0, checkpointTracker?.Total ?? 0);
			}
		}

		private ObstacleRaceTrackInfo BuildTrackInfo(IArenaContext ctx)
		{
			return new ObstacleRaceTrackInfo(_track?.Seed, _track?.Plan, ctx.ParticipantActorNumbers?.Count ?? 0);
		}

		private void RefreshMusicIntensity()
		{
			CheckpointTracker checkpointTracker = _course?.Tracker;
			if (checkpointTracker != null && checkpointTracker.Total > 0 && checkpointTracker.Last >= 0)
			{
				if ((checkpointTracker.Last + 1) * 2 >= checkpointTracker.Total)
				{
					_music.SetHighIntensity();
				}
				else
				{
					_music.SetLowIntensity();
				}
			}
		}

		private void PlayOutcomeMusic(IArenaContext ctx)
		{
			if (_finished && !_outcomePlayed)
			{
				int num = _ledger.PlaceOf(ctx);
				if (num > 0)
				{
					_outcomePlayed = true;
					int num2 = ((ctx.Config is ObstacleRaceConfig obstacleRaceConfig) ? obstacleRaceConfig.RewardsCount : 0);
					_music.SetOutcome(num <= num2);
				}
			}
		}

		private void ClaimPlace(IArenaContext ctx)
		{
			if (_ledger.TryClaim(ctx, out var elapsed))
			{
				_finish.CaptureTime(elapsed);
				_finish.TryReport(ctx, () => _ledger.PlaceOf(ctx));
				_analyticsRun.CaptureFinish(elapsed);
				_analyticsRun.TryFinish(() => _ledger.PlaceOf(ctx));
				PlayOutcomeMusic(ctx);
			}
		}

		private PositionsAndRotation EnterPodium(IArenaContext ctx)
		{
			_podiumPlace = _ledger.PlaceOf(ctx);
			ArenaLocalPerson.MountCameraOn(_data.PodiumOnline.CameraMount);
			return _data.PodiumOnline.GetPlacePose(_podiumPlace, ctx.ParticipantActorNumbers.Count);
		}

		private void TakePodiumSlot(IArenaContext ctx)
		{
			if (_podiumPlace == -1 && (bool)_data && (bool)_data.PodiumOnline)
			{
				int num = _ledger.PlaceOf(ctx);
				if (num > 0)
				{
					_podiumPlace = num;
					PositionsAndRotation placePose = _data.PodiumOnline.GetPlacePose(num, ctx.ParticipantActorNumbers.Count);
					ArenaLocalPerson.PlaceAt(placePose.Position, placePose.Rotation, updateCamera: false, resyncNetwork: true);
				}
			}
		}

		private static ArenaSpectatorCamera EnsureSpectatorCamera()
		{
			if (!V.CoreGameManager)
			{
				return null;
			}
			return V.CoreGameManager.gameObject.EnsureComponent<ArenaSpectatorCamera>();
		}

		private void RestoreCameraAfterSpectate()
		{
			TouchMovePerson component;
			if ((bool)_data && (bool)_data.PodiumOnline && (bool)_data.PodiumOnline.CameraMount)
			{
				ArenaLocalPerson.MountCameraOn(_data.PodiumOnline.CameraMount);
			}
			else if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<TouchMovePerson>(out component))
			{
				component.UpdateCamera = true;
				component.SnapCameraBehind(V.DriverPerson.transform.rotation);
			}
		}

		bool IArenaToRoadPolicy.TryResolveToRoad(in ToRoadRequest request, out ToRoadResult result)
		{
			return TryResolveToRoad(in request, out result);
		}
	}
}
