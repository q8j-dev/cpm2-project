using VContainer;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;

namespace _Car_Parking.Scripts.Scopes
{
	public static class VContainerRpcRrBinder
	{
		public static void Initialize(IContainerBuilder builder, RpcRrService rpcRrService)
		{
			builder.RegisterInstance(rpcRrService);
			builder.Register<RpcRrRequestCarDetails>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestCarTuningDataString>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestExchange>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestSendBuyCarOffer>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestSendExchangeCarOffer>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestShortCarTuningData>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestTest>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestBuyCarInCarTrigger>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
			builder.Register<RpcRrRequestBuyCarProfile>(Lifetime.Singleton).As<RpcRrRequestObjectBase>();
		}
	}
}
