using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.ArtistTools.Enums;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class DestroyableMeshPhysics : DestroyableMeshBase
	{
		private const float ImpulsePowerMin = 6.4f;

		private const float ImpulsePowerMax = 9.6f;

		private const float ImpulseRandomDir = 0.2f;

		[SerializeField]
		private Collider[] Colliders;

		[SerializeField]
		private float DestroyableObjectsMass = 7f;

		public Collider[] AllColliders => Colliders;

		protected override bool StartCollisionEffect(GameObject other)
		{
			Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
			Vector3 normalized = componentInParent.velocity.normalized;
			float magnitude = componentInParent.velocity.magnitude;
			for (int i = 0; i < DestroyableObjects.Count; i++)
			{
				if ((bool)DestroyableObjects[i])
				{
					DestroyableObjects[i].layer = LayerMask.NameToLayer("short distance");
					Rigidbody rigidbody = DestroyableObjects[i].AddComponent<Rigidbody>();
					rigidbody.mass = DestroyableObjectsMass;
					rigidbody.isKinematic = false;
					rigidbody.AddForce((normalized + Random.insideUnitSphere * 0.2f).normalized * magnitude * Random.Range(6.4f, 9.6f), ForceMode.Impulse);
					rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
			for (int j = 0; j < Colliders.Length; j++)
			{
				if ((bool)Colliders[j])
				{
					Colliders[j].enabled = true;
				}
			}
			return true;
		}

		private void SetRefs()
		{
			if (DestroyableObjects != null && DestroyableObjects.Count > 0 && (bool)DestroyableObjects[0])
			{
				Colliders = DestroyableObjects[0].transform.parent.GetComponentsInChildren<Collider>();
			}
			if (DestroyableObjects != null && DestroyableObjects.Count > 0 && (bool)DestroyableObjects[0] && Colliders.Length == 0)
			{
				Colliders = DestroyableObjects[0].transform.GetComponentsInChildren<Collider>();
			}
			if ((bool)MainCollider)
			{
				List<Collider> list = new List<Collider>(Colliders);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == MainCollider)
					{
						list.RemoveAt(i);
						i--;
					}
				}
				Colliders = list.ToArray();
			}
			for (int j = 0; j < Colliders.Length; j++)
			{
				Colliders[j].enabled = false;
				if (DestroyableObjects != null && !DestroyableObjects.Contains(Colliders[j].gameObject))
				{
					Colliders[j].gameObject.layer = LayerMask.NameToLayer("short distance");
				}
			}
			if ((bool)MainCollider)
			{
				MainCollider.isTrigger = base.CollisionType == RegisterCollisionType.Trigger;
			}
		}
	}
}
