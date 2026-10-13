using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.ProceduralDoors
{
	public class StayInColliderLink : MonoBehaviour
	{
		private List<EnableIfPlayerStaysInCollider> _enableIfPlayerStaysInColliders;

		private Collider _collider;

		private readonly List<Collider> _occupantColliders = new List<Collider>();

		public void Init(EnableIfPlayerStaysInCollider enableIfPlayerStaysInCollider)
		{
			base.gameObject.name = "StayInColliderLink";
			TryGetComponent<Collider>(out _collider);
			if (_enableIfPlayerStaysInColliders == null)
			{
				_enableIfPlayerStaysInColliders = new List<EnableIfPlayerStaysInCollider>();
			}
			_enableIfPlayerStaysInColliders.Add(enableIfPlayerStaysInCollider);
		}

		public void OnEnter()
		{
			foreach (EnableIfPlayerStaysInCollider enableIfPlayerStaysInCollider in _enableIfPlayerStaysInColliders)
			{
				enableIfPlayerStaysInCollider.OnEnter();
			}
		}

		public void OnExit()
		{
			if (IsPlayerStillInside())
			{
				return;
			}
			foreach (EnableIfPlayerStaysInCollider enableIfPlayerStaysInCollider in _enableIfPlayerStaysInColliders)
			{
				enableIfPlayerStaysInCollider.OnExit();
			}
		}

		private bool IsPlayerStillInside()
		{
			GameObject gameObject = (FlagsStorage.OutOfCar ? V.DriverPerson : V.Car);
			if (!_collider || !gameObject || !gameObject.activeInHierarchy)
			{
				return false;
			}
			Transform transform = _collider.transform;
			gameObject.GetComponentsInChildren(includeInactive: false, _occupantColliders);
			foreach (Collider occupantCollider in _occupantColliders)
			{
				if (IsComparable(occupantCollider))
				{
					Transform transform2 = occupantCollider.transform;
					if (Physics.ComputePenetration(_collider, transform.position, transform.rotation, occupantCollider, transform2.position, transform2.rotation, out var _, out var _))
					{
						_occupantColliders.Clear();
						return true;
					}
				}
			}
			_occupantColliders.Clear();
			return false;
		}

		private static bool IsComparable(Collider occupantCollider)
		{
			if (occupantCollider.enabled && !(occupantCollider is WheelCollider))
			{
				return !(occupantCollider is MeshCollider meshCollider) || meshCollider.convex;
			}
			return false;
		}
	}
}
