using System.Threading;
using CPM.DailyTasks.Daily;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using VContainer;

namespace CPM.DailyTasks
{
	public sealed class DailyTasksFetchServiceRouter : IDailyTasksFetchService
	{
		private readonly FirebaseDailyTasksFetchService _firebase;

		private readonly BackendDailyTasksFetchService _backend;

		private readonly IRemoteConfigManager _remoteConfig;

		[Inject]
		public DailyTasksFetchServiceRouter(FirebaseDailyTasksFetchService firebase, BackendDailyTasksFetchService backend, IRemoteConfigManager remoteConfig)
		{
			_firebase = firebase;
			_backend = backend;
			_remoteConfig = remoteConfig;
		}

		public async UniTask<FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks>> GetAsync(CancellationToken token = default(CancellationToken))
		{
			return await (await ResolveAsync()).GetAsync(token);
		}

		private async UniTask<IDailyTasksFetchService> ResolveAsync()
		{
			if ((await _remoteConfig.GetConfigValuesStorage()).useNewRatingBackend)
			{
				return _backend;
			}
			return _firebase;
		}
	}
}
