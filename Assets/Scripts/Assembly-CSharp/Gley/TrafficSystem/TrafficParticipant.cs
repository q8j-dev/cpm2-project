using System.Collections;
using Gley.TrafficSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem
{
	[RequireComponent(typeof(Rigidbody))]
	public class TrafficParticipant : MonoBehaviour, ITrafficParticipant
	{
		private Rigidbody _rb;

		private bool _initialized;

		private Transform _myTransform;

		private void OnEnable()
		{
			StartCoroutine(Initialize());
		}

		private IEnumerator Initialize()
		{
			while (!TrafficManager.Instance.Initialized)
			{
				yield return null;
			}
			_rb = GetComponent<Rigidbody>();
			_myTransform = base.transform;
			_initialized = true;
		}

		public bool AlreadyCollidingWith(Collider[] allColliders)
		{
			return false;
		}

		public float GetCurrentSpeedMS()
		{
			if (!_initialized)
			{
				return 0f;
			}
			return _rb.velocity.magnitude;
		}

		public Vector3 GetHeading()
		{
			return _myTransform.forward;
		}
	}
}
