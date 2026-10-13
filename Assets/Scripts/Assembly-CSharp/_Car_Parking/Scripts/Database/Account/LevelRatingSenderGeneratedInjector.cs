using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using CPM.Rating;
using VContainer;

namespace _Car_Parking.Scripts.Database.Account
{
	[Preserve]
	internal class LevelRatingSenderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LevelRatingSender levelRatingSender = new LevelRatingSender((RatingSender)resolver.ResolveOrParameter(typeof(RatingSender), "ratingController", parameters), (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters));
			Inject(levelRatingSender, resolver, parameters);
			return levelRatingSender;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
