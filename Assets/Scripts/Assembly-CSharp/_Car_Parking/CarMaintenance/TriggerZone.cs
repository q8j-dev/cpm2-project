using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.CarMaintenance
{
	[RequireComponent(typeof(CapsuleCollider))]
	public class TriggerZone : MonoBehaviour
	{
		private bool _isOccupied;

		private CapsuleCollider _collider;

		private RaycastHit[] _results;

		[SerializeField]
		private LayerMask _carLayer;

		public event Action<GameObject> OnZoneTriggerred;

		private void Awake()
		{
			_collider = GetComponent<CapsuleCollider>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((!other.CompareTag("Car") && !other.CompareTag("OnlineCar")) || _isOccupied)
			{
				return;
			}
			_isOccupied = true;
			GameObject gameObject = other.transform.parent.gameObject;
			if (gameObject.TryGetComponent<Powertrain>(out var component) && !(component.SpeedMps > 10f))
			{
				_isOccupied = true;
				VehicleData component2 = gameObject.GetComponent<VehicleData>();
				if (!(component2.Driver != V.DriverPerson) && !component2.Towing && !(component2.TowingComposition.LoadCar != null))
				{
					OnZoneTriggerred?.Invoke(gameObject);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Car") || other.CompareTag("OnlineCar"))
			{
				int num = Physics.SphereCastNonAlloc(_collider.bounds.center + Vector3.up, _collider.radius, Vector3.down, _results, 2f, _carLayer);
				_isOccupied = num > 0;
			}
		}

		private void OnDisable()
		{
			_isOccupied = false;
		}

		private void OnEnable()
		{
			int num = Physics.SphereCastNonAlloc(_collider.bounds.center + Vector3.up, _collider.radius, Vector3.down, _results, 2f, _carLayer);
			_isOccupied = num > 1;
		}
	}
}
