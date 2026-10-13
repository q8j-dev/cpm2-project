using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using VContainer;

namespace Rating.DailyTasks.Scripts
{
	[Preserve]
	internal class DailyServicesInitializerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DailyServicesInitializer dailyServicesInitializer = new DailyServicesInitializer((DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters), (IConnectionDataHandler)resolver.ResolveOrParameter(typeof(IConnectionDataHandler), "connectionDataHandler", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(dailyServicesInitializer, resolver, parameters);
			return dailyServicesInitializer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
