using VContainer;
using _Car_Parking.Scripts.Multiplayer.RpcMethods;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;

namespace _Car_Parking.Scripts.Scopes
{
	public static class VContainerRpcCollectionBinder
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<RpcCollection>(Lifetime.Singleton);
			builder.Register<RpcBrake>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcCarEnteredCarSellTrigger>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcCarExitCarSellTrigger>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcChatMessageReceived>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcGetOutCar>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcInvitationReceived>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcOpenCloseDoorAnimate>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcOpenCloseDoorCar>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcOpenCloseHood>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcOpenCloseRoof>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcOpenCloseTrunk>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcOtherPlayerBlockedYou>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcOtherPlayerMutedYou>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcPoliceMute>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcPolicePenaltyAnswer>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcPolicePenaltyRequest>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcPoliceSiren>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcRequestPublishAvgPing>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcSendDataOpenDoors>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcSendMoney>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcSendWheelsState>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcSitIntoCar>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcSitToCarAnswer>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcSitToCarRequest>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcSpectator>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcTradeOfferCancelled>(Lifetime.Singleton).As<RpcObjectBase>();
			builder.Register<RpcTradeOfferPending>(Lifetime.Singleton).As<RpcObjectBase>();
		}
	}
}
