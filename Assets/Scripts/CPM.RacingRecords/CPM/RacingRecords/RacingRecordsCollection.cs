using System.Collections.Generic;
using UnityEngine.Scripting;

namespace CPM.RacingRecords
{
	[Preserve]
	public sealed class RacingRecordsCollection
	{
		private readonly List<RacingRecord> _items = new List<RacingRecord>();

		public IReadOnlyList<RacingRecord> Items => _items;

		public void SetPage(IReadOnlyList<RacingRecord> records)
		{
			_items.Clear();
			if (records != null)
			{
				_items.AddRange(records);
			}
		}

		public void Clear()
		{
			_items.Clear();
		}
	}
}
