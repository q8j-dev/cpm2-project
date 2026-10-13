using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public static class AmbienceStateZones
	{
		private const float ListenerProbeRadius = 0.01f;

		public static void ReapplyOccupiedZones()
		{
			Camera main = Camera.main;
			if ((bool)main)
			{
				Collider[] array = Physics.OverlapSphere(main.TryGetComponent<Collider>(out var component) ? component.bounds.center : main.transform.position, 0.01f, -1, QueryTriggerInteraction.Collide);
				for (int i = 0; i < array.Length; i++)
				{
					GetZone(array[i])?.ApplyEnterState();
				}
			}
		}

		private static IAmbienceStateZone GetZone(Collider zoneCollider)
		{
			if (!zoneCollider.isTrigger)
			{
				return null;
			}
			IAmbienceStateZone component = zoneCollider.GetComponent<IAmbienceStateZone>();
			if (component != null)
			{
				return component;
			}
			Transform parent = zoneCollider.transform.parent;
			if (!parent)
			{
				return null;
			}
			return parent.GetComponent<IAmbienceStateZone>();
		}
	}
}
