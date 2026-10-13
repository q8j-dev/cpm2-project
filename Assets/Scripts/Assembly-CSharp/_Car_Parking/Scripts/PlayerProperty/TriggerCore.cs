using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.PlayerProperty
{
	public class TriggerCore : MonoBehaviour
	{
		[SerializeField]
		private float CheckInterval = 0.5f;

		private readonly List<IIntervalUpdatable> _intervalUpdatables = new List<IIntervalUpdatable>();

		private void Awake()
		{
			_intervalUpdatables.AddRange(GetComponentsInChildren<IIntervalUpdatable>(includeInactive: true));
		}

		private void Start()
		{
			Observable.Interval(TimeSpan.FromSeconds(CheckInterval)).Subscribe((long _) =>
			{
				IntervalUpdate();
			}).AddTo(this);
		}

		public void Register(IIntervalUpdatable updatable)
		{
			if (!_intervalUpdatables.Contains(updatable))
			{
				_intervalUpdatables.Add(updatable);
			}
		}

		public void Unregister(IIntervalUpdatable updatable)
		{
			_intervalUpdatables.Remove(updatable);
		}

		private void IntervalUpdate()
		{
			foreach (IIntervalUpdatable intervalUpdatable in _intervalUpdatables)
			{
				intervalUpdatable.IntervalUpdate();
			}
		}
	}
}
