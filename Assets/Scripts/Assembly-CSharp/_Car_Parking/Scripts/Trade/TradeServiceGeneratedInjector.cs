using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.Trade
{
	[Preserve]
	internal class TradeServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TradeService tradeService = new TradeService((IEnumerable<ITradeOfferHandler>)resolver.ResolveOrParameter(typeof(IEnumerable<ITradeOfferHandler>), "handlers", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (TradeAnalytic)resolver.ResolveOrParameter(typeof(TradeAnalytic), "tradeAnalytic", parameters), (IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "gameFactManager", parameters));
			Inject(tradeService, resolver, parameters);
			return tradeService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
