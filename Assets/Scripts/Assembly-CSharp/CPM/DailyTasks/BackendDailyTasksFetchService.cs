using System;
using System.Threading;
using CPM.DailyTasks.Daily;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace CPM.DailyTasks
{
	public sealed class BackendDailyTasksFetchService : IDailyTasksFetchService
	{
		private readonly BackendClient _backendClient;

		[Inject]
		public BackendDailyTasksFetchService(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks>> GetAsync(CancellationToken token = default(CancellationToken))
		{
			HttpRequest request = (await BackendFactory.CreateRequest("progress-service/v1/rating/daily", HttpVerb.Get, token)).Expect(409L);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, token);
			if (result.IsFail)
			{
				if (result.Error is UnityWebRequestException ex && ex.ResponseCode == 409)
				{
					return FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks>.Ok(null);
				}
				return FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks>.Fail(FunctionStatus.HttpError);
			}
			FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks> result2 = BackendEnvelopeParser.Parse<CPM.DailyTasks.Daily.DailyTasks>(result.Value.Body);
			_ = result2.IsSuccess;
			return result2;
		}
	}
}
