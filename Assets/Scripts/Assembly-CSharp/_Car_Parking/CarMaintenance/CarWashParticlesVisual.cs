using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.CarMaintenance
{
	public class CarWashParticlesVisual : CarWashVisualBase
	{
		[SerializeField]
		private List<ParticleSystem> _particles = new List<ParticleSystem>();

		[SerializeField]
		private Transform _carPlacePoint;

		[SerializeField]
		private string _playEvent;

		[SerializeField]
		private string _stopEvent;

		public override async UniTask PlayAsync(GameObject car, float duration, CancellationToken ct)
		{
			if (car == null)
			{
				return;
			}
			Transform transform = (_carPlacePoint ? _carPlacePoint : base.transform);
			Transform transform2 = car.transform;
			if (car.TryGetComponent<VehicleData>(out var component))
			{
				component.StopCarCompletely(duration);
			}
			transform2.position = new Vector3(transform.position.x, transform2.position.y, transform.position.z);
			SetParticles(play: true);
			if (!string.IsNullOrEmpty(_playEvent))
			{
				AudioWrapper.PostEvent(_playEvent, base.gameObject);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			if (!ct.IsCancellationRequested)
			{
				SetParticles(play: false);
				if (!string.IsNullOrEmpty(_stopEvent))
				{
					AudioWrapper.PostEvent(_stopEvent, base.gameObject);
				}
			}
		}

		private void SetParticles(bool play)
		{
			foreach (ParticleSystem particle in _particles)
			{
				if ((bool)particle)
				{
					if (play)
					{
						particle.Play();
					}
					else
					{
						particle.Stop();
					}
				}
			}
		}

		private void OnDisable()
		{
			SetParticles(play: false);
		}
	}
}
