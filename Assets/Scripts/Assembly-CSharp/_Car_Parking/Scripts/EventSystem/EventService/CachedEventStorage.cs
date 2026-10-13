using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	public class CachedEventStorage
	{
		private const string EventKey = "event_";

		public void CacheEvent(Event @event)
		{
			string key = "event_" + @event.Type;
			string value = JsonConvert.SerializeObject(@event);
			ObscuredPrefs.SetString(key, value);
		}

		public Event GetCachedEventDataById(EventType type)
		{
			string value = ObscuredPrefs.GetString("event_" + type);
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			return JsonConvert.DeserializeObject<Event>(value);
		}

		public void RemoveCachedEvent(EventType type)
		{
			ObscuredPrefs.DeleteKey("event_" + type);
		}

		public bool CachedEventExists(EventType type)
		{
			return ObscuredPrefs.HasKey("event_" + type);
		}
	}
}
