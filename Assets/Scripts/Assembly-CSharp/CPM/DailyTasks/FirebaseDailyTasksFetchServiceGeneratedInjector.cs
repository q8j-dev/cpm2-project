using System.Collections.Generic;
using VContainer;

namespace CPM.DailyTasks
{
	[Preserve]
	internal class FirebaseDailyTasksFetchServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseDailyTasksFetchService firebaseDailyTasksFetchService = new FirebaseDailyTasksFetchService();
			Inject(firebaseDailyTasksFetchService, resolver, parameters);
			return firebaseDailyTasksFetchService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
