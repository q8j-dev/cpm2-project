using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutBackend : IFalloutBackend
	{
		private const string LogTag = "FalloutBackend";

		private const string StartUrl = "race-service/v1/fallout/start";

		private const string FinishUrl = "race-service/v1/fallout/finish";

		private const string LimitsUrl = "progress-service/v1/fallout/limits";

		private const string ClaimUrl = "progress-service/v1/fallout/claim";

		private readonly BackendClient _backendClient;

		public FalloutBackend(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result<bool>> StartRace(string uniqueRaceId, CancellationToken ct = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(uniqueRaceId))
			{
				return Result<bool>.Fail();
			}
			return await _backendClient.PostAsync("race-service/v1/fallout/start", new FalloutStartRequest
			{
				UniqueRaceId = uniqueRaceId
			}, FalloutResponse.ParseOk, "FalloutBackend", ct);
		}

		public async UniTask<Result<FalloutFinish>> FinishRace(string uniqueRaceId, long timeSpentMilliseconds, int place = -1, CancellationToken ct = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(uniqueRaceId))
			{
				return Result<FalloutFinish>.Fail();
			}
			return await PostAsync<FalloutFinish>("race-service/v1/fallout/finish", new FalloutFinishRequest
			{
				UniqueRaceId = uniqueRaceId,
				TimeSpend = timeSpentMilliseconds,
				Place = place
			}, ct);
		}

		public async UniTask<Result<FalloutLimits>> GetLimits(int activityType, CancellationToken ct = default(CancellationToken))
		{
			return await PostAsync<FalloutLimits>("progress-service/v1/fallout/limits", new FalloutLimitsRequest
			{
				RaceType = activityType
			}, ct);
		}

		public async UniTask<Result<FalloutClaim>> ClaimReward(string uniqueRaceId, CancellationToken ct = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(uniqueRaceId))
			{
				return Result<FalloutClaim>.Fail();
			}
			return await PostAsync<FalloutClaim>("progress-service/v1/fallout/claim", new FalloutClaimRequest
			{
				UniqueRaceId = uniqueRaceId
			}, ct);
		}

		private UniTask<Result<T>> PostAsync<T>(string url, object requestData, CancellationToken ct) where T : class
		{
			return _backendClient.PostAsync(url, requestData, FalloutResponse.Parse<T>, "FalloutBackend", ct);
		}
	}
}
