using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcChatMessageReceivedGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcChatMessageReceived rpcChatMessageReceived = new RpcChatMessageReceived();
			Inject(rpcChatMessageReceived, resolver, parameters);
			return rpcChatMessageReceived;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
