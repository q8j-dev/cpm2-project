using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.CarMaintenance
{
	public class CarWashCylinders : CarWashVisualBase
	{
		private const string CarWash = "Play_SFX_Washing_Car";

		private const string CarWashStop = "Stop_SFX_Washing_Car";

		[SerializeField]
		private Transform leftCylinders;

		[SerializeField]
		private Transform rightCylinders;

		[SerializeField]
		private ParticleSystem waterParticles;

		private float _rotateAngle = 90f;

		private Vector3 _originPos;

		private Quaternion _originRot;

		private Camera _camera;

		private void Start()
		{
			Transform transform = base.transform;
			_originPos = transform.localPosition;
			_originRot = transform.localRotation;
		}

		public override async UniTask PlayAsync(GameObject car, float duration, CancellationToken ct)
		{
			if (!(car == null))
			{
				Transform obj = base.transform;
				obj.rotation = car.transform.rotation;
				Vector3 position = obj.position;
				Vector3 position2 = car.transform.position;
				obj.position = new Vector3(position2.x, position.y, position2.z);
				await MoveRotateCoroutine(duration, ct);
			}
		}

		private void MoveRotateOnce(bool isRotate = true)
		{
			if (isRotate)
			{
				if (_rotateAngle > 0f)
				{
					_rotateAngle -= 5f;
					if (leftCylinders != null)
					{
						leftCylinders.localRotation = Quaternion.Euler(0f, 0f, 0f - _rotateAngle);
					}
					if (rightCylinders != null)
					{
						rightCylinders.localRotation = Quaternion.Euler(0f, 0f, _rotateAngle);
					}
				}
				for (int i = 0; i < 3; i++)
				{
					if (leftCylinders != null)
					{
						leftCylinders.GetChild(i).Rotate(Vector3.up, 8f);
					}
					if (rightCylinders != null)
					{
						rightCylinders?.GetChild(i).Rotate(Vector3.up, 8f);
					}
				}
			}
			else if (_rotateAngle < 90f)
			{
				_rotateAngle += 5f;
				if (leftCylinders != null)
				{
					leftCylinders.localRotation = Quaternion.Euler(0f, 0f, 0f - _rotateAngle);
				}
				if (rightCylinders != null)
				{
					rightCylinders.localRotation = Quaternion.Euler(0f, 0f, _rotateAngle);
				}
			}
		}

		private bool CheckIsVisible()
		{
			if (_camera == null)
			{
				_camera = V.CoreGameManager.GetComponent<Camera>();
			}
			Vector3 vector = _camera.WorldToViewportPoint(base.transform.position);
			if ((_camera.transform.position - base.transform.position).magnitude < 20f)
			{
				return true;
			}
			if (vector.x < 1f && vector.x > 0f && vector.y < 1f && vector.y > 0f)
			{
				if (vector.z > 0f)
				{
					return vector.z < 25f;
				}
				return false;
			}
			return false;
		}

		public async UniTask MoveRotateCoroutine(float duration, CancellationToken ct = default(CancellationToken))
		{
			_ = 1;
			try
			{
				if (CheckIsVisible())
				{
					waterParticles.Play();
					AudioWrapper.PostEvent("Play_SFX_Washing_Car", base.gameObject);
					float currentDuration = duration;
					_rotateAngle = 90f;
					while (currentDuration > duration * 0.15f)
					{
						currentDuration -= Time.deltaTime;
						MoveRotateOnce();
						await UniTask.Yield(ct);
					}
					waterParticles.Stop();
					AudioWrapper.PostEvent("Stop_SFX_Washing_Car", base.gameObject);
					_rotateAngle = 0f;
					while (_rotateAngle < 90f)
					{
						MoveRotateOnce(isRotate: false);
						await UniTask.Yield(ct);
					}
					Transform obj = base.transform;
					obj.localPosition = _originPos;
					obj.localRotation = _originRot;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}
	}
}
