using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	internal class DriftAttackResultCounterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DriftAttackResultCounter driftAttackResultCounter = new DriftAttackResultCounter();
			Inject(driftAttackResultCounter, resolver, parameters);
			return driftAttackResultCounter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DriftAttackResultCounter obj = (DriftAttackResultCounter)instance;
			FreeDriveControl freeDriveControl = (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters);
			FreeTimeAttackController timeAttackController = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "timeAttackController", parameters);
			EventRestrictionService eventRestrictionService = (EventRestrictionService)resolver.ResolveOrParameter(typeof(EventRestrictionService), "eventRestrictionService", parameters);
			OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			obj.Construct(freeDriveControl, timeAttackController, eventRestrictionService, onlineCollectionsProvider, steamAchievementService);
		}
	}
}
