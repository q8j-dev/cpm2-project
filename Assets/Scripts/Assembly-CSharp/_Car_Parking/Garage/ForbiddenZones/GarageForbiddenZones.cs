using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Garage.ForbiddenZones
{
	[DisallowMultipleComponent]
	public class GarageForbiddenZones : MonoBehaviour, IGarageForbiddenZones
	{
		[SerializeField]
		[Tooltip("Trigger colliders that represent forbidden zones for this garage type")]
		private List<Collider> _zones = new List<Collider>();

		private IForbiddenZonesRegistrar _registrar;

		public IReadOnlyList<Collider> Zones => _zones;

		[Inject]
		public void Construct(IForbiddenZonesRegistrar registrar)
		{
			_registrar = registrar;
		}

		private void OnEnable()
		{
			_registrar?.Register(this);
		}

		private void OnDisable()
		{
			_registrar?.Unregister(this);
		}

		private void Reset()
		{
			foreach (Collider zone in _zones)
			{
				if ((bool)zone)
				{
					zone.isTrigger = true;
				}
			}
		}
	}
}
