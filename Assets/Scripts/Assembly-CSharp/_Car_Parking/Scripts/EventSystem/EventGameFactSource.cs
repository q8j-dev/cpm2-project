using System;
using System.Collections.Generic;
using VContainer.Unity;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.EventSystem
{
	public class EventGameFactSource : IStartable, IDisposable
	{
		private readonly IGameFactManager _manager;

		private readonly IEventService _eventService;

		private readonly IReadOnlyDictionary<EventType, GameFact> _map;

		public EventGameFactSource(IGameFactManager manager, IEventService eventService, IReadOnlyDictionary<EventType, GameFact> map)
		{
			_manager = manager;
			_eventService = eventService;
			_map = map;
		}

		public void Start()
		{
			_eventService.OnEventsRetrieved += Reevaluate;
			Reevaluate();
		}

		public void Dispose()
		{
			_eventService.OnEventsRetrieved -= Reevaluate;
			_manager.Release(this);
		}

		private void Reevaluate()
		{
			HashSet<EventType> hashSet = new HashSet<EventType>();
			IReadOnlyList<Event> currentEvents = _eventService.CurrentEvents;
			if (currentEvents != null)
			{
				foreach (Event item in currentEvents)
				{
					hashSet.Add(item.Type);
				}
			}
			foreach (KeyValuePair<EventType, GameFact> item2 in _map)
			{
				_manager.Set(item2.Value, this, hashSet.Contains(item2.Key));
			}
		}
	}
}
