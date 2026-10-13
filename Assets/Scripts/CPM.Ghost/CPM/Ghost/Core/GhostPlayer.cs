using System.Collections.Generic;
using UnityEngine;

namespace CPM.Ghost.Core
{
	public sealed class GhostPlayer
	{
		private readonly Transform _target;

		private readonly GhostRecording _recording;

		private int _cursor;

		public bool IsFinished { get; private set; }

		public GhostPlayer(Transform target, GhostRecording recording)
		{
			_target = target;
			_recording = recording;
		}

		public void ApplyFirstFrame()
		{
			if (!(_target == null) && _recording != null && _recording.Frames.Count != 0)
			{
				GhostFrame ghostFrame = _recording.Frames[0];
				_target.localPosition = ghostFrame.Position;
				_target.localRotation = ghostFrame.Rotation;
			}
		}

		public void Tick(float currentTimeSec)
		{
			if (_target == null || _recording == null)
			{
				return;
			}
			List<GhostFrame> frames = _recording.Frames;
			if (frames.Count == 0)
			{
				IsFinished = true;
				return;
			}
			while (_cursor + 1 < frames.Count && frames[_cursor + 1].Time <= currentTimeSec)
			{
				_cursor++;
			}
			if (_cursor + 1 >= frames.Count)
			{
				GhostFrame ghostFrame = frames[frames.Count - 1];
				_target.localPosition = ghostFrame.Position;
				_target.localRotation = ghostFrame.Rotation;
				IsFinished = true;
			}
			else
			{
				GhostFrame ghostFrame2 = frames[_cursor];
				GhostFrame ghostFrame3 = frames[_cursor + 1];
				float num = ghostFrame3.Time - ghostFrame2.Time;
				float t = ((num > 0.0001f) ? Mathf.Clamp01((currentTimeSec - ghostFrame2.Time) / num) : 0f);
				_target.localPosition = Vector3.LerpUnclamped(ghostFrame2.Position, ghostFrame3.Position, t);
				_target.localRotation = Quaternion.SlerpUnclamped(ghostFrame2.Rotation, ghostFrame3.Rotation, t);
			}
		}
	}
}
