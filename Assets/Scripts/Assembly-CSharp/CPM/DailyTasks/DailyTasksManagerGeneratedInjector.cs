using System.Collections.Generic;
using CPM.Rating;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CPMRating.DailyTasks.Scripts;

namespace CPM.DailyTasks
{
	[Preserve]
	internal class DailyTasksManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DailyTasksManager dailyTasksManager = new DailyTasksManager((DailyTasksInfoContainer)resolver.ResolveOrParameter(typeof(DailyTasksInfoContainer), "dailyTasksInfoContainer", parameters), (IDailyTasksFetchService)resolver.ResolveOrParameter(typeof(IDailyTasksFetchService), "dailyTasksFetchService", parameters), (DailyTaskAnalytic)resolver.ResolveOrParameter(typeof(DailyTaskAnalytic), "dailyTaskAnalytic", parameters), (AchievementPopupChannel)resolver.ResolveOrParameter(typeof(AchievementPopupChannel), "popupChannel", parameters));
			Inject(dailyTasksManager, resolver, parameters);
			return dailyTasksManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
