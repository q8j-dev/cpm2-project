using MVPFree.MapIcons.Enum;
using UnityEngine;

namespace _Car_Parking.Scripts.GameActivity
{
	public readonly struct GameActivityEntry
	{
		public readonly GameActivityType ActivityType;

		public readonly object ActivityId;

		public readonly string Name;

		public readonly string Description;

		public readonly IconType IconType;

		public readonly Vector3? MapPosition;

		public GameActivityEntry(GameActivityType activityType, object activityId, string name, string description, IconType iconType, Vector3? mapPosition)
		{
			ActivityType = activityType;
			ActivityId = activityId;
			Name = name;
			Description = description;
			IconType = iconType;
			MapPosition = mapPosition;
		}
	}
}
