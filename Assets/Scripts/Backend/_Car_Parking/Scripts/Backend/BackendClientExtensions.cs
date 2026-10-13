using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace _Car_Parking.Scripts.Backend
{
	public static class BackendClientExtensions
	{
		private static readonly Func<byte[], Result<bool>> IgnoreBody = (byte[] _) => Result<bool>.Success(result: true);

		public static UniTask<Result<T>> PostAsync<T>(this BackendClient client, string url, object requestData, Func<byte[], Result<T>> parse, string logTag, CancellationToken ct = default(CancellationToken))
		{
			return client.SendAsync(url, HttpVerb.Post, requestData, parse, logTag, ct);
		}

		public static UniTask<Result<bool>> PostAsync(this BackendClient client, string url, object requestData, string logTag, CancellationToken ct = default(CancellationToken))
		{
			return client.SendAsync(url, HttpVerb.Post, requestData, IgnoreBody, logTag, ct);
		}

		public static async UniTask<Result<T>> SendAsync<T>(this BackendClient client, string url, HttpVerb verb, object requestData, Func<byte[], Result<T>> parse, string logTag, CancellationToken ct = default(CancellationToken))
		{
			HttpRequest httpRequest = await BackendFactory.CreateRequest(url, verb, ct);
			if (requestData != null)
			{
				httpRequest.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestData));
				httpRequest.BodyType = HttpBodyType.Json;
			}
			Result<HttpResponse, Exception> result = await client.HttpAsync(httpRequest, ct);
			if (result.IsFail)
			{
				return Result<T>.Fail();
			}
			LogResponse(logTag, url, result.Value);
			return parse(result.Value.Body);
		}

		private static void LogResponse(string logTag, string url, HttpResponse response)
		{
			if (response.Body != null)
			{
				Encoding.UTF8.GetString(response.Body);
			}
		}
	}
}
