using System.Collections.Generic;
using System.Threading;
using CPM.Ghost.Core;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Backend;

namespace CPM.RacingRecords
{
	public interface IRacingRecordsBackend
	{
		UniTask<Result<IReadOnlyList<RacingRecord>>> GetRecords(string mode, string mapId, int count, int page, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> StartRace(string mode, string raceUid, string raceKey, string roomId, int carId, int carClass, bool rain, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> SubmitResult(string mode, string raceUid, string raceKey, string nickname, int carId, int carClass, long score, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> SubmitReplay(string mode, string raceUid, string raceKey, string roomId, long score, GhostRecording recording, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<GhostRecording>> GetReplay(string mode, string userId, string raceKey, CancellationToken cancellationToken = default(CancellationToken));
	}
}
