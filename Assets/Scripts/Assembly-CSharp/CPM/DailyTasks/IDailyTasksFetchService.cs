using System.Threading;
using CPM.DailyTasks.Daily;
using Cysharp.Threading.Tasks;
using Firebase.Functions;

namespace CPM.DailyTasks
{
	public interface IDailyTasksFetchService
	{
		UniTask<FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks>> GetAsync(CancellationToken token = default(CancellationToken));
	}
}
