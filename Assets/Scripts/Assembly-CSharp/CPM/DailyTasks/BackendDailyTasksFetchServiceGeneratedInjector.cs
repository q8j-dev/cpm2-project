using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace CPM.DailyTasks
{
	[Preserve]
	internal class BackendDailyTasksFetchServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BackendDailyTasksFetchService backendDailyTasksFetchService = new BackendDailyTasksFetchService((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(backendDailyTasksFetchService, resolver, parameters);
			return backendDailyTasksFetchService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
