using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods
{
	public abstract class RpcObjectBase
	{
		protected enum CodeType
		{
			CarEnteredCarSellTrigger = 42,
			CarExitCarSellTrigger = 43,
			SendWheelsState = 70,
			PoliceSiren = 101,
			PolicePenaltyRequest = 102,
			PolicePenaltyAnswer = 103,
			PoliceMute = 104,
			SitToCarRequest = 105,
			SitToCarAnswer = 106,
			TradeOfferCancelled = 120,
			TradeOfferPending = 121,
			OtherPlayerBlockedYou = 122,
			OtherPlayerMutedYou = 123,
			RequestPublishAvgPing = 150
		}

		protected const string SeparatorBase = "?";

		public abstract byte Code { get; }

		public virtual bool SendToInterestGroup => false;

		public RpcObjectBase()
		{
		}

		public abstract void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name);

		public virtual void ExecuteOnPerson(FreeDriveProvider freeDriveProvider, string b, string name)
		{
		}
	}
}
