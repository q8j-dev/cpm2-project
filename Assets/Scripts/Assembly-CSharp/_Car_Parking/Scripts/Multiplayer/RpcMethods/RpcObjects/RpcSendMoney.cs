using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcSendMoney : RpcObjectBase
	{
		private FreeDriveProvider _freeDriveProvider;

		public override byte Code => 29;

		public RpcSendMoney()
		{
		}

		[Inject]
		public RpcSendMoney(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (MultiplayerPosControlExtensions.VDriverPersonPhotonId(b))
			{
				string[] array = name.Split("?"[0]);
				ObscuredInt moneyReceived = Mathf.Clamp(int.Parse(array[0]), 0, 500000);
				string offerId = array[1];
				TradeOfferState tradeOfferState = (TradeOfferState)int.Parse(array[2]);
				string playerId = array[3];
				switch (tradeOfferState)
				{
				case TradeOfferState.Enabled:
					_freeDriveProvider.MoneySendService.NewOfferReceived(playerId, moneyReceived, offerId);
					break;
				case TradeOfferState.Cancelled:
					_freeDriveProvider.MoneySendService.OfferCanceled(offerId);
					break;
				case TradeOfferState.Processing:
					_freeDriveProvider.MoneySendService.OfferConfirmed(offerId);
					break;
				case TradeOfferState.Declined:
					_freeDriveProvider.MoneySendService.OfferDeclined(offerId);
					break;
				case TradeOfferState.Pending:
					_freeDriveProvider.MoneySendService.OfferPending(offerId);
					break;
				case TradeOfferState.Completed:
					_freeDriveProvider.MoneySendService.OnOfferCompleted(offerId);
					break;
				}
			}
		}
	}
}
