using System;
using UniRx;

namespace _Car_Parking.Scripts.EventSystem
{
	public class EventZoneTimerComponent : IDisposable
	{
		public Action<int> OnTick;

		public Action OnTimerFinished;

		public Action OnHalfTimeBeat;

		public bool IsActive;

		private bool _isPartTimeBeatedCalled;

		private bool _isPartTimeBeat;

		private float _targetTime;

		private float _partTargetTime;

		private int _currentTime;

		private TimedActivityZoneController _timedActivityZoneController;

		private IDisposable _startTimer;

		public float TargetTime => _targetTime;

		public float CurrentTime => _currentTime;

		public bool IsPartTimeBeat => _isPartTimeBeat;

		public EventZoneTimerComponent(TimedActivityZoneController timedActivityZoneController)
		{
			_timedActivityZoneController = timedActivityZoneController;
		}

		public void Init(float targetTime, int partTimePercent = 50)
		{
			_targetTime = targetTime;
			_partTargetTime = targetTime * (float)partTimePercent / 100f;
		}

		public void Start()
		{
			_startTimer?.Dispose();
			_currentTime = (int)_targetTime;
			StartTimer();
		}

		public void Resume()
		{
			StartTimer();
		}

		public void Pause()
		{
			StopTimer();
		}

		private void StartTimer()
		{
			_startTimer = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				Tick();
			});
		}

		public void Reset()
		{
			_isPartTimeBeatedCalled = false;
			_isPartTimeBeat = false;
			StopTimer();
			_currentTime = (int)_targetTime;
		}

		private void Tick()
		{
			IsActive = true;
			_currentTime--;
			_isPartTimeBeat = _partTargetTime > (float)_currentTime;
			if (_isPartTimeBeat && !_isPartTimeBeatedCalled)
			{
				OnHalfTimeBeat?.Invoke();
				_isPartTimeBeatedCalled = true;
			}
			CheckTimerData();
			OnTick?.Invoke(_currentTime);
		}

		private void CheckTimerData()
		{
			if (CurrentTime <= 0f)
			{
				OnTimerFinished?.Invoke();
				StopTimer();
			}
		}

		private void StopTimer()
		{
			IsActive = false;
			_startTimer?.Dispose();
		}

		public void Dispose()
		{
			StopTimer();
		}
	}
}
