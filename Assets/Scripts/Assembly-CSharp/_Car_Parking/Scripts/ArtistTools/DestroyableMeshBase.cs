using System;
using System.Collections.Generic;
using System.Linq;
using AK.Wwise;
using FunctionalExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using _Car_Parking.Scripts.ArtistTools.Aspects;
using _Car_Parking.Scripts.ArtistTools.Enums;
using _Car_Parking.Scripts.ArtistTools.Trackables;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.ArtistTools
{
	public abstract class DestroyableMeshBase : TrackableWithAspects, IInvisibleRegister, IDestroyAspect, IAspect
	{
		[SerializeField]
		public List<GameObject> DisableObjectsInCollision;

		[SerializeField]
		public List<GameObject> DestroyableObjects;

		[SerializeField]
		public Collider MainCollider;

		[SerializeField]
		private VfxSpawner VfxSpawner = default;

		[SerializeField]
		private MeshRenderer MainRenderer = default;

		[SerializeField]
		private LODGroup LodGroupHandler = default;

		[SerializeField]
		private AK.Wwise.Event CollisionSoundEvent;

		protected readonly List<InvisibleHandler> _invisibles = new List<InvisibleHandler>();

		public Action<Collider> OnTriggerEnterAction;

		[SerializeField]
		public float DamageResistanceMass = 60f;

		[field: SerializeField]
		[field: FormerlySerializedAs("CollisionType")]
		public RegisterCollisionType CollisionType { get; private set; }

		public bool IsTriggerActivated { get; private set; }

		public void UnregisterInvisible(InvisibleHandler handler)
		{
			_invisibles.Remove(handler);
			if (!_invisibles.Any() || !_invisibles.All(F.Exists))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public virtual void OnDestroyImpl(TrackableWithAspects _)
		{
			CollisionSoundEvent = null;
		}

		protected abstract bool StartCollisionEffect(GameObject other);

		private void OnTriggerEnter(Collider other)
		{
			if ((bool)other && (bool)other.gameObject)
			{
				TryActivate(other.gameObject);
				OnTriggerEnterAction?.Invoke(other);
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if ((bool)other.gameObject)
			{
				TryActivate(other.gameObject);
			}
		}

		private void TryActivate(GameObject other)
		{
			if (other.CompareTag("Car") || other.CompareTag("OnlineCar"))
			{
				ActivateCollisionEffect(other);
			}
		}

		private void ApplyDamageToCar(GameObject other)
		{
			if (DamageResistanceMass <= 0f || !other)
			{
				return;
			}
			VehicleDamage componentInParent = other.GetComponentInParent<VehicleDamage>();
			if ((bool)componentInParent)
			{
				Rigidbody componentInParent2 = other.GetComponentInParent<Rigidbody>();
				Vector3 vector = (componentInParent2 ? componentInParent2.velocity : Vector3.zero);
				float magnitude = vector.magnitude;
				if (!(magnitude < 0.001f))
				{
					Vector3 inward = -vector / magnitude;
					Vector3 vector2 = (componentInParent2 ? componentInParent2.worldCenterOfMass : other.transform.position);
					Vector3 vector3 = ClosestSurfacePoint(MainCollider, vector2, base.transform.position);
					Collider collider = FindHullCollider(componentInParent);
					Vector3 worldPoint = (collider ? ClosestSurfacePoint(collider, vector3, vector2) : (componentInParent2 ? componentInParent2.ClosestPointOnBounds(vector3) : vector2));
					componentInParent.ApplyExternalImpact(worldPoint, inward, DamageResistanceMass * magnitude, vector);
				}
			}
		}

		private static Collider FindHullCollider(Component car)
		{
			Collider[] componentsInChildren = car.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if ((bool)componentsInChildren[i] && !componentsInChildren[i].isTrigger)
				{
					return componentsInChildren[i];
				}
			}
			return null;
		}

		private static Vector3 ClosestSurfacePoint(Collider collider, Vector3 target, Vector3 fallback)
		{
			if (!collider)
			{
				return fallback;
			}
			Vector3 vector = collider.ClosestPoint(target);
			if ((vector - target).sqrMagnitude > 1E-08f)
			{
				return vector;
			}
			if (!collider.bounds.Contains(target))
			{
				return collider.ClosestPointOnBounds(target);
			}
			return vector;
		}

		private void ActivateCollisionEffect(GameObject other)
		{
			if (IsTriggerActivated)
			{
				return;
			}
			IsTriggerActivated = true;
			ApplyDamageToCar(other);
			if ((bool)MainCollider)
			{
				UnityEngine.Object.Destroy(MainCollider);
			}
			if ((bool)MainRenderer)
			{
				MainRenderer.enabled = false;
			}
			if ((bool)VfxSpawner)
			{
				VfxSpawner.TryActivateVfxEffect();
			}
			foreach (GameObject item in DisableObjectsInCollision)
			{
				if ((bool)item)
				{
					item.SetActive(value: false);
				}
			}
			AudioWrapper.PostProcess(CollisionSoundEvent, base.gameObject);
			if (!StartCollisionEffect(other))
			{
				DeactivateLOD();
				return;
			}
			DeactivateLOD();
			RegisterInvisibles();
		}

		private void DeactivateLOD()
		{
			if (!LodGroupHandler)
			{
				return;
			}
			LodGroupHandler.enabled = false;
			LOD[] lODs = LodGroupHandler.GetLODs();
			if (lODs.IsNullOrEmpty())
			{
				return;
			}
			LOD[] array = lODs;
			for (int i = 0; i < array.Length; i++)
			{
				LOD lOD = array[i];
				if (lOD.renderers.IsNullOrEmpty())
				{
					continue;
				}
				Renderer[] renderers = lOD.renderers;
				foreach (Renderer renderer in renderers)
				{
					if ((bool)renderer && !DestroyableObjects.Contains(renderer.gameObject))
					{
						renderer.enabled = false;
					}
				}
			}
		}

		private void RegisterInvisibles()
		{
			foreach (GameObject destroyableObject in DestroyableObjects)
			{
				if ((bool)destroyableObject)
				{
					InvisibleHandler invisibleHandler = destroyableObject.AddComponent<InvisibleHandler>();
					invisibleHandler.Init(this);
					_invisibles.Add(invisibleHandler);
				}
			}
		}

		private void OnBecameInvisible()
		{
			if (IsTriggerActivated && (!_invisibles.Any() || !_invisibles.All(F.Exists)))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
