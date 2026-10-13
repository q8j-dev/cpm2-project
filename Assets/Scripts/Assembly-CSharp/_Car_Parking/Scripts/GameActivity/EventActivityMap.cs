using System.Collections.Generic;
using _Car_Parking.Scripts.EventSystem;

namespace _Car_Parking.Scripts.GameActivity
{
	public static class EventActivityMap
	{
		private static readonly Dictionary<EventType, GameActivityType> ActivityByEvent = new Dictionary<EventType, GameActivityType>();

		private static readonly Dictionary<GameActivityType, EventType> EventByActivity = BuildReverse();

		public static bool HasActivity(EventType eventType)
		{
			return ActivityByEvent.ContainsKey(eventType);
		}

		public static bool TryGetActivity(EventType eventType, out GameActivityType activityType)
		{
			return ActivityByEvent.TryGetValue(eventType, out activityType);
		}

		public static bool HasEvent(GameActivityType activityType)
		{
			return EventByActivity.ContainsKey(activityType);
		}

		public static bool TryGetEvent(GameActivityType activityType, out EventType eventType)
		{
			return EventByActivity.TryGetValue(activityType, out eventType);
		}

		private static Dictionary<GameActivityType, EventType> BuildReverse()
		{
			Dictionary<GameActivityType, EventType> dictionary = new Dictionary<GameActivityType, EventType>(ActivityByEvent.Count);
			foreach (KeyValuePair<EventType, GameActivityType> item in ActivityByEvent)
			{
				dictionary[item.Value] = item.Key;
			}
			return dictionary;
		}
	}
}
