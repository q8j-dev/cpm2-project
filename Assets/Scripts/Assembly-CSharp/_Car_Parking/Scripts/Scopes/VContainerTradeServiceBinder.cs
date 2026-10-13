using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Scopes
{
	public static class VContainerTradeServiceBinder
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<CarTradeMoreInfoAnalytic>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<TradeService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<P2PAntiSpamService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<TradeServiceFactory>(Lifetime.Singleton);
			builder.Register<TradeOfferHandlerCarCar>(Lifetime.Singleton).As<ITradeOfferHandler>();
			builder.Register<TradeOfferHandlerCarMoneyPrivate>(Lifetime.Singleton).As<ITradeOfferHandler>();
			builder.Register<TradeOfferHandlerCarMoneyPublic>(Lifetime.Singleton).As<ITradeOfferHandler>();
		}
	}
}
