using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Base;

namespace CPM.Checkpoints
{
	public sealed class CheckpointCourse : IDisposable
	{
		private readonly Dictionary<TriggerRelay, int> _indexByRelay = new Dictionary<TriggerRelay, int>();

		private readonly Func<GameObject, bool> _isTrackedRacer;

		public CheckpointTracker Tracker { get; }

		public int Missed => Tracker.Missed;

		public CheckpointCourse(IReadOnlyList<Collider> checkpoints, Func<GameObject, bool> isTrackedRacer)
		{
			_isTrackedRacer = isTrackedRacer ?? throw new ArgumentNullException("isTrackedRacer");
			int num = checkpoints?.Count ?? 0;
			Tracker = new CheckpointTracker(num);
			for (int i = 0; i < num; i++)
			{
				Collider collider = checkpoints[i];
				if ((bool)collider)
				{
					TriggerRelay triggerRelay = collider.gameObject.EnsureComponent<TriggerRelay>();
					_indexByRelay[triggerRelay] = i;
					triggerRelay.OnTriggerEntered += OnEntered;
				}
			}
		}

		public int Seal()
		{
			return Tracker.Seal();
		}

		private void OnEntered(Collider other, TriggerRelay source)
		{
			GameObject arg = (other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject);
			if (_isTrackedRacer(arg) && _indexByRelay.TryGetValue(source, out var value))
			{
				Tracker.Touch(value);
			}
		}

		public void Dispose()
		{
			foreach (KeyValuePair<TriggerRelay, int> item in _indexByRelay)
			{
				if ((bool)item.Key)
				{
					item.Key.OnTriggerEntered -= OnEntered;
				}
			}
			_indexByRelay.Clear();
		}
	}
}
