using System.Collections.Generic;
using CPM.Common;
using CPM.DailyTasks;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.CPMRating.AchievementsService;
using _Car_Parking.Scripts.Rating;

namespace CPM.Rating
{
	[Preserve]
	internal class RatingSenderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RatingSender ratingSender = new RatingSender((AppStateHandler)resolver.ResolveOrParameter(typeof(AppStateHandler), "appStateHandler", parameters), (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters), (RatingDataCollection)resolver.ResolveOrParameter(typeof(RatingDataCollection), "ratingDataCollection", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (IAchievementsService)resolver.ResolveOrParameter(typeof(IAchievementsService), "achievementsService", parameters), (IRatingService)resolver.ResolveOrParameter(typeof(IRatingService), "ratingService", parameters), (AchievementPopupChannel)resolver.ResolveOrParameter(typeof(AchievementPopupChannel), "popupChannel", parameters));
			Inject(ratingSender, resolver, parameters);
			return ratingSender;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
