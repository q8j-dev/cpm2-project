using System.Collections.Generic;
using CPM.Connection;
using CPM.DailyTasks;
using VContainer;
using _Car_Parking.Scripts.DailyRewards;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.Racing_Scripts;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	[Preserve]
	internal class GameDataRepositoryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameDataRepository gameDataRepository = new GameDataRepository((IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters), (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (IDailyRewardsServer)resolver.ResolveOrParameter(typeof(IDailyRewardsServer), "dailyRewardsServer", parameters), (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (RallyData)resolver.ResolveOrParameter(typeof(RallyData), "rallyData", parameters), (IPlayerProfileService)resolver.ResolveOrParameter(typeof(IPlayerProfileService), "playerProfileService", parameters), (IRacingAdRewardReclaimService)resolver.ResolveOrParameter(typeof(IRacingAdRewardReclaimService), "racingAdRewardReclaim", parameters), (IAdsDailyLimitsService)resolver.ResolveOrParameter(typeof(IAdsDailyLimitsService), "adsDailyLimits", parameters));
			Inject(gameDataRepository, resolver, parameters);
			return gameDataRepository;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
