using System.Threading;
using CPM.DailyTasks.Daily;
using Cysharp.Threading.Tasks;
using Firebase.Functions;

namespace CPM.DailyTasks
{
	public sealed class FirebaseDailyTasksFetchService : IDailyTasksFetchService
	{
		public const string GetDailyTaskFn = "GetDailyTaskCall24_1";

		private readonly FirebaseWrappedFunctionFactory _factory = new FirebaseWrappedFunctionFactory();

		public async UniTask<FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks>> GetAsync(CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			return await _factory.Create("GetDailyTaskCall24_1").CallAsyncResult<CPM.DailyTasks.Daily.DailyTasks>("");
		}
	}
}
