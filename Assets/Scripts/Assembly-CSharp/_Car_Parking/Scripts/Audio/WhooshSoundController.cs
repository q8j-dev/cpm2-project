using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public class WhooshSoundController : IDisposable
	{
		public const float MaxDistanceToCheck = 7f;

		public const float MinSpeed = 60f;

		public const float SpeedFast = 100f;

		private readonly int _transparentFXLayer = LayerMask.GetMask("TransparentFX");

		private readonly Collider[] _colliders = new Collider[10];

		private static readonly uint _playWindPassbySoundFastEvent = AudioWrapper.GetId("Play_wind_passby_fast");

		private static readonly uint _playWindPassbySoundSlowEvent = AudioWrapper.GetId("Play_wind_passby_slow");

		private HashSet<int> _playingObjectsHashes = new HashSet<int>();

		private IDisposable _checkerPlayingObjectsNull;

		public void Update(float speedKmh)
		{
			if (Physics.OverlapSphereNonAlloc(V.CoreGameManager.transform.position, 7f, _colliders, _transparentFXLayer) == 0)
			{
				return;
			}
			Collider[] colliders = _colliders;
			foreach (Collider collider in colliders)
			{
				if (!collider)
				{
					continue;
				}
				int hash = collider.gameObject.GetHashCode();
				if (!_playingObjectsHashes.Contains(hash))
				{
					AudioWrapper.PostEvent((speedKmh >= 100f) ? _playWindPassbySoundFastEvent : _playWindPassbySoundSlowEvent, collider.gameObject);
					_playingObjectsHashes.Add(hash);
					_checkerPlayingObjectsNull = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
					{
						_playingObjectsHashes.Remove(hash);
					});
				}
			}
		}

		public void Dispose()
		{
			_checkerPlayingObjectsNull?.Dispose();
		}
	}
}
