using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.BannerSystem.Rules
{
	public class EventsBannerRule : IBannerRule
	{
		private readonly IEventService _eventService;

		[Inject]
		public EventsBannerRule(IEventService eventService)
		{
			_eventService = eventService;
		}

		public async UniTask<bool> CanShow()
		{
			List<Event> list = await SetNewEvents();
			return list != null && list.Count != 0;
		}

		private async UniTask<List<Event>> SetNewEvents()
		{
			List<Event> newEvents = new List<Event>();
			List<Event> list = await _eventService.CheckForUncachedEvents();
			if (list != null)
			{
				foreach (Event item in list)
				{
					if (item.Status != EventStatus.Claimed)
					{
						newEvents.Add(item);
					}
				}
			}
			return newEvents;
		}
	}
}
