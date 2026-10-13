using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcRequestPublishAvgPingGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcRequestPublishAvgPing rpcRequestPublishAvgPing = new RpcRequestPublishAvgPing();
			Inject(rpcRequestPublishAvgPing, resolver, parameters);
			return rpcRequestPublishAvgPing;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
