using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Free_drive
{
	public class SnowParticlesMover : MonoBehaviour
	{
		private const string HomeTriggerName = "StopSnowTrigger";

		public static SnowParticlesMover Instance;

		private const float MoveTimer = 0.1f;

		private Transform _cameraTransform;

		private Transform _child;

		private int _triggerCount;

		private ParticleSystem _particleSystem;

		private bool _state;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			_child = base.transform.GetChild(0);
		}

		private void Start()
		{
			_state = true;
			_particleSystem = GetComponentInChildren<ParticleSystem>();
			Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				Init();
			}).AddTo(this);
		}

		private void Init()
		{
			_cameraTransform = V.CoreGameManager.MainCamera.transform;
			Observable.Interval(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
			{
				Move();
			}).AddTo(this);
			if ((bool)_particleSystem && _state)
			{
				_particleSystem.Play();
			}
		}

		public void SetState(bool state)
		{
			_state = state;
			if ((bool)_particleSystem)
			{
				if (_state)
				{
					_particleSystem.Play();
				}
				else
				{
					_particleSystem.Stop();
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!(other.transform.name != "StopSnowTrigger"))
			{
				_child.gameObject.SetActive(value: false);
				_triggerCount++;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.transform.name != "StopSnowTrigger")
			{
				return;
			}
			_triggerCount--;
			if (_triggerCount <= 0)
			{
				_child.gameObject.SetActive(value: true);
				ParticleSystem componentInChildren = GetComponentInChildren<ParticleSystem>();
				if ((bool)componentInChildren)
				{
					componentInChildren.Play();
				}
			}
		}

		private void Move()
		{
			base.transform.position = _cameraTransform.position;
			base.transform.rotation = _cameraTransform.rotation;
		}

		private void OnDestroy()
		{
			Instance = null;
		}
	}
}
