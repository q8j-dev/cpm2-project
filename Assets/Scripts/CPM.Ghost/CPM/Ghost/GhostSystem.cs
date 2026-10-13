using System;
using System.Threading;
using CPM.Ghost.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.Ghost
{
	public sealed class GhostSystem : IDisposable
	{
		private const float AlphaInterval = 0.2f;

		private readonly IGhostCarFactory _factory;

		private readonly GhostStorage _storage;

		private readonly IGhostLogger _log;

		private GhostRecorder _recorder;

		private float _recordStartSec;

		private bool _recording;

		private GhostPlayback _playback;

		private float _alphaAccum;

		private CancellationTokenSource _loopCts;

		private bool _loopRunning;

		private CancellationTokenSource _prepareCts;

		public Transform PlaybackGhostTransform => _playback?.GhostTransform;

		public event Action PlaybackPrepared;

		public GhostSystem(IGhostCarFactory factory, GhostStorage storage, IGhostLogger log)
		{
			_factory = factory;
			_storage = storage;
			_log = log;
		}

		public void StartRecording(Transform source, Transform parent, string raceKey, int carId, int recordHz, float maxRecordSeconds)
		{
			_recorder = new GhostRecorder(source, parent, recordHz, maxRecordSeconds, raceKey, carId);
			_recordStartSec = Time.time;
			_recording = true;
			EnsureLoop();
		}

		public void StopRecording()
		{
			_recording = false;
		}

		public GhostRecording BuildRecording(long timeMs)
		{
			GhostRecording result = _recorder?.Build(timeMs);
			_recording = false;
			_recorder = null;
			return result;
		}

		public void DiscardActiveRecording()
		{
			_recording = false;
			_recorder = null;
		}

		public UniTask SaveRecordingAsync(GhostRecording rec, int maxReplays, CancellationToken ct)
		{
			return _storage.SaveAsync(rec, maxReplays, ct);
		}

		public async UniTask PreparePlaybackAsync(GhostRecording recording, GhostConfig config, Transform parent, Transform observer, CancellationToken ct)
		{
			if (recording == null)
			{
				return;
			}
			if (_playback != null && _playback.IsActive && _playback.CarId == recording.CarId)
			{
				_prepareCts?.Cancel();
				_prepareCts?.Dispose();
				_prepareCts = null;
				_playback.Rebind(recording);
				EnsureLoop();
				PlaybackPrepared?.Invoke();
				return;
			}
			_prepareCts?.Cancel();
			_prepareCts?.Dispose();
			_prepareCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
			GhostPlayback playback = new GhostPlayback();
			try
			{
				await playback.PrepareAsync(_factory, recording, config.MaterialPreset, config.LodWheelPool, config.GroundLayers, parent, observer, _prepareCts.Token);
			}
			catch (OperationCanceledException)
			{
				playback.Release(_factory);
				return;
			}
			if (playback.IsActive)
			{
				_playback?.Release(_factory);
				_playback = playback;
				EnsureLoop();
				PlaybackPrepared?.Invoke();
			}
		}

		public void StartPlayback()
		{
			if (_playback != null && _playback.IsActive)
			{
				_playback.StartFrom(Time.time);
				EnsureLoop();
			}
		}

		public void StopPlayback()
		{
			_playback?.Release(_factory);
			_playback = null;
		}

		public void PausePlayback()
		{
			_playback?.Pause();
		}

		private void EnsureLoop()
		{
			if (!_loopRunning)
			{
				_loopCts?.Dispose();
				_loopCts = new CancellationTokenSource();
				_loopRunning = true;
				RunLoop(_loopCts.Token).Forget();
			}
		}

		private async UniTaskVoid RunLoop(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				bool flag = _playback != null && _playback.IsActive;
				if (!_recording && !flag)
				{
					_loopRunning = false;
					return;
				}
				if (_recording)
				{
					_recorder?.Tick(Time.time - _recordStartSec);
				}
				if (flag)
				{
					_playback.Tick(Time.time);
					_alphaAccum += Time.deltaTime;
					if (_alphaAccum >= 0.2f)
					{
						_alphaAccum = 0f;
						_playback.UpdateAlpha();
					}
				}
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
			_loopRunning = false;
		}

		public void Dispose()
		{
			_prepareCts?.Cancel();
			_prepareCts?.Dispose();
			_loopCts?.Cancel();
			_loopCts?.Dispose();
			_loopRunning = false;
			_recording = false;
			_recorder = null;
			_playback?.Release(_factory);
			_playback = null;
		}
	}
}
