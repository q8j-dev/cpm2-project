using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public sealed class AdsRewardsBackend : IAdsRewardsBackend
	{
		private const string UnclaimedUrl = "progress-service/v1/ads/rewards/unclaimed";

		private const string ClaimUrl = "progress-service/v1/ads/rewards/claim";

		private const string LimitsUrl = "progress-service/v1/ads/limits";

		private readonly BackendClient _backendClient;

		public AdsRewardsBackend(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result<List<UnclaimedAdReward>>> GetUnclaimed(CancellationToken ct = default(CancellationToken), string debugPlacement = "")
		{
			HttpRequest request = await BackendFactory.CreateRequest("progress-service/v1/ads/rewards/unclaimed", HttpVerb.Get, ct);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, ct);
			if (result.IsFail)
			{
				return Result<List<UnclaimedAdReward>>.Fail();
			}
			LogResponse("progress-service/v1/ads/rewards/unclaimed", result.Value);
			return Parse<List<UnclaimedAdReward>>(result.Value);
		}

		public async UniTask<Result<List<AdDailyLimit>>> GetLimits(CancellationToken ct = default(CancellationToken))
		{
			HttpRequest request = await BackendFactory.CreateRequest("progress-service/v1/ads/limits", HttpVerb.Get, ct);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, ct);
			if (result.IsFail)
			{
				return Result<List<AdDailyLimit>>.Fail();
			}
			LogResponse("progress-service/v1/ads/limits", result.Value);
			Result<AdDailyLimitsResponse> result2 = Parse<AdDailyLimitsResponse>(result.Value);
			return (result2.IsSuccess && result2.Value.Limits != null) ? Result<List<AdDailyLimit>>.Success(result2.Value.Limits) : Result<List<AdDailyLimit>>.Fail();
		}

		public UniTask<Result<List<AdRewardClaimResult>>> Claim(List<AdRewardClaimEntry> claims, CancellationToken ct = default(CancellationToken))
		{
			return SendClaimShapedAsync("progress-service/v1/ads/rewards/claim", claims, ct);
		}

		private async UniTask<Result<List<AdRewardClaimResult>>> SendClaimShapedAsync(string url, List<AdRewardClaimEntry> claims, CancellationToken ct)
		{
			string json = JsonConvert.SerializeObject(new AdRewardClaimRequest
			{
				Claims = claims
			});
			HttpRequest httpRequest = await BackendFactory.CreateRequest(url, HttpVerb.Post, ct);
			httpRequest.Body = Encoding.UTF8.GetBytes(json);
			httpRequest.BodyType = HttpBodyType.Json;
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(httpRequest, ct);
			if (result.IsFail)
			{
				return Result<List<AdRewardClaimResult>>.Fail();
			}
			LogResponse(url, result.Value);
			Result<AdRewardClaimResponse> result2 = Parse<AdRewardClaimResponse>(result.Value);
			return (result2.IsSuccess && result2.Value.Results != null) ? Result<List<AdRewardClaimResult>>.Success(result2.Value.Results) : Result<List<AdRewardClaimResult>>.Fail();
		}

		private static Result<T> Parse<T>(HttpResponse response) where T : class
		{
			if (response.StatusCode != 200 || response.Body == null)
			{
				return Result<T>.Fail();
			}
			try
			{
				T val = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(response.Body));
				return (val != null) ? Result<T>.Success(val) : Result<T>.Fail();
			}
			catch (JsonException)
			{
				return Result<T>.Fail();
			}
		}

		private static void LogResponse(string url, HttpResponse response)
		{
			if (response.Body != null)
			{
				Encoding.UTF8.GetString(response.Body);
			}
		}
	}
}
