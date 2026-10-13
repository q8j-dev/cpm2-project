using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl
{
	public class TimerObject : IDisposable
	{
		private CancellationTokenSource _cts = new CancellationTokenSource();

		private AdjustableDictionary _adjustableDictionary;

		private CancellationTokenRegistration _timer;

		public TimerObject(byte key, float time, AdjustableDictionary adjustableDictionary)
		{
			TimerObject timerObject = this;
			_adjustableDictionary = adjustableDictionary;
			_timer = Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe((long _) =>
			{
				timerObject._adjustableDictionary.Remove(key);
			}).AddTo(_cts.Token);
		}

		~TimerObject()
		{
			Dispose();
		}

		public void Dispose()
		{
			_adjustableDictionary = null;
			if (_cts != null)
			{
				_cts.Cancel();
				_cts.Dispose();
				_cts = null;
			}
		}
	}
}
