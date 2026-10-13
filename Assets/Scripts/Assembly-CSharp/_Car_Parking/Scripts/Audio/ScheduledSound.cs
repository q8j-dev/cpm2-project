using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;

namespace _Car_Parking.Scripts.Audio
{
	public class ScheduledSound : MonoBehaviour
	{
		[Serializable]
		public class DayTime
		{
			[Range(0f, 23f)]
			public int Hours;

			[Range(0f, 59f)]
			public int Minutes;
		}

		private const int DayNightCycleMode = 3;

		private const int CustomTimeMode = 5;

		private const float TimeToCheckTime = 0.5f;

		[SerializeField]
		private string PlayEvent;

		[SerializeField]
		private float DurationSound;

		[SerializeField]
		private List<DayTime> Schedule = new List<DayTime>();

		private bool _isPlaying;

		private IDisposable _checkTimeDisposable;

		private TimeAndWeatherController _timeController;

		private void Awake()
		{
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.directionalLight && V.CoreGameManager.directionalLight.TryGetComponent<TimeAndWeatherController>(out _timeController))
			{
				V.OnSkyboxChanged = (Action)Delegate.Combine(V.OnSkyboxChanged, new Action(TryPlaySound));
				TryPlaySound();
			}
		}

		private void TryPlaySound()
		{
			int skyboxSelectedRuntime = TimeProvider.skyboxSelectedRuntime;
			_checkTimeDisposable?.Dispose();
			_checkTimeDisposable = null;
			switch (skyboxSelectedRuntime)
			{
			case 3:
				_checkTimeDisposable = (from t in Observable.Interval(TimeSpan.FromSeconds(0.5), Scheduler.MainThread)
					where CheckTime(_timeController.CurrentTime.Hour, _timeController.CurrentTime.Minute)
					select t).Subscribe((long _) =>
				{
					PlaySound();
				}).AddTo(this);
				break;
			case 5:
				if (CheckTime(_timeController.CurrentTime.Hour, _timeController.CurrentTime.Minute))
				{
					PlaySound();
				}
				break;
			}
		}

		private void PlaySound()
		{
			if (_isPlaying)
			{
				return;
			}
			_checkTimeDisposable?.Dispose();
			_checkTimeDisposable = null;
			AudioWrapper.PostEvent(PlayEvent, base.gameObject);
			_isPlaying = true;
			Observable.Timer(TimeSpan.FromSeconds(DurationSound), Scheduler.MainThread).Subscribe((long _) =>
			{
				_isPlaying = false;
				if (TimeProvider.skyboxSelectedRuntime == 3)
				{
					TryPlaySound();
				}
			}).AddTo(this);
		}

		private bool CheckTime(int currentHour, int currentMinute)
		{
			for (int i = 0; i < Schedule.Count; i++)
			{
				if (Schedule[i].Hours == currentHour && Schedule[i].Minutes == currentMinute)
				{
					return true;
				}
			}
			return false;
		}

		private void OnDestroy()
		{
			_checkTimeDisposable?.Dispose();
			_checkTimeDisposable = null;
			V.OnSkyboxChanged = (Action)Delegate.Remove(V.OnSkyboxChanged, new Action(TryPlaySound));
			if (_isPlaying)
			{
				AudioWrapper.StopAll(base.gameObject);
			}
		}
	}
}
