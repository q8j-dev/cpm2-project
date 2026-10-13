using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;

namespace CPM.DailyTasks
{
	[Preserve]
	internal class DailyTasksFetchServiceRouterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DailyTasksFetchServiceRouter dailyTasksFetchServiceRouter = new DailyTasksFetchServiceRouter((FirebaseDailyTasksFetchService)resolver.ResolveOrParameter(typeof(FirebaseDailyTasksFetchService), "firebase", parameters), (BackendDailyTasksFetchService)resolver.ResolveOrParameter(typeof(BackendDailyTasksFetchService), "backend", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfig", parameters));
			Inject(dailyTasksFetchServiceRouter, resolver, parameters);
			return dailyTasksFetchServiceRouter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
