using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class AnimationBlinkerGO : MonoBehaviour
	{
		public GameObject Blinker;

		public float BlinkDelay = 0.2f;

		private bool _isBlinking;

		private IDisposable _blinkTick;

		private bool _blinkerState;

		public bool IsBlinking
		{
			get
			{
				return _isBlinking;
			}
			set
			{
				_isBlinking = value;
				Blinker.SetActive(!_isBlinking);
				BlinkUpdated();
			}
		}

		public void ChangeBlinker(bool isShouldBlink)
		{
			_isBlinking = isShouldBlink;
			Blinker.SetActive(_isBlinking);
			BlinkUpdated();
		}

		private void BlinkUpdated()
		{
			if (_isBlinking)
			{
				_blinkTick?.Dispose();
				_blinkTick = Observable.Interval(TimeSpan.FromSeconds(BlinkDelay)).Subscribe((long _) =>
				{
					Blink();
				}).AddTo(this);
			}
		}

		private void Blink()
		{
			_blinkerState = !_blinkerState;
			Blinker.SetActive(_blinkerState);
		}

		private void OnDisable()
		{
			_blinkTick?.Dispose();
			_blinkTick = null;
		}
	}
}
