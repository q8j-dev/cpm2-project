using System;
using System.Collections.Generic;
using System.Linq;
using _Car_Parking.Scripts.Analytics.Root.Data;

namespace _Car_Parking.Scripts.Analytics.Root.Batching
{
	public class BatchTracker : IDisposable
	{
		private const string Key = "CPM_A_batches_1";

		private readonly AnalyticCoreSetting _setting;

		private readonly IBatchSource _source;

		private readonly AnalyticsRepository<List<Batch>> _batchRepository = new AnalyticsRepository<List<Batch>>("CPM_A_batches_1");

		private readonly List<Batch> _batches;

		public Batch FirstBatch => _batches.FirstOrDefault();

		public BatchTracker(IBatchSource source, AnalyticCoreSetting setting)
		{
			_source = source;
			_setting = setting;
			_source.EventsReadyForBatch += ReceiveBatchContent;
			List<Batch> list = _batchRepository.LoadData();
			_batches = list ?? new List<Batch>();
		}

		private void ReceiveBatchContent(List<Event> content)
		{
			_batches.Add(new Batch
			{
				Events = content
			});
			_batchRepository.SaveData(_batches);
		}

		public void RemoveBatch(string batchUuid)
		{
			for (int i = 0; i < _batches.Count; i++)
			{
				if (_batches[i].Uuid.Equals(batchUuid))
				{
					_batches.RemoveAt(i);
				}
			}
			_batchRepository.SaveData(_batches);
		}

		public void Dispose()
		{
			_source.EventsReadyForBatch -= ReceiveBatchContent;
		}
	}
}
