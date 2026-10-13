using Cysharp.Threading.Tasks;
using Photon.Realtime;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	public abstract class RpcRrRequestObjectBase
	{
		public enum CodeType
		{
			Test = 0,
			CarDetails = 1,
			FirstCarMultiplayerCarInstanceId = 2,
			BuyCar = 3,
			SendBuyCarOffer = 4,
			SendExchangeCarOffer = 5,
			BuyCarInCarTrigger = 6,
			CarTuningDataString = 7,
			BuyCarInCarProfile = 8,
			Exchange = 9,
			CarTuningDataInt = 10
		}

		protected const string Separator = "?";

		public abstract CodeType Code { get; }

		public RpcRrRequestObjectBase()
		{
		}

		public abstract UniTask<string> ExecuteRequestAsync(Player sender, string requestData);

		public abstract RpcRrResponseObjectBase CreateResponse(string executedRequestData);
	}
}
