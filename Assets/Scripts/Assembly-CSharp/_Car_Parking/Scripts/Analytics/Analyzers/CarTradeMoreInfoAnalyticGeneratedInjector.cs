using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class CarTradeMoreInfoAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarTradeMoreInfoAnalytic carTradeMoreInfoAnalytic = new CarTradeMoreInfoAnalytic((ITradeService)resolver.ResolveOrParameter(typeof(ITradeService), "tradeService", parameters), (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters));
			Inject(carTradeMoreInfoAnalytic, resolver, parameters);
			return carTradeMoreInfoAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
