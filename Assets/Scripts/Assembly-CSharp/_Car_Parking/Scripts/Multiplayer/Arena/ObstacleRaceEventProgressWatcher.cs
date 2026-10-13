using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceEventProgressWatcher
	{
		private const int NotWatching = -1;

		private readonly IEventService _eventService;

		private int _snapshot = -1;

		public ObstacleRaceEventProgressWatcher(IEventService eventService)
		{
			_eventService = eventService;
		}

		public void Begin()
		{
			_snapshot = CurrentEvent()?.Progress ?? (-1);
		}

		public void Reset()
		{
			_snapshot = -1;
		}

		public bool TryGetGain(out ObstacleRaceEventProgress progress)
		{
			progress = default;
			if (_snapshot == -1)
			{
				return false;
			}
			Event obj = CurrentEvent();
			if (obj == null || obj.Progress <= _snapshot || obj.Status == EventStatus.Finished)
			{
				return false;
			}
			progress = new ObstacleRaceEventProgress(_snapshot, obj.Progress, obj.MaxProgress, obj.Status);
			return true;
		}

		private Event CurrentEvent()
		{
			return _eventService?.GetEventByName(EventType.ObstacleRace);
		}
	}
}
