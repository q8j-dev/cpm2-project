using CPM.Account;
using CPMEngine;
using UnityEngine;
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
	public class TradeOfferHandlerCarMoneyPrivate : TradeOfferHandlerCarMoneyBase
	{
		public override TradeOfferVisibleType OfferVisibleType => TradeOfferVisibleType.Private;

		protected override RpcRrRequestObjectBase.CodeType RpcRrRequestCodeType => RpcRrRequestObjectBase.CodeType.BuyCarInCarProfile;

		public TradeOfferHandlerCarMoneyPrivate(ManagerUI managerUI, IAccountController accountController, FreeDriveProvider freeDriveProvider, CurrencyManagerController currencyManagerController, ISlotService slotService, RpcRrService rpcRrService, IPoliceSystem policeSystem, ISteamAchievementService steamAchievementService, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IProgressBackend progressBackend, TradeAnalytic tradeAnalytic, ITradeTransactionBackend tradeTransactionBackend)
			: base(managerUI, accountController, freeDriveProvider, currencyManagerController, slotService, rpcRrService, policeSystem, steamAchievementService, virtualPurchaseAnalytic, progressBackend, tradeAnalytic, tradeTransactionBackend)
		{
		}

		protected override string GetCarTextTitle(TradeOffer tradeOffer)
		{
			string text = "";
			MultiPersonControl multiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(tradeOffer.OfferActorNr);
			if ((bool)multiPersonControl)
			{
				text = text + "\"" + multiPersonControl.ThisPlayerName + "\"";
			}
			text += new LocalizedString("FreeDriveControl.WantsToSellACar");
			GameObject carByOwnerID = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(tradeOffer.OfferActorNr);
			if ((bool)carByOwnerID && carByOwnerID.TryGetComponent<Powertrain>(out var component))
			{
				component.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
				text += $"({maxHp}hp/{maxTq} Nm)";
			}
			return text;
		}
	}
}
