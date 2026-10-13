using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	[RequireComponent(typeof(CapsuleCollider))]
	public abstract class BuyTrigger : MonoBehaviour
	{
		private RaycastHit[] _results = new RaycastHit[5];

		private CapsuleCollider _collider;

		protected IDisposable _checkOperation;

		private int _checkTriggerLayers;

		public const int Price = 200000;

		private void Awake()
		{
			_collider = GetComponent<CapsuleCollider>();
			_checkTriggerLayers = LayerMask.GetMask("CarBody", "DriverPerson", "car");
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!FlagsStorage.IsPhotoMode)
			{
				CheckObject(other.gameObject);
			}
		}

		private bool CheckObject(GameObject checkObject)
		{
			if (checkObject.CompareTag("Car") || checkObject.CompareTag("OnlineCar"))
			{
				VehicleData componentInParent = checkObject.GetComponentInParent<VehicleData>();
				if (!componentInParent || componentInParent.gameObject != V.Car)
				{
					return false;
				}
				if (componentInParent.GetPowertrain.SpeedKmh > 25f)
				{
					_checkOperation?.Dispose();
					_checkOperation = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
					{
						_checkOperation = null;
						CheckTrigger();
					}).AddTo(this);
					return false;
				}
				Execute();
				return true;
			}
			if (checkObject.CompareTag("DriverPerson"))
			{
				if (checkObject.gameObject != V.DriverPerson)
				{
					return false;
				}
				Execute();
				return true;
			}
			return false;
		}

		protected abstract void Execute();

		private void CheckTrigger()
		{
			int num = Physics.SphereCastNonAlloc(_collider.bounds.center + Vector3.up * 2f, _collider.radius, Vector3.down, _results, 5f, _checkTriggerLayers);
			for (int i = 0; i < num && !CheckObject(_results[i].collider.gameObject); i++)
			{
			}
		}
	}
}
