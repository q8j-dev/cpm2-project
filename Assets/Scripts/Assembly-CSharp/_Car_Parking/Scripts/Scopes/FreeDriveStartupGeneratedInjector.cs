using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class FreeDriveStartupGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeDriveStartup freeDriveStartup = new FreeDriveStartup((ISellExchangeFreeService)resolver.ResolveOrParameter(typeof(ISellExchangeFreeService), "sellExchangeFreeService", parameters), (ITradeService)resolver.ResolveOrParameter(typeof(ITradeService), "tradeService", parameters), (TradeServiceFactory)resolver.ResolveOrParameter(typeof(TradeServiceFactory), "tradeServiceFactory", parameters), (ChatExtraDataController)resolver.ResolveOrParameter(typeof(ChatExtraDataController), "chatExtraDataController", parameters), (MoneySendService)resolver.ResolveOrParameter(typeof(MoneySendService), "moneySendService", parameters), (AddFriendService)resolver.ResolveOrParameter(typeof(AddFriendService), "addFriendService", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (VfxPoolProvider)resolver.ResolveOrParameter(typeof(VfxPoolProvider), "vfxPoolProvider", parameters), (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters));
			Inject(freeDriveStartup, resolver, parameters);
			return freeDriveStartup;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
