using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _Car_Parking.Scripts.Backend.Session
{
	public class SessionBackend
	{
		private readonly BackendClient _backendClient;

		[Inject]
		public SessionBackend(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result> StartSessionAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpRequest request = await BackendFactory.CreateRequest("check-service/v1/session/start", HttpVerb.Post);
			return (await _backendClient.HttpAsync(request, cancellationToken)).IsSuccess ? Result.Success() : Result.Fail();
		}

		public async UniTask<Result<bool>> ExtendSessionAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpRequest request = await BackendFactory.CreateRequest("check-service/v1/session/extend", HttpVerb.Post);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, cancellationToken);
			if (result.IsFail)
			{
				return Result<bool>.Fail();
			}
			return Result<bool>.Success(bool.Parse(Encoding.UTF8.GetString(result.Value.Body)));
		}

		public async UniTask<Result<bool>> EndSessionAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpRequest request = await BackendFactory.CreateRequest("check-service/v1/session/end", HttpVerb.Post);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, cancellationToken);
			if (result.IsFail)
			{
				return Result<bool>.Fail();
			}
			return Result<bool>.Success(bool.Parse(Encoding.UTF8.GetString(result.Value.Body)));
		}
	}
}
