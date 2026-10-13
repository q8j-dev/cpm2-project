using System;
using System.Collections.Generic;
using Gley.UrbanSystem.Internal;

namespace Gley.TrafficSystem.Internal
{
	[Serializable]
	public abstract class GenericIntersection : IIntersection
	{
		protected List<int> _carsInIntersection;

		public abstract bool IsPathFree(int waypointIndex);

		public void VehicleEnter(int vehicleIndex)
		{
			_carsInIntersection.Add(vehicleIndex);
		}

		public void VehicleLeft(int vehicleIndex)
		{
			_carsInIntersection.Remove(vehicleIndex);
		}

		public abstract void PedestrianPassed(int agentIndex);

		public abstract void UpdateIntersection(float realtimeSinceStartup);

		public abstract int[] GetPedStopWaypoint();

		public abstract string GetName();

		public abstract List<int> GetStopWaypoints();

		public void RemoveVehicle(int index)
		{
			VehicleLeft(index);
		}

		public virtual void ResetIntersection()
		{
			_carsInIntersection = new List<int>();
		}
	}
}
