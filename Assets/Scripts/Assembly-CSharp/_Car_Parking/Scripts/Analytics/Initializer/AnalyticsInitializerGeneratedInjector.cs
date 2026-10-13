using System.Collections.Generic;
using CPM.Account;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Analytics.Initializer
{
	[Preserve]
	internal class AnalyticsInitializerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AnalyticsInitializer analyticsInitializer = new AnalyticsInitializer((PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (GeoGetterService)resolver.ResolveOrParameter(typeof(GeoGetterService), "geoGetterService", parameters), (ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters), (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters));
			Inject(analyticsInitializer, resolver, parameters);
			return analyticsInitializer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
