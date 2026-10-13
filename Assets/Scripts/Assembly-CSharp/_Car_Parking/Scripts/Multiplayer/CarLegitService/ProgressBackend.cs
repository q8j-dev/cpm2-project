using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Multiplayer.CarLegitService
{
	[UnityEngine.Scripting.Preserve]
	public class ProgressBackend : IProgressBackend
	{
		[UnityEngine.Scripting.Preserve]
		private class OtherCarValidationItem
		{
			[JsonProperty("userId")]
			public string UserId;

			[JsonProperty("carUid")]
			public string CarUid;

			[JsonProperty("carId")]
			public int CarId;
		}

		private readonly BackendClient _backendClient;

		private static readonly int[] BackoffDelaysMs = new int[3] { 1000, 3000, 10000 };

		public ProgressBackend()
		{
		}

		[Inject]
		public ProgressBackend(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result<IReadOnlyList<bool>>> CheckOtherCarsLegit(IReadOnlyList<(string firebaseId, string carInstanceId, int carId)> requests, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (requests == null || requests.Count == 0)
			{
				return Result<IReadOnlyList<bool>>.Success(Array.Empty<bool>());
			}
			bool[] results = new bool[requests.Count];
			List<int> validIndices = new List<int>(requests.Count);
			List<OtherCarValidationItem> validItems = new List<OtherCarValidationItem>(requests.Count);
			for (int i = 0; i < requests.Count; i++)
			{
				(string, string, int) tuple = requests[i];
				if (string.IsNullOrEmpty(tuple.Item1) || string.IsNullOrEmpty(tuple.Item2))
				{
					results[i] = false;
					continue;
				}
				validIndices.Add(i);
				OtherCarValidationItem otherCarValidationItem = new OtherCarValidationItem();
				(otherCarValidationItem.UserId, otherCarValidationItem.CarUid, otherCarValidationItem.CarId) = tuple;
				validItems.Add(otherCarValidationItem);
			}
			if (validItems.Count == 0)
			{
				return Result<IReadOnlyList<bool>>.Success(results);
			}
			string payload = JsonConvert.SerializeObject(validItems);
			HttpRequest httpRequest = await BackendFactory.CreateRequest("progress-service/v1/cars/validate/batch", HttpVerb.Post);
			httpRequest.Body = Encoding.UTF8.GetBytes(payload);
			httpRequest.BodyType = HttpBodyType.Json;
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(httpRequest, cancellationToken);
			if (result.IsFail)
			{
				return Result<IReadOnlyList<bool>>.Fail();
			}
			string value = Encoding.UTF8.GetString(result.Value.Body);
			try
			{
				List<bool> list = JsonConvert.DeserializeObject<List<bool>>(value);
				if (list == null || list.Count != validItems.Count)
				{
					return Result<IReadOnlyList<bool>>.Fail();
				}
				for (int j = 0; j < validIndices.Count; j++)
				{
					results[validIndices[j]] = list[j];
				}
				return Result<IReadOnlyList<bool>>.Success(results);
			}
			catch (Exception)
			{
				return Result<IReadOnlyList<bool>>.Fail();
			}
		}

		public async UniTask<Result<bool>> CheckOtherCarLegit(string firebaseId, string carInstanceId, int carId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(carInstanceId) || string.IsNullOrEmpty(firebaseId))
			{
				return Result<bool>.Fail();
			}
			string url = $"progress-service/v1/cars/validate/transfer?userId={firebaseId}&carUid={carInstanceId}&carId={carId}";
			return await RequestBoolAsync(url, HttpVerb.Get, cancellationToken);
		}

		public async UniTask<Result<bool>> RegisterCar(string carInstanceId, int carId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(carInstanceId))
			{
				return Result<bool>.Fail();
			}
			string url = $"progress-service/v1/cars/validate?carUid={carInstanceId}&carId={carId}";
			return await RequestBoolAsync(url, HttpVerb.Get, cancellationToken);
		}

		public async UniTask RegisterCarWithBackoff(string carInstanceId, int carId, CancellationToken cancellationToken = default(CancellationToken))
		{
			int[] backoffDelaysMs = BackoffDelaysMs;
			for (int i = 0; i < backoffDelaysMs.Length; i++)
			{
				await UniTask.Delay(backoffDelaysMs[i], ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				Result<bool> result;
				try
				{
					result = await RegisterCar(carInstanceId, carId, cancellationToken);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception)
				{
					continue;
				}
				if (!result.IsFail)
				{
					break;
				}
			}
		}

		public async UniTask<Result<bool>> RemoveCar(string carInstanceId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(carInstanceId))
			{
				return Result<bool>.Fail();
			}
			string url = "progress-service/v1/cars/validate/remove?carUid=" + carInstanceId;
			return await RequestBoolAsync(url, HttpVerb.Post, cancellationToken);
		}

		private async UniTask<Result<bool>> RequestBoolAsync(string url, HttpVerb verb = HttpVerb.Get, CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpRequest request = await BackendFactory.CreateRequest(url, verb);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, cancellationToken);
			if (result.IsFail)
			{
				return Result<bool>.Fail();
			}
			return Result<bool>.Success(bool.Parse(Encoding.UTF8.GetString(result.Value.Body)));
		}
	}
}
