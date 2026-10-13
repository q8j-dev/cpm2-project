using UnityEngine;

namespace _Car_Parking.Scripts.Car.Visual
{
	public class WindshieldBrakeVfx : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem ParticleSystem = default;

		[SerializeField]
		[Tooltip("May throw error if deleted within a parent")]
		private bool ChangeParent = default;

		private Transform _rootParentTransform;

		private void Awake()
		{
			if (_rootParentTransform == null)
			{
				_rootParentTransform = base.transform.parent;
			}
		}

		private void Reset()
		{
			SetVfxPosition(_rootParentTransform);
		}

		public void ActivateAt(Transform actionObjectTransform)
		{
			SetVfxPosition(actionObjectTransform);
			PlayVfx();
		}

		public void ReturnToPool()
		{
			SetVfxPosition(_rootParentTransform);
		}

		private void SetVfxPosition(Transform actionObjectTransform)
		{
			if ((bool)actionObjectTransform)
			{
				if (!ChangeParent)
				{
					base.transform.SetPositionAndRotation(actionObjectTransform.position, actionObjectTransform.rotation);
				}
				else
				{
					base.transform.SetParent(actionObjectTransform, worldPositionStays: false);
				}
			}
		}

		private void PlayVfx()
		{
			if ((bool)ParticleSystem)
			{
				ParticleSystem.Stop();
				ParticleSystem.Play();
			}
		}
	}
}
