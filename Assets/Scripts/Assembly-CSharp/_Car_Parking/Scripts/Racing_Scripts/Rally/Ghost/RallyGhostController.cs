using System;
using System.Threading;
using CPM.Ghost;
using CPM.Ghost.Core;
using CPM.RacingRecords;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	public sealed class RallyGhostController : IInitializable, IDisposable
	{
		private readonly GhostSystem _system;

		private readonly GhostStorage _storage;

		private readonly GhostConfig _config;

		private readonly RaceModeController _raceModeController;

		private readonly RallyRecordsSync _records;

		private readonly RallyRecordsBank _bank;

		private readonly GhostRaceState _raceState;

		private RallyController _rally;

		private CancellationTokenSource _cts;

		private bool _playbackPrepared;

		private bool _playbackPreparing;

		private Transform _globalPoint;

		private string _raceUid;

		[Inject]
		public RallyGhostController(GhostSystem system, GhostStorage storage, GhostConfig config, RaceModeController raceModeController, RallyRecordsSync records, RallyRecordsBank bank, GhostRaceState raceState)
		{
			_system = system;
			_storage = storage;
			_config = config;
			_raceModeController = raceModeController;
			_records = records;
			_bank = bank;
			_raceState = raceState;
		}

		private string BackendKey()
		{
			RallyMapDataSO rallyMapDataSO = (_rally ? _rally.RallyMapDataSO : null);
			int locationIndex = RallyController.LocationIndex;
			if ((bool)rallyMapDataSO && rallyMapDataSO.RallyLocationDatas != null && locationIndex >= 0 && locationIndex < rallyMapDataSO.RallyLocationDatas.Count)
			{
				Result<string> result = RacingRecordsMapId.Build(rallyMapDataSO.RallyLocationDatas[locationIndex].AnalyticsId, RallyController.TrajectoryIndex);
				if (result.IsSuccess)
				{
					return result.Value;
				}
			}
			return $"{RallyController.LocationIndex}_{RallyController.TrajectoryIndex}";
		}

		public void Initialize()
		{
			_cts = new CancellationTokenSource();
			InitAsync(_cts.Token).Forget();
		}

		private async UniTaskVoid InitAsync(CancellationToken ct)
		{
			await UniTask.WaitUntil(() => _raceModeController.SpawnedObject, PlayerLoopTiming.Update, ct);
			_rally = _raceModeController.SpawnedObject.GetComponentInChildren<RallyController>();
			if ((bool)_rally)
			{
				_globalPoint = _rally.transform.parent ?? _rally.transform;
				_rally.GhostTransformProvider = () => _system.PlaybackGhostTransform;
				_rally.GhostReadyProvider = WaitForPlaybackReadyAsync;
				_rally.RunStaged += OnStaged;
				_rally.RunStarted += OnStarted;
				_rally.RunFinished += OnFinished;
				_rally.RunAborted += OnAborted;
				if (_records != null)
				{
					_records.ResultSubmitted += OnResultSubmitted;
				}
			}
		}

		private void OnResultSubmitted(string mapId)
		{
			_bank?.ForceRefresh(mapId);
		}

		private void OnStaged()
		{
			_system.DiscardActiveRecording();
			_system.PausePlayback();
			_playbackPrepared = false;
			_playbackPreparing = false;
			switch ((_raceState != null) ? _raceState.EffectiveKind : GhostSelectionKind.None)
			{
			case GhostSelectionKind.Opponent:
			{
				RacingRecord effectiveOpponent = _raceState.EffectiveOpponent;
				if (effectiveOpponent == null || string.IsNullOrEmpty(effectiveOpponent.UserId))
				{
					_system.StopPlayback();
					break;
				}
				_playbackPreparing = true;
				PrepareOpponentAsync(effectiveOpponent.UserId, _cts.Token).Forget();
				break;
			}
			case GhostSelectionKind.Local:
			{
				if (!_storage.TryGetBestMeta(RallyLeaderboardMapId.LocalKey(), out var _))
				{
					_system.StopPlayback();
					break;
				}
				_playbackPreparing = true;
				PrepareAsync(_cts.Token).Forget();
				break;
			}
			default:
				_system.StopPlayback();
				break;
			}
		}

		private async UniTaskVoid PrepareAsync(CancellationToken ct)
		{
			_ = 1;
			try
			{
				GhostRecording ghostRecording = await _storage.LoadBestRecordingAsync(RallyLeaderboardMapId.LocalKey(), ct);
				if (ghostRecording != null)
				{
					await _system.PreparePlaybackAsync(ghostRecording, _config, _globalPoint, ObserverTransform(), ct);
					_playbackPrepared = true;
				}
			}
			finally
			{
				_playbackPreparing = false;
			}
		}

		private async UniTaskVoid PrepareOpponentAsync(string userId, CancellationToken ct)
		{
			_ = 1;
			try
			{
				GhostRecording ghostRecording = await _records.DownloadOpponent(userId, BackendKey(), ct);
				if (ghostRecording == null)
				{
					_system.StopPlayback();
					return;
				}
				await _system.PreparePlaybackAsync(ghostRecording, _config, _globalPoint, ObserverTransform(), ct);
				_playbackPrepared = true;
			}
			finally
			{
				_playbackPreparing = false;
			}
		}

		public async UniTask WaitForPlaybackReadyAsync(CancellationToken ct)
		{
			if (_raceState != null && _raceState.EffectiveKind != GhostSelectionKind.None && !_playbackPrepared)
			{
				float num = ((_config != null) ? _config.PlaybackReadyTimeoutSeconds : 5f);
				UniTask uniTask = UniTask.WaitUntil(() => _playbackPrepared || !_playbackPreparing, PlayerLoopTiming.Update, ct);
				UniTask uniTask2 = UniTask.Delay(TimeSpan.FromSeconds(num), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				await UniTask.WhenAny(uniTask, uniTask2);
			}
		}

		private void OnStarted()
		{
			_system.StartRecording(V.Car.transform, _globalPoint, RallyLeaderboardMapId.LocalKey(), V.CarId, _config.RecordHz, _config.MaxRecordSeconds);
			if (_playbackPrepared)
			{
				_system.StartPlayback();
			}
			_raceUid = Guid.NewGuid().ToString();
			bool rain = RainWetnessDriver.CurrentIntensity > 0f;
			_records.SubmitStart(_raceUid, BackendKey(), V.CarId, CarClassId(), rain, _cts.Token);
		}

		private void OnFinished(float timeSec)
		{
			long num = (long)(timeSec * 1000f);
			GhostRecording ghostRecording = _system.BuildRecording(num);
			_system.StopPlayback();
			if (ghostRecording != null)
			{
				if (!string.IsNullOrEmpty(_raceUid) && (bool)_rally && _rally.IsDrivingIssuedRallyCar())
				{
					_records.SubmitFinish(_raceUid, BackendKey(), V.CarId, CarClassId(), num, ghostRecording, _cts.Token);
				}
				if (!_storage.TryGetBestMeta(RallyLeaderboardMapId.LocalKey(), out var meta) || num < meta.TimeMs)
				{
					_system.SaveRecordingAsync(ghostRecording, _config.MaxPersonalRecords, _cts.Token).Forget();
				}
			}
		}

		private int CarClassId()
		{
			if (!_rally)
			{
				return 0;
			}
			return _rally.CurrentCarClassId;
		}

		private void OnAborted()
		{
			_system.DiscardActiveRecording();
			_system.StopPlayback();
		}

		private static Transform ObserverTransform()
		{
			if (!V.Car)
			{
				return null;
			}
			return V.Car.transform;
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cts);
			if (_records != null)
			{
				_records.ResultSubmitted -= OnResultSubmitted;
			}
			_records?.ClearOpponentCache();
			if ((bool)_rally)
			{
				_rally.RunStaged -= OnStaged;
				_rally.RunStarted -= OnStarted;
				_rally.RunFinished -= OnFinished;
				_rally.RunAborted -= OnAborted;
			}
		}
	}
}
