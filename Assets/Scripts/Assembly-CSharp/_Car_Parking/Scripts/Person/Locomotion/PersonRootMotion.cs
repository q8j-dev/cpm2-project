using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	[DisallowMultipleComponent]
	public class PersonRootMotion : MonoBehaviour
	{
		private Animator _animator;

		private Vector3 _accumulated;

		private Quaternion _accumulatedRotation = Quaternion.identity;

		public bool Active { get; set; }

		public bool PassThrough { get; set; }

		private void Awake()
		{
			TryGetComponent<Animator>(out _animator);
		}

		private void OnAnimatorMove()
		{
			if (!_animator)
			{
				return;
			}
			if (PassThrough)
			{
				if (!_animator.applyRootMotion)
				{
					_animator.applyRootMotion = true;
				}
				base.transform.position += _animator.deltaPosition;
				base.transform.rotation = _animator.deltaRotation * base.transform.rotation;
			}
			else if (Active)
			{
				Vector3 deltaPosition = _animator.deltaPosition;
				deltaPosition.y = 0f;
				_accumulated += deltaPosition;
				_accumulatedRotation = _animator.deltaRotation * _accumulatedRotation;
			}
		}

		public Vector3 ConsumeHorizontalDelta()
		{
			Vector3 accumulated = _accumulated;
			_accumulated = Vector3.zero;
			return accumulated;
		}

		public Quaternion ConsumeRotation()
		{
			Quaternion accumulatedRotation = _accumulatedRotation;
			_accumulatedRotation = Quaternion.identity;
			return accumulatedRotation;
		}

		public void ResetDelta()
		{
			_accumulated = Vector3.zero;
			_accumulatedRotation = Quaternion.identity;
		}
	}
}
