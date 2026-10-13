using System;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public struct SpawnWaypoint
	{
		[SerializeField]
		private int[] _allowedAgents;

		[SerializeField]
		private int _waypointIndex;

		[SerializeField]
		private int _priority;

		public readonly int[] AllowedAgents => _allowedAgents;

		public readonly int WaypointIndex => _waypointIndex;

		public readonly int Priority => _priority;

		public SpawnWaypoint(int waypointIndex, int[] allowedVehicles, int priority)
		{
			_waypointIndex = waypointIndex;
			_allowedAgents = allowedVehicles;
			_priority = priority;
		}
	}
}
