using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class SellCarTriggerHelperNoMoveV2 : SellCarTriggerHelperNoMove
	{
		private const float DistanceToDisableSquare = 64f;

		private const float ColliderSizeMargin = 0.2f;

		private GameObject _car;

		private Collider _carCollider;

		private BoxCollider _fakeCarCollider;

		private CancellationTokenSource _cancellationTokenSource;

		private CarRef _carRef;

		public override void Init(GameObject car)
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
			_cancellationTokenSource = new CancellationTokenSource();
			InitAsync(car, _cancellationTokenSource.Token).Forget();
		}

		private void Awake()
		{
			GameObject gameObject = new GameObject("CarSellBlockCollider");
			gameObject.layer = LayerMask.NameToLayer("TransparentFX");
			gameObject.tag = "CarSellBlockCollider";
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			_fakeCarCollider = gameObject.AddComponent<BoxCollider>();
			_fakeCarCollider.excludeLayers = 1 << LayerMask.NameToLayer("Wheel");
			_carRef = gameObject.AddComponent<CarRef>();
		}

		private void OnDestroy()
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
		}

		private void FixedUpdate()
		{
			UpdateFakeCarCollider();
		}

		private void Update()
		{
			UpdateFakeCarCollider();
		}

		private void LateUpdate()
		{
			UpdateFakeCarCollider();
		}

		private async UniTaskVoid InitAsync(GameObject car, CancellationToken cancellationToken)
		{
			DeleteIgnoreCollision();
			_car = car;
			_carCollider = null;
			_carRef.Car = _car;
			if ((bool)_car && _car.TryGetComponent<VehicleData>(out var vehicleData) && (bool)vehicleData.Collider)
			{
				_carCollider = vehicleData.Collider;
				new Bounds(Vector3.zero, Vector3.zero);
				Bounds bounds;
				while (true)
				{
					if (cancellationToken.IsCancellationRequested || !vehicleData || !_carCollider)
					{
						return;
					}
					Quaternion rotation = _carCollider.transform.rotation;
					vehicleData.Collider.transform.rotation = Quaternion.identity;
					bounds = _carCollider.bounds;
					_carCollider.transform.rotation = rotation;
					if (bounds.size.sqrMagnitude > 0.1f)
					{
						break;
					}
					await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				}
				_fakeCarCollider.size = bounds.size + Vector3.one * 0.2f;
				_fakeCarCollider.center = bounds.center - vehicleData.Collider.transform.position;
				Physics.IgnoreCollision(_carCollider, _fakeCarCollider, ignore: true);
			}
			base.enabled = true;
			_fakeCarCollider.gameObject.SetActive(value: true);
			UpdateFakeCarCollider();
		}

		private void UpdateFakeCarCollider()
		{
			if ((bool)_car && (bool)_carCollider)
			{
				_fakeCarCollider.transform.position = _carCollider.transform.position;
				_fakeCarCollider.transform.rotation = _carCollider.transform.rotation;
				if (Extensions.DistanceSquare(_car.transform.position, base.transform.position) > 64f)
				{
					Disable();
				}
			}
			else
			{
				Disable();
			}
		}

		private void Disable()
		{
			base.enabled = false;
			_fakeCarCollider.gameObject.SetActive(value: false);
			DeleteIgnoreCollision();
			_car = null;
			_carCollider = null;
		}

		private void DeleteIgnoreCollision()
		{
			if ((bool)_carCollider)
			{
				Physics.IgnoreCollision(_carCollider, _fakeCarCollider, ignore: false);
			}
		}
	}
}
