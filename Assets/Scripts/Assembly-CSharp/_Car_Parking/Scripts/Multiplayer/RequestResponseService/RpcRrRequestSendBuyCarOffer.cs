using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrRequestSendBuyCarOffer : RpcRrRequestObjectBase
	{
		private const string Success = "1";

		private const string ErrorIsBusy = "0";

		private const string ErrorIsInTrade = "2";

		private const string ErrorEncrypt = "3";

		private const string SuccessAndBlocked = "4";

		public const float Timeout = 20f;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly CommonProvider _commonProvider;

		private readonly TradeServiceFactory _tradeServiceFactory;

		private readonly TradeAnalytic _tradeAnalytic;

		public override CodeType Code => CodeType.SendBuyCarOffer;

		public static string GetRequestData(GameObject car, int sellPrice, string tradeOfferID)
		{
			int num = car.GetComponent<VehicleData>().carInfo.TuningData.CalcShortTuningStates();
			return WalletCypher.Encode(sellPrice + "?" + num + "?" + tradeOfferID, "1");
		}

		public RpcRrRequestSendBuyCarOffer(FreeDriveProvider freeDriveProvider, CommonProvider commonProvider, TradeServiceFactory tradeServiceFactory, TradeAnalytic tradeAnalytic)
		{
			_freeDriveProvider = freeDriveProvider;
			_commonProvider = commonProvider;
			_tradeServiceFactory = tradeServiceFactory;
			_tradeAnalytic = tradeAnalytic;
		}

		public override async UniTask<string> ExecuteRequestAsync(Player sender, string requestData)
		{
			requestData = WalletCypher.DecodeWithOtherUID(requestData, sender.ActorNumber);
			string[] array = requestData.Split("?");
			if (array == null || array.Length != 3)
			{
				return "3";
			}
			if (!int.TryParse(array[0], out var carPrice))
			{
				return "3";
			}
			if (!_freeDriveProvider.MultiPlayer.GetCarByOwnerID(sender.ActorNumber))
			{
				return "0";
			}
			int tuningData = int.Parse(array[1]);
			string tradeOfferID = array[2];
			TradeItemCar tradeItemCar = await _tradeServiceFactory.CreateItemCarAsync(sender.ActorNumber, tuningData);
			TradeItemMoney tradeItemMoney = new TradeItemMoney(new AdvancedInt(carPrice));
			TradeOffer tradeOffer = _tradeServiceFactory.CreateOfferP2PSell(tradeItemCar, tradeItemMoney, sender.ActorNumber, PhotonNetwork.LocalPlayer.ActorNumber, tradeOfferID);
			_commonProvider.TradeService.AddOffer(tradeOffer);
			_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.RequestSent);
			if (SellExchangeFreeHelperCanSellOrExchange.PlayerInRaceNow())
			{
				_commonProvider.TradeService.CancelOffer(tradeOffer.TradeID, TradeOfferCancelReason.PlayerInRace, notifyOtherPlayer: false, isCanceled: false);
				return "0";
			}
			MultiPersonControl multiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(sender.ActorNumber);
			if (((bool)multiPersonControl && multiPersonControl.IsBlocked) || MultiPlayerHelper.IgnoreCarSellRequest)
			{
				return "4";
			}
			GlobalEventSystem.OnNewOffer?.Invoke(tradeOffer);
			return "1";
		}

		public override RpcRrResponseObjectBase CreateResponse(string executedRequestData)
		{
			return executedRequestData switch
			{
				"4" => new RpcRrResponseSendBuyCarOffer(RpcRrResponseObjectBase.CodeType.Success, RpcRrResponseSendBuyCarOffer.SendBuyCarOfferStatusCode.SuccessAndBlocked), 
				"1" => new RpcRrResponseSendBuyCarOffer(RpcRrResponseObjectBase.CodeType.Success, RpcRrResponseSendBuyCarOffer.SendBuyCarOfferStatusCode.Success), 
				"2" => new RpcRrResponseSendBuyCarOffer(RpcRrResponseObjectBase.CodeType.Success, RpcRrResponseSendBuyCarOffer.SendBuyCarOfferStatusCode.IsInTrade), 
				_ => new RpcRrResponseSendBuyCarOffer(RpcRrResponseObjectBase.CodeType.Success, RpcRrResponseSendBuyCarOffer.SendBuyCarOfferStatusCode.IsBusy), 
			};
		}
	}
}
