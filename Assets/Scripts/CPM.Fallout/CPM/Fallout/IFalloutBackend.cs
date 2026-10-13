using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Backend;

namespace CPM.Fallout
{
	public interface IFalloutBackend
	{
		UniTask<Result<bool>> StartRace(string uniqueRaceId, CancellationToken ct = default(CancellationToken));

		UniTask<Result<FalloutFinish>> FinishRace(string uniqueRaceId, long timeSpentMilliseconds, int place = -1, CancellationToken ct = default(CancellationToken));

		UniTask<Result<FalloutLimits>> GetLimits(int activityType, CancellationToken ct = default(CancellationToken));

		UniTask<Result<FalloutClaim>> ClaimReward(string uniqueRaceId, CancellationToken ct = default(CancellationToken));
	}
}
