using System;
using System.Threading;
using CPM.RacingRecords;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	public sealed class RallyRecordsBank : IDisposable, ILeaderboardRecordsSource
	{
		private static readonly TimeSpan ClearDelay = TimeSpan.FromMinutes(5.0);

		private readonly RacingRecordsCache _cache;

		public event Action<string> Updated
		{
			add
			{
				_cache.Updated += value;
			}
			remove
			{
				_cache.Updated -= value;
			}
		}

		public RallyRecordsBank(RacingRecordsCache cache)
		{
			_cache = cache;
			_cache.CancelClear();
		}

		public UniTask<RacingRecordsCollection> GetOrFetch(string mapId, CancellationToken ct)
		{
			return _cache.GetOrFetch(mapId, ct);
		}

		public void ForceRefresh(string mapId)
		{
			_cache.ForceRefresh(mapId);
		}

		public bool TryGet(string mapId, out RacingRecordsCollection collection)
		{
			return _cache.TryGet(mapId, out collection);
		}

		public void Dispose()
		{
			_cache.ScheduleClear(ClearDelay);
		}
	}
}
