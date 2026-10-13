using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Racing_Scripts;

namespace RacingScripts.RaceControllers
{
	public class RallyProgressUIDriver : IDisposable
	{
		private readonly RacingProgressUIController _ui;

		private readonly RallyCar _rallyCar;

		private readonly RallyTriggersControl _triggersControl;

		private readonly Func<Transform> _ghostTransformProvider;

		private float[] _remainingDist;

		private int _ghostNearestIdx;

		public RallyProgressUIDriver(RacingProgressUIController ui, RallyCar rallyCar, RallyTriggersControl triggersControl, Func<Transform> ghostTransformProvider = null)
		{
			_ui = ui;
			_rallyCar = rallyCar;
			_triggersControl = triggersControl;
			_ghostTransformProvider = ghostTransformProvider;
			BuildRemainingDistances();
			RallyCar rallyCar2 = _rallyCar;
			rallyCar2.OnTimerUpdate = (Action<float, float>)Delegate.Combine(rallyCar2.OnTimerUpdate, new Action<float, float>(OnTimerUpdate));
			RallyTriggersControl triggersControl2 = _triggersControl;
			triggersControl2.CheckpointCrossed = (Action<int>)Delegate.Combine(triggersControl2.CheckpointCrossed, new Action<int>(OnCheckpointCrossed));
			RefreshCheckpoints();
		}

		private void BuildRemainingDistances()
		{
			List<Vector3> roadCenterline = _triggersControl.RoadCenterline;
			if (roadCenterline == null || roadCenterline.Count == 0)
			{
				_remainingDist = Array.Empty<float>();
				return;
			}
			_remainingDist = new float[roadCenterline.Count];
			_remainingDist[roadCenterline.Count - 1] = 0f;
			for (int num = roadCenterline.Count - 2; num >= 0; num--)
			{
				_remainingDist[num] = _remainingDist[num + 1] + Vector3.Distance(roadCenterline[num], roadCenterline[num + 1]);
			}
		}

		private void OnTimerUpdate(float raceTime, float penalty)
		{
			List<Vector3> roadCenterline = _triggersControl.RoadCenterline;
			if (roadCenterline != null && roadCenterline.Count > 1)
			{
				int num = Mathf.Clamp(_rallyCar.CenterlineNearestIdx, 0, roadCenterline.Count - 1);
				float value = (float)num / (float)(roadCenterline.Count - 1);
				_ui.SetProgress(Mathf.Clamp01(value));
				if (_remainingDist != null && _remainingDist.Length > num)
				{
					_ui.SetDistance(_remainingDist[num] / 1000f);
				}
				UpdateGhostProgress(roadCenterline);
			}
		}

		private void UpdateGhostProgress(List<Vector3> centerline)
		{
			Transform transform = _ghostTransformProvider?.Invoke();
			if (transform == null)
			{
				_ui.SetGhostVisible(visible: false);
				return;
			}
			Vector3 position = transform.position;
			position.y = 0f;
			int num = Mathf.Max(0, _ghostNearestIdx - 30);
			int num2 = Mathf.Min(centerline.Count - 1, _ghostNearestIdx + 40);
			float num3 = float.MaxValue;
			for (int i = num; i <= num2; i++)
			{
				Vector3 vector = centerline[i];
				vector.y = 0f;
				float sqrMagnitude = (position - vector).sqrMagnitude;
				if (sqrMagnitude < num3)
				{
					num3 = sqrMagnitude;
					_ghostNearestIdx = i;
				}
			}
			_ui.SetGhostVisible(visible: true);
			_ui.SetGhostProgress(Mathf.Clamp01((float)_ghostNearestIdx / (float)(centerline.Count - 1)));
		}

		private void OnCheckpointCrossed(int idx)
		{
			RefreshCheckpoints();
		}

		private void RefreshCheckpoints()
		{
			int realCheckpointsPassed = _triggersControl.RealCheckpointsPassed;
			int realCheckpointsTotal = _triggersControl.RealCheckpointsTotal;
			_ui.SetProgressText($"{realCheckpointsPassed}/{realCheckpointsTotal}");
		}

		public void Dispose()
		{
			if (_rallyCar != null)
			{
				RallyCar rallyCar = _rallyCar;
				rallyCar.OnTimerUpdate = (Action<float, float>)Delegate.Remove(rallyCar.OnTimerUpdate, new Action<float, float>(OnTimerUpdate));
			}
			if (_triggersControl != null)
			{
				RallyTriggersControl triggersControl = _triggersControl;
				triggersControl.CheckpointCrossed = (Action<int>)Delegate.Remove(triggersControl.CheckpointCrossed, new Action<int>(OnCheckpointCrossed));
			}
		}
	}
}
