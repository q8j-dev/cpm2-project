using CPM.Account;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Trade
{
	[Preserve]
	public class TradeOfferHandlerCarMoneyPublic : TradeOfferHandlerCarMoneyBase
	{
		public override TradeOfferVisibleType OfferVisibleType => TradeOfferVisibleType.Public;

		protected override RpcRrRequestObjectBase.CodeType RpcRrRequestCodeType => RpcRrRequestObjectBase.CodeType.BuyCarInCarTrigger;

		public TradeOfferHandlerCarMoneyPublic(ManagerUI managerUI, IAccountController accountController, FreeDriveProvider freeDriveProvider, CurrencyManagerController currencyManagerController, ISlotService slotService, RpcRrService rpcRrService, IPoliceSystem policeSystem, ISteamAchievementService steamAchievementService, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IProgressBackend progressBackend, TradeAnalytic tradeAnalytic, ITradeTransactionBackend tradeTransactionBackend)
			: base(managerUI, accountController, freeDriveProvider, currencyManagerController, slotService, rpcRrService, policeSystem, steamAchievementService, virtualPurchaseAnalytic, progressBackend, tradeAnalytic, tradeTransactionBackend)
		{
		}

		protected override string GetCarTextTitle(TradeOffer tradeOffer)
		{
			return new LocalizedString("FreeDriveControl.BuyThisCar");
		}
	}
}
