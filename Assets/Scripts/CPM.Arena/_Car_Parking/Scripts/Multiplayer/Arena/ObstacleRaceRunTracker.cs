using System;
using System.Collections.Generic;
using CPM.Checkpoints;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceRunTracker
	{
		private readonly List<ObstacleRaceCheckpointRecord> _records = new List<ObstacleRaceCheckpointRecord>();

		private IArenaContext _ctx;

		private CheckpointTracker _checkpoints;

		private string[] _trackPlan;

		private double _matchStart;

		private bool _started;

		private bool _terminated;

		private int _nextRecord;

		private int _restartsSinceCheckpoint;

		private int _restartTotal;

		private double? _finishElapsed;

		public IReadOnlyList<ObstacleRaceCheckpointRecord> Journal => _records;

		public int RestartCount => _restartTotal;

		private double? Elapsed
		{
			get
			{
				if (_ctx == null || _matchStart <= 0.0)
				{
					return null;
				}
				double num = _ctx.ServerTime - _matchStart;
				if (!(num >= 0.0))
				{
					return null;
				}
				return num;
			}
		}

		public event Action<ObstacleRaceRunSnapshot> OnSnapshot;

		public void Go(IArenaContext ctx, ObstacleRaceTrackInfo track, CheckpointTracker checkpoints)
		{
			if (!_started)
			{
				Reset();
				_ctx = ctx;
				_checkpoints = checkpoints;
				_matchStart = ctx?.MatchStartTime ?? 0.0;
				_trackPlan = ((track.Plan != null && track.Plan.Length != 0) ? track.Plan : null);
				_started = true;
				if (_checkpoints != null)
				{
					_checkpoints.Advanced += OnCheckpointAdvanced;
				}
				OnSnapshot?.Invoke(new ObstacleRaceRunSnapshot
				{
					Status = ObstacleRaceRunStatus.Go,
					RaceId = ctx?.RaceId,
					PlayersAtStart = ((track.ParticipantCount > 0) ? new int?(track.ParticipantCount) : ((int?)null)),
					IsMaster = ctx?.IsMasterClient,
					TrackSeed = track.Seed,
					TrackPlan = _trackPlan
				});
			}
		}

		public void Reset()
		{
			if (_checkpoints != null)
			{
				_checkpoints.Advanced -= OnCheckpointAdvanced;
			}
			_ctx = null;
			_checkpoints = null;
			_trackPlan = null;
			_matchStart = 0.0;
			_started = false;
			_terminated = false;
			_records.Clear();
			_nextRecord = 0;
			_restartsSinceCheckpoint = 0;
			_restartTotal = 0;
			_finishElapsed = null;
		}

		public void CaptureFinish(double elapsed)
		{
			if (_started && !_terminated && !_finishElapsed.HasValue)
			{
				_finishElapsed = ((elapsed >= 0.0) ? elapsed : 0.0);
			}
		}

		public void TryFinish(Func<int> resolvePlace)
		{
			if (_started && !_terminated && _finishElapsed.HasValue)
			{
				int num = resolvePlace?.Invoke() ?? 0;
				if (num > 0)
				{
					EmitFinished(num);
				}
			}
		}

		public void Leave(int? progressPercent)
		{
			if (_started && !_terminated)
			{
				if (_finishElapsed.HasValue)
				{
					EmitFinished(null);
					return;
				}
				AppendPendingCheckpoint();
				ObstacleRaceRunSnapshot obstacleRaceRunSnapshot = BuildTerminal(ObstacleRaceRunStatus.Left);
				obstacleRaceRunSnapshot.DurationMs = ToMilliseconds(Elapsed);
				obstacleRaceRunSnapshot.ProgressCurrent = progressPercent;
				Emit(obstacleRaceRunSnapshot);
			}
		}

		public void Respawn()
		{
			if (_started && !_terminated)
			{
				_restartTotal++;
				_restartsSinceCheckpoint++;
			}
		}

		private void OnCheckpointAdvanced()
		{
			if (_started && !_terminated && _checkpoints != null)
			{
				int last = _checkpoints.Last;
				for (int i = _nextRecord; i < last; i++)
				{
					_records.Add(new ObstacleRaceCheckpointRecord(i, null, 0, missed: true));
				}
				_records.Add(new ObstacleRaceCheckpointRecord(last, Elapsed, _restartsSinceCheckpoint, missed: false));
				_nextRecord = last + 1;
				_restartsSinceCheckpoint = 0;
			}
		}

		private void EmitFinished(int? place)
		{
			ObstacleRaceRunSnapshot obstacleRaceRunSnapshot = BuildTerminal(ObstacleRaceRunStatus.Finished);
			obstacleRaceRunSnapshot.IsMaster = _ctx?.IsMasterClient;
			obstacleRaceRunSnapshot.Place = place;
			obstacleRaceRunSnapshot.DurationMs = ToMilliseconds(_finishElapsed);
			Emit(obstacleRaceRunSnapshot);
		}

		private ObstacleRaceRunSnapshot BuildTerminal(ObstacleRaceRunStatus status)
		{
			int? checkpointEarned = _checkpoints?.Passed;
			_checkpoints?.Seal();
			return new ObstacleRaceRunSnapshot
			{
				Status = status,
				RaceId = _ctx?.RaceId,
				TrackPlan = _trackPlan,
				CheckpointCount = _checkpoints?.Total,
				CheckpointEarned = checkpointEarned,
				CheckpointMissed = _checkpoints?.Missed,
				RestartCount = _restartTotal,
				Checkpoints = ((_records.Count > 0) ? new List<ObstacleRaceCheckpointRecord>(_records) : null)
			};
		}

		private void AppendPendingCheckpoint()
		{
			if (_checkpoints != null && _nextRecord < _checkpoints.Total)
			{
				_records.Add(new ObstacleRaceCheckpointRecord(_nextRecord, null, _restartsSinceCheckpoint, missed: false));
			}
		}

		private void Emit(ObstacleRaceRunSnapshot snapshot)
		{
			_terminated = true;
			if (_checkpoints != null)
			{
				_checkpoints.Advanced -= OnCheckpointAdvanced;
			}
			OnSnapshot?.Invoke(snapshot);
		}

		private static int? ToMilliseconds(double? seconds)
		{
			if (!seconds.HasValue || !(seconds.Value >= 0.0))
			{
				return null;
			}
			return (int)Math.Round(seconds.Value * 1000.0);
		}
	}
}
