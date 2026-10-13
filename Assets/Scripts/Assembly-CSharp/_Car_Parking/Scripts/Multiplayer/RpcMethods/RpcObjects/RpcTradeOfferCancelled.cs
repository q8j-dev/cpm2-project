using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcTradeOfferCancelled : RpcObjectBase
	{
		private readonly CommonProvider _commonProvider;

		public override byte Code => 120;

		public static string GetRequestData(string tradeOfferID, bool isCancel, TradeOfferCancelReason cancelReason)
		{
			return string.Format("{0}{1}{2}{3}{4}", tradeOfferID, "?", isCancel ? "1" : "0", "?", (int)cancelReason);
		}

		[Inject]
		public RpcTradeOfferCancelled(CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (string.IsNullOrEmpty(b) || _commonProvider.TradeService == null)
			{
				return;
			}
			string[] array = b.Split("?");
			if (array != null && array.Length == 3)
			{
				string tradeId = array[0];
				if (int.TryParse(array[1], out var result) && int.TryParse(array[2], out var result2) && _commonProvider.TradeService.GetOffer(tradeId) != null)
				{
					_commonProvider.TradeService.CancelOffer(tradeId, (TradeOfferCancelReason)result2, notifyOtherPlayer: false, result == 1);
				}
			}
		}
	}
}
