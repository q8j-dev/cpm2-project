using System;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	public class ZoneParticleMover : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem ParticleSystem = default;

		private const float MoveTimer = 0.1f;

		private Transform _cameraTransform;

		private TimedActivityZoneController _timedActivityZoneController;

		private IDisposable _moveDisposable;

		[Inject]
		public void Construct(OptionalDependency<TimedActivityZoneController> timedActivityZoneController)
		{
			_timedActivityZoneController = timedActivityZoneController.Value;
		}

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			ParticleSystem.Stop();
			if (_timedActivityZoneController != null)
			{
				_cameraTransform = V.SceneControl.mainCamera.transform;
				_timedActivityZoneController.RegisterParticle(this);
				base.gameObject.SetActive(value: false);
			}
		}

		public void ChangeState(bool state)
		{
			_moveDisposable?.Dispose();
			ParticleSystem.Stop();
			if (state)
			{
				ParticleSystem.Play();
				_moveDisposable = Observable.Interval(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
				{
					Move();
				}).AddTo(this);
			}
			ParticleSystem.gameObject.SetActive(state);
		}

		public void PlayParticleSystem()
		{
			ParticleSystem.Play();
		}

		private void Move()
		{
			base.transform.SetPositionAndRotation(_cameraTransform.position, _cameraTransform.rotation);
		}

		private void OnDestroy()
		{
			_moveDisposable?.Dispose();
		}
	}
}
