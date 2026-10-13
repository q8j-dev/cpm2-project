using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Garage.ForbiddenZones
{
	public interface IGarageForbiddenZones
	{
		IReadOnlyList<Collider> Zones { get; }
	}
}
