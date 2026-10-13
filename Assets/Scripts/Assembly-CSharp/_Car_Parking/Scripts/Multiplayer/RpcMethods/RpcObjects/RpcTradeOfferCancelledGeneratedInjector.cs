using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcTradeOfferCancelledGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcTradeOfferCancelled rpcTradeOfferCancelled = new RpcTradeOfferCancelled((CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters));
			Inject(rpcTradeOfferCancelled, resolver, parameters);
			return rpcTradeOfferCancelled;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
