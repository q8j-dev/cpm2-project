using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Other.Cypher;

namespace _Car_Parking.Scripts.Rating
{
	public class BackendRatingService : IRatingService
	{
		private readonly BackendClient _backendClient;

		[Inject]
		public BackendRatingService(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<FunctionCallResult<SetRatingResponse>> SetUserRating(string json, string accountID, CancellationToken token = default(CancellationToken))
		{
			string text = WalletCypher.Encode(json, null);
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogError("[BackendRatingService.SetUserRating] WalletCypher.Encode returned null/empty (likely empty AccountID); aborting request.");
				return FunctionCallResult<SetRatingResponse>.Fail(FunctionStatus.LocalEncryptionFailed);
			}
			string s = JsonConvert.SerializeObject(new
			{
				data = text
			});
			byte[] bodyBytes = Encoding.UTF8.GetBytes(s);
			HttpRequest httpRequest = await BackendFactory.CreateRequest("progress-service/v1/rating/update", HttpVerb.Post, token);
			httpRequest.Body = bodyBytes;
			httpRequest.BodyType = HttpBodyType.Json;
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(httpRequest, token);
			if (result.IsFail)
			{
				return FunctionCallResult<SetRatingResponse>.Fail(FunctionStatus.HttpError);
			}
			FunctionCallResult<SetRatingResponse> result2 = BackendEnvelopeParser.Parse<SetRatingResponse>(result.Value.Body);
			_ = result2.IsSuccess;
			return result2;
		}

		public async UniTask<FunctionCallResult<RatingResultAll>> GetUserRating(string accountID, CancellationToken token = default(CancellationToken))
		{
			HttpRequest httpRequest = await BackendFactory.CreateRequest("progress-service/v1/rating", HttpVerb.Get, token);
			if (!string.IsNullOrEmpty(accountID))
			{
				httpRequest.QueryParameters = new Dictionary<string, string> { ["uid"] = accountID };
			}
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(httpRequest, token);
			if (result.IsFail)
			{
				return FunctionCallResult<RatingResultAll>.Fail(FunctionStatus.HttpError);
			}
			FunctionCallResult<RatingResultAll> result2 = BackendEnvelopeParser.Parse<RatingResultAll>(result.Value.Body);
			_ = result2.IsSuccess;
			return result2;
		}
	}
}
