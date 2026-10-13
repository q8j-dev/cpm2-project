using System;
using Photon.Pun;
using VContainer.Unity;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Scopes
{
	public class FreeDriveStartup : IStartable, IDisposable
	{
		private readonly ISellExchangeFreeService _sellExchangeFreeService;

		private readonly ITradeService _tradeService;

		private readonly TradeServiceFactory _tradeServiceFactory;

		private readonly ChatExtraDataController _chatExtraDataController;

		private readonly MoneySendService _moneySendService;

		private readonly AddFriendService _addFriendService;

		private readonly CommonProvider _commonProvider;

		private readonly VfxPoolProvider _vfxPoolProvider;

		private readonly OnlineCollectionsProvider _onlineCollectionsProvider;

		private readonly FreeDriveProvider _freeDriveProvider;

		public FreeDriveStartup(ISellExchangeFreeService sellExchangeFreeService, ITradeService tradeService, TradeServiceFactory tradeServiceFactory, ChatExtraDataController chatExtraDataController, MoneySendService moneySendService, AddFriendService addFriendService, CommonProvider commonProvider, VfxPoolProvider vfxPoolProvider, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider)
		{
			_sellExchangeFreeService = sellExchangeFreeService;
			_tradeService = tradeService;
			_tradeServiceFactory = tradeServiceFactory;
			_chatExtraDataController = chatExtraDataController;
			_moneySendService = moneySendService;
			_addFriendService = addFriendService;
			_commonProvider = commonProvider;
			_vfxPoolProvider = vfxPoolProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_freeDriveProvider = freeDriveProvider;
		}

		public void Start()
		{
			_commonProvider.SellExchangeFreeService = _sellExchangeFreeService;
			_commonProvider.TradeService = _tradeService;
			_commonProvider.TradeServiceFactory = _tradeServiceFactory;
			_commonProvider.ChatExtraDataController = _chatExtraDataController;
			_vfxPoolProvider.Initialize();
		}

		public void Dispose()
		{
			V.TryClearV();
			PhotonNetwork.PrefabPool.ClearResources(destroyDistanceCheckers: true);
			_onlineCollectionsProvider.ResetAllCollections();
			_freeDriveProvider.TryClearProvider();
			_moneySendService.ClearOffers();
			_addFriendService.ClearOffers();
			_vfxPoolProvider.Dispose();
			_commonProvider.SellExchangeFreeService = null;
			_commonProvider.TradeService = null;
			_commonProvider.TradeServiceFactory = null;
			_commonProvider.ChatExtraDataController = null;
		}
	}
}
