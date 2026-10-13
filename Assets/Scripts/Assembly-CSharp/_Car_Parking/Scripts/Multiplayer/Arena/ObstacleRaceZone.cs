using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceZone : MonoBehaviour
	{
		public ObstacleRaceZoneDifficulty Difficulty;

		public ObstacleRaceMechanicType[] Mechanics;

		[Tooltip("Everything else the track planner sorts zones by. Tick what applies.")]
		public ObstacleRaceZoneTrait Traits;

		[Tooltip("Stable analytics id. Leave empty to report the prefab name instead.")]
		public string ZoneId;

		[Tooltip("Where the next zone's pivot lands. Both position and rotation are used.")]
		public Transform Exit;

		[Tooltip("The zone's checkpoint trigger. Required: one per zone.")]
		public Collider Checkpoint;

		[Tooltip("Progress polyline through this zone. May be empty.")]
		public Transform[] Waypoints;

		public string AnalyticsId
		{
			get
			{
				if (!string.IsNullOrEmpty(ZoneId))
				{
					return ZoneId;
				}
				return base.name;
			}
		}

		private void OnValidate()
		{
			_ = (bool)Exit;
			if ((bool)Checkpoint)
			{
				_ = Checkpoint.isTrigger;
			}
			if (Waypoints != null)
			{
				for (int i = 0; i < Waypoints.Length; i++)
				{
					_ = (bool)Waypoints[i];
				}
			}
		}
	}
}
