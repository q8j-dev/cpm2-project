using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public class IgnoreWhooshSoundController : MonoBehaviour
	{
		private const string ObjectsTagToPlayWhoosh = "MainCamera";

		private const float TimeToResetIgnorePlayWhoosh = 1f;

		private IDisposable _ignoreWhooshDisposable;

		public bool IgnorePlayWhoosh { get; private set; }

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("MainCamera"))
			{
				IgnorePlayWhoosh = true;
				_ignoreWhooshDisposable?.Dispose();
				_ignoreWhooshDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					IgnorePlayWhoosh = false;
				}).AddTo(this);
			}
		}
	}
}
