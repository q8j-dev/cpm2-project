using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class IdleVehiclesData
	{
		private readonly List<VehicleComponent> _idleVehicles;

		public List<VehicleComponent> IdleVehicles => _idleVehicles;

		public IdleVehiclesData(List<VehicleComponent> idleVehicles)
		{
			_idleVehicles = idleVehicles;
		}

		public void AddVehicle(VehicleComponent vehicle)
		{
			if (!_idleVehicles.Contains(vehicle) && !vehicle.gameObject.activeSelf && !vehicle.Ignored)
			{
				_idleVehicles.Add(vehicle);
			}
		}

		public void RemoveVehicle(VehicleComponent vehicle)
		{
			_idleVehicles.Remove(vehicle);
		}

		public VehicleComponent GetRandomVehicleOfType(VehicleTypes type)
		{
			List<VehicleComponent> list = _idleVehicles.Where((VehicleComponent cond) => cond.vehicleType == type).ToList();
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count)];
			}
			return null;
		}

		public VehicleComponent GetRandomVehicle()
		{
			if (_idleVehicles.Count == 0)
			{
				return null;
			}
			return _idleVehicles[Random.Range(0, _idleVehicles.Count)];
		}
	}
}
