using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public class SandFallSound : MonoBehaviour
	{
		private const float MinDistance = 25f;

		[Header("Wwise Events (Looped)")]
		[SerializeField]
		private string PlaySandLoopEvent;

		[SerializeField]
		private string StopSandLoopEvent;

		private ParticleSystem _particleSystem;

		private ParticleSystem.Particle[] _particles;

		private IDisposable _checkerParticlesDisposable;

		private bool _isPlayingLoop;

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
			if ((bool)_particleSystem)
			{
				_particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
			}
		}

		public void StartCheckParticles()
		{
			if ((bool)_particleSystem)
			{
				_checkerParticlesDisposable?.Dispose();
				_checkerParticlesDisposable = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.20000000298023224)).Subscribe((long _) =>
				{
					CheckParticles();
				}).AddTo(this);
			}
		}

		public void StopCheckParticles()
		{
			_checkerParticlesDisposable?.Dispose();
		}

		private void CheckParticles()
		{
			if (!_particleSystem)
			{
				return;
			}
			if ((V.CoreGameManager.transform.position - base.transform.position).sqrMagnitude > 625f)
			{
				StopLoopIfPlaying();
				return;
			}
			int particles = _particleSystem.GetParticles(_particles);
			if (particles > 0 && !_isPlayingLoop)
			{
				AudioWrapper.PostEvent(PlaySandLoopEvent, base.gameObject);
				_isPlayingLoop = true;
			}
			else if (particles == 0 && _isPlayingLoop)
			{
				AudioWrapper.PostEvent(StopSandLoopEvent, base.gameObject);
				_isPlayingLoop = false;
			}
		}

		private void StopLoopIfPlaying()
		{
			if (_isPlayingLoop)
			{
				AudioWrapper.PostEvent(StopSandLoopEvent, base.gameObject);
				_isPlayingLoop = false;
			}
		}

		private void OnDestroy()
		{
			StopLoopIfPlaying();
			_checkerParticlesDisposable?.Dispose();
		}
	}
}
