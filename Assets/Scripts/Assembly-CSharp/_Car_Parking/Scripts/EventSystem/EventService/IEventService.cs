using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	public interface IEventService
	{
		UniTask Initialized { get; }

		IReadOnlyList<Event> CurrentEvents { get; }

		event Action OnEventsRetrieved;

		event Action<EventType> OnEventClaimed;

		event Action<EventType> OnEventUpdated;

		UniTask<List<Event>> RetrieveCurrentEvents(bool forceRefresh = false);

		void CacheCurrentEvents();

		void CacheEventByName(EventType eventType);

		void RemoveCacheEventByName(EventType eventType);

		UniTask<List<Event>> CheckForUncachedEvents(bool forceRefresh = false);

		UniTask<List<Event>> CheckForFinishedEvents();

		Event GetEventByName(EventType eventType);

		bool TryUpdateEvent(EventType eventType, Action<Event> mutate);

		UniTask<int> SubmitEventRecord(EventType eventType, PayloadData data);

		UniTask<ClaimRewardResult> ClaimReward(EventType eventType);

		UniTask WaitUntilInitializedAsync();

		void Reset();
	}
}
