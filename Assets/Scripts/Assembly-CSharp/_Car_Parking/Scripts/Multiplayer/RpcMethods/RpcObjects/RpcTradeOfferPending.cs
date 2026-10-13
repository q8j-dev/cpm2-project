using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcTradeOfferPending : RpcObjectBase
	{
		private readonly CommonProvider _commonProvider;

		public override byte Code => 121;

		[Inject]
		public RpcTradeOfferPending(CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (!string.IsNullOrEmpty(b) && _commonProvider.TradeService != null)
			{
				TradeOffer offer = _commonProvider.TradeService.GetOffer(b);
				if (offer != null && offer.State == TradeOfferState.Enabled)
				{
					offer.State = TradeOfferState.Pending;
				}
			}
		}
	}
}
