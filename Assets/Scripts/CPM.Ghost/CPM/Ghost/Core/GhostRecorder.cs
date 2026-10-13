using System;
using System.Collections.Generic;
using UnityEngine;

namespace CPM.Ghost.Core
{
	public sealed class GhostRecorder
	{
		private readonly Transform _source;

		private readonly Transform _parent;

		private readonly int _recordHz;

		private readonly float _maxRecordSec;

		private readonly string _raceKey;

		private readonly int _carId;

		private readonly float _sampleInterval;

		private readonly List<GhostFrame> _frames = new List<GhostFrame>();

		private float _nextSampleTime;

		public bool ExceededLimit { get; private set; }

		public int FrameCount => _frames.Count;

		public GhostRecorder(Transform source, Transform parent, int recordHz, float maxRecordSeconds, string raceKey, int carId)
		{
			_source = source;
			_parent = parent;
			_recordHz = Mathf.Clamp(recordHz, 1, 60);
			_maxRecordSec = maxRecordSeconds;
			_raceKey = raceKey;
			_carId = carId;
			_sampleInterval = 1f / (float)_recordHz;
			_nextSampleTime = 0f;
		}

		public void Tick(float currentTimeSec)
		{
			if (!(_source == null) && !(_parent == null))
			{
				if (currentTimeSec > _maxRecordSec)
				{
					ExceededLimit = true;
					_frames.Clear();
				}
				else if (!(currentTimeSec < _nextSampleTime))
				{
					Vector3 pos = _parent.InverseTransformPoint(_source.position);
					Quaternion rot = Quaternion.Inverse(_parent.rotation) * _source.rotation;
					_frames.Add(GhostFrame.Create(currentTimeSec, pos, rot));
					_nextSampleTime += _sampleInterval;
				}
			}
		}

		public GhostRecording Build(long timeMs)
		{
			if (ExceededLimit || _frames.Count == 0)
			{
				return null;
			}
			return new GhostRecording
			{
				ReplayId = GhostIds.New(_raceKey, _carId, timeMs),
				RaceKey = _raceKey,
				CarId = _carId,
				TimeMs = timeMs,
				RecordedAtUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
				RecordHz = _recordHz,
				Frames = new List<GhostFrame>(_frames)
			};
		}
	}
}
