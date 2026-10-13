using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Multiplayer.TradeTransaction
{
	[UnityEngine.Scripting.Preserve]
	public class TradeTransactionBackend : ITradeTransactionBackend
	{
		[UnityEngine.Scripting.Preserve]
		private class CreateTransactionResponse
		{
			[JsonProperty("status")]
			public string Status = default;

			[JsonProperty("transactionId")]
			public string TransactionId = default;
		}

		[UnityEngine.Scripting.Preserve]
		private class ConfirmTransactionResponse
		{
			[JsonProperty("status")]
			public string Status = default;
		}

		private const string CreateUrl = "check-service/v1/p2p/create";

		private const string ConfirmUrl = "check-service/v1/p2p/accept";

		private const string StatusCreated = "CREATED";

		private const string StatusComplete = "COMPLETE";

		private const string StatusFailed = "FAILED";

		private readonly BackendClient _backendClient;

		public TradeTransactionBackend()
		{
		}

		[Inject]
		public TradeTransactionBackend(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result<string>> CreateTransaction(TradeCreateRequest req, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (req == null || string.IsNullOrEmpty(req.Type))
			{
				return Result<string>.Fail();
			}
			string body = JsonConvert.SerializeObject(req);
			HttpRequest httpRequest = await BackendFactory.CreateRequest("check-service/v1/p2p/create", HttpVerb.Post, cancellationToken);
			httpRequest.Body = Encoding.UTF8.GetBytes(body);
			httpRequest.BodyType = HttpBodyType.Json;
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(httpRequest, cancellationToken);
			if (result.IsFail)
			{
				return Result<string>.Fail();
			}
			try
			{
				CreateTransactionResponse createTransactionResponse = JsonConvert.DeserializeObject<CreateTransactionResponse>(Encoding.UTF8.GetString(result.Value.Body));
				if (createTransactionResponse == null || string.IsNullOrEmpty(createTransactionResponse.TransactionId))
				{
					return Result<string>.Fail();
				}
				if (createTransactionResponse.Status != "CREATED")
				{
					return Result<string>.Fail();
				}
				return Result<string>.Success(createTransactionResponse.TransactionId);
			}
			catch (Exception)
			{
				return Result<string>.Fail();
			}
		}

		public async UniTask<Result<bool>> ConfirmTransaction(TradeConfirmRequest req, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (req == null || string.IsNullOrEmpty(req.Type) || string.IsNullOrEmpty(req.TransactionId))
			{
				return Result<bool>.Fail();
			}
			_ = req.TransactionId;
			string body = JsonConvert.SerializeObject(req);
			HttpRequest httpRequest = await BackendFactory.CreateRequest("check-service/v1/p2p/accept", HttpVerb.Post, cancellationToken);
			httpRequest.Body = Encoding.UTF8.GetBytes(body);
			httpRequest.BodyType = HttpBodyType.Json;
			httpRequest.TimeoutInSeconds = 30;
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(httpRequest, cancellationToken);
			if (result.IsFail)
			{
				return Result<bool>.Fail();
			}
			try
			{
				ConfirmTransactionResponse confirmTransactionResponse = JsonConvert.DeserializeObject<ConfirmTransactionResponse>(Encoding.UTF8.GetString(result.Value.Body));
				if (confirmTransactionResponse == null)
				{
					return Result<bool>.Fail();
				}
				string status = confirmTransactionResponse.Status;
				if (!(status == "COMPLETE"))
				{
					if (status == "FAILED")
					{
						return Result<bool>.Success(result: false);
					}
					return Result<bool>.Fail();
				}
				return Result<bool>.Success(result: true);
			}
			catch (Exception)
			{
				return Result<bool>.Fail();
			}
		}
	}
}
