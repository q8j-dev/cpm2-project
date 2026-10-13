using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CPM.RacingRecords
{
	public interface ILeaderboardRecordsSource
	{
		event Action<string> Updated;

		UniTask<RacingRecordsCollection> GetOrFetch(string mapId, CancellationToken ct);

		bool TryGet(string mapId, out RacingRecordsCollection collection);
	}
}
