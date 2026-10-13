using System;
using UnityEngine;
using _Car_Parking.Scripts.ArtistTools;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyTriggerPoint : MonoBehaviour
	{
		[SerializeField]
		private float TriggerCoolDown = 3f;

		public RallyTriggerPointType PointType;

		private float _lastPassTime;

		private DestroyableMeshBase _destroyableMeshBase;

		private void Start()
		{
			_destroyableMeshBase = GetComponent<DestroyableMeshBase>();
			if ((bool)_destroyableMeshBase)
			{
				DestroyableMeshBase destroyableMeshBase = _destroyableMeshBase;
				destroyableMeshBase.OnTriggerEnterAction = (Action<Collider>)Delegate.Combine(destroyableMeshBase.OnTriggerEnterAction, new Action<Collider>(OnTriggerEnter));
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((bool)other.attachedRigidbody)
			{
				RallyCar component = other.attachedRigidbody.GetComponent<RallyCar>();
				if (component != null && !(Time.time - _lastPassTime <= TriggerCoolDown))
				{
					_lastPassTime = Time.time;
					component.OnRallyPointTriggered(PointType);
				}
			}
		}

		private void OnDestroy()
		{
			if ((bool)_destroyableMeshBase)
			{
				DestroyableMeshBase destroyableMeshBase = _destroyableMeshBase;
				destroyableMeshBase.OnTriggerEnterAction = (Action<Collider>)Delegate.Remove(destroyableMeshBase.OnTriggerEnterAction, new Action<Collider>(OnTriggerEnter));
			}
		}
	}
}
