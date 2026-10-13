using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using VContainer;

namespace _Car_Parking.Scripts.Backend
{
	public class CheckBackendService
	{
		private readonly BackendClient _backendClient;

		[Inject]
		public CheckBackendService(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result<List<string>>> GetAllBundlesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			new List<string>();
			HttpRequest request = HttpRequest.Request("check-service/v1/bundles/all", null, HttpBodyType.None, HttpVerb.Get);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, cancellationToken);
			if (result.IsFail)
			{
				return Result<List<string>>.Fail();
			}
			return Result<List<string>>.Success(JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(result.Value.Body)));
		}

		public async UniTask<Result<bool, string>> CheckHashAsync(int timeoutInSeconds, string hash, CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpRequest httpRequest = HttpRequest.Request("check-service/v1/hash/check", null, HttpBodyType.None, HttpVerb.Get, timeoutInSeconds);
			httpRequest = httpRequest.SetHeaders(new Dictionary<string, string> { ["X-Client-Hash"] = hash });
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(httpRequest, cancellationToken);
			if (result.IsFail)
			{
				return Result<bool, string>.Fail(result.Error.ToString());
			}
			return Result<bool, string>.Success(bool.Parse(Encoding.UTF8.GetString(result.Value.Body)));
		}
	}
}
