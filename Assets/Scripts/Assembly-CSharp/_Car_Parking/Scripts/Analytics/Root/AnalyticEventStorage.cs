using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Root.Batching;
using _Car_Parking.Scripts.Analytics.Root.Data;

namespace _Car_Parking.Scripts.Analytics.Root
{
	public class AnalyticEventStorage : IBatchSource
	{
		private const string Key = "CPM_A_events_1";

		private readonly AnalyticCoreSetting _setting;

		private readonly AnalyticsRepository<List<Event>> _eventRepository = new AnalyticsRepository<List<Event>>("CPM_A_events_1");

		private List<Event> _events;

		public event Action<List<Event>> EventsReadyForBatch;

		public AnalyticEventStorage(AnalyticCoreSetting setting)
		{
			_setting = setting;
		}

		public void Initialize()
		{
			List<Event> list = _eventRepository.LoadData();
			_events = list ?? new List<Event>();
		}

		public void AddEvent(Parameter[] parameters)
		{
			_events.Add(new Event
			{
				SendTime = DateTime.UtcNow,
				Parameters = parameters
			});
			CheckForBatches();
			_eventRepository.SaveData(_events);
		}

		public void FlushToBatch()
		{
			if (_events.Count != 0)
			{
				EventsReadyForBatch?.Invoke(TakeBatch(_events.Count));
				_eventRepository.SaveData(_events);
			}
		}

		private void CheckForBatches()
		{
			if (_events.Count >= _setting.BatchSize)
			{
				EventsReadyForBatch?.Invoke(TakeBatch(_setting.BatchSize));
			}
		}

		private List<Event> TakeBatch(int batchSize)
		{
			int count = Math.Min(batchSize, _events.Count);
			List<Event> range = _events.GetRange(0, count);
			_events.RemoveRange(0, count);
			return range;
		}
	}
}
