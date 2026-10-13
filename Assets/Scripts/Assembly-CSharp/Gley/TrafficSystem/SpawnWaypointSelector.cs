using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public delegate int SpawnWaypointSelector(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType, bool useWaypointPriority);
}
