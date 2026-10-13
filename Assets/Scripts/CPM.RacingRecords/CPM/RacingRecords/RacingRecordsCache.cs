using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace CPM.RacingRecords
{
	[Preserve]
	public sealed class RacingRecordsCache : ILeaderboardRecordsSource
	{
		private sealed class Entry
		{
			public readonly RacingRecordsCollection Collection = new RacingRecordsCollection();

			public double FetchedAt;

			public bool HasData;

			public bool InFlight;
		}

		private readonly Dictionary<string, Entry> _byMap = new Dictionary<string, Entry>();

		private readonly IRacingRecordsBackend _backend;

		private readonly string _mode;

		private readonly int _count;

		private readonly double _ttlSeconds;

		private readonly Func<double> _now;

		private CancellationTokenSource _clearCts;

		public event Action<string> Updated;

		public RacingRecordsCache(IRacingRecordsBackend backend, string mode, int count, double ttlSeconds, Func<double> now)
		{
			_backend = backend;
			_mode = mode;
			_count = count;
			_ttlSeconds = ttlSeconds;
			_now = now;
		}

		public async UniTask<RacingRecordsCollection> GetOrFetch(string mapId, CancellationToken ct)
		{
			if (string.IsNullOrEmpty(mapId))
			{
				return new RacingRecordsCollection();
			}
			Entry e = GetOrCreate(mapId);
			if (e.HasData && _now() - e.FetchedAt <= _ttlSeconds)
			{
				return e.Collection;
			}
			await FetchInto(mapId, e, ct);
			return e.Collection;
		}

		public void ForceRefresh(string mapId)
		{
			if (!string.IsNullOrEmpty(mapId))
			{
				ForceRefreshAsync(mapId, GetOrCreate(mapId)).Forget();
			}
		}

		private async UniTaskVoid ForceRefreshAsync(string mapId, Entry e)
		{
			while (e.InFlight)
			{
				await UniTask.Yield();
			}
			if (await FetchInto(mapId, e, CancellationToken.None))
			{
				Updated?.Invoke(mapId);
			}
		}

		private async UniTask<bool> FetchInto(string mapId, Entry e, CancellationToken ct)
		{
			e.InFlight = true;
			try
			{
				Result<IReadOnlyList<RacingRecord>> result = await _backend.GetRecords(_mode, mapId, _count, 0, ct);
				if (result.IsSuccess)
				{
					e.Collection.SetPage(result.Value);
					e.FetchedAt = _now();
					e.HasData = true;
					return true;
				}
				return false;
			}
			finally
			{
				e.InFlight = false;
			}
		}

		public bool TryGet(string mapId, out RacingRecordsCollection collection)
		{
			if (!string.IsNullOrEmpty(mapId) && _byMap.TryGetValue(mapId, out var value) && value.HasData)
			{
				collection = value.Collection;
				return true;
			}
			collection = null;
			return false;
		}

		public void ScheduleClear(TimeSpan delay)
		{
			CancelClear();
			_clearCts = new CancellationTokenSource();
			ClearAfterAsync(delay, _clearCts.Token).Forget();
		}

		public void CancelClear()
		{
			if (_clearCts != null)
			{
				_clearCts.Cancel();
				_clearCts.Dispose();
				_clearCts = null;
			}
		}

		public void ClearAll()
		{
			_byMap.Clear();
		}

		private async UniTaskVoid ClearAfterAsync(TimeSpan delay, CancellationToken token)
		{
			if (!(await UniTask.Delay(delay, DelayType.Realtime, PlayerLoopTiming.Update, token).SuppressCancellationThrow()))
			{
				ClearAll();
			}
		}

		private Entry GetOrCreate(string mapId)
		{
			if (!_byMap.TryGetValue(mapId, out var value))
			{
				value = new Entry();
				_byMap[mapId] = value;
			}
			return value;
		}
	}
}
