using System;
using UnityEngine;

namespace CPM.Obstacles
{
	public abstract class Obstacle : MonoBehaviour
	{
		public enum EventType
		{
			StartMove = 0,
			FinishMove = 1,
			StartReturn = 2,
			FinishReturn = 3,
			Action = 4
		}

		private const int NoPhase = -1;

		private int _lastPhaseIndex = -1;

		private EventType _lastEvent;

		public string CurrentPhase
		{
			get
			{
				if (_lastPhaseIndex != -1)
				{
					return $"#{_lastPhaseIndex} {_lastEvent}";
				}
				return "not reported yet";
			}
		}

		public virtual bool UsesSmoothedClock => true;

		public event Action<EventType> EventRaised;

		private void OnEnable()
		{
			_lastPhaseIndex = -1;
		}

		public abstract void EvaluateAt(double elapsedSeconds);

		protected void ReportPhase(int phaseIndex, EventType type)
		{
			if (phaseIndex == _lastPhaseIndex)
			{
				return;
			}
			bool num = _lastPhaseIndex == -1;
			_lastPhaseIndex = phaseIndex;
			_lastEvent = type;
			if (num)
			{
				return;
			}
			try
			{
				EventRaised?.Invoke(type);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, this);
			}
		}
	}
}
