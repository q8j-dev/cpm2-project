using Photon.Realtime;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrRequestBuyCarInCarTrigger : RpcRrRequestBuyCarBase
	{
		public override CodeType Code => CodeType.BuyCarInCarTrigger;

		public RpcRrRequestBuyCarInCarTrigger(FreeDriveProvider freeDriveProvider, ITradeService tradeService, ISteamAchievementService steamAchievementService, CommonProvider commonProvider, ManagerUI managerUI, ISlotService slotService, CurrencyManagerController currencyManagerController, IDatabaseCarsService databaseCarsService, LockCarButtonController lockCarButtonController, TradeAnalytic tradeAnalytic, VirtualPurchaseAnalytic virtualPurchaseAnalytic, ITradeTransactionBackend tradeTransactionBackend)
			: base(freeDriveProvider, tradeService, steamAchievementService, managerUI, slotService, currencyManagerController, databaseCarsService, lockCarButtonController, commonProvider, tradeAnalytic, virtualPurchaseAnalytic, tradeTransactionBackend)
		{
		}

		protected override string ExtraCheckCanSellCar(TradeOffer tradeOffer, Player sender)
		{
			if (!_freeDriveProvider.FirstCarMultiPlayer)
			{
				return "3";
			}
			if (_freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>().СurrentSellCarTrigger == null)
			{
				return "3";
			}
			return "";
		}

		protected override void ExtraActionAfterSellCar(TradeOffer tradeOffer)
		{
			if ((bool)_freeDriveProvider.FirstCarMultiPlayer && _commonProvider.SellExchangeFreeService != null)
			{
				VehicleData component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
				if (component.СurrentSellCarTrigger != null)
				{
					_commonProvider.SellExchangeFreeService.SellCarExit(component.СurrentSellCarTrigger.Index);
				}
			}
		}
	}
}
