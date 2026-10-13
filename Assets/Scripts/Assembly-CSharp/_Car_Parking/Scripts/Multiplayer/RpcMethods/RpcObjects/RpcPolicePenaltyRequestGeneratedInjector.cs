using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcPolicePenaltyRequestGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcPolicePenaltyRequest rpcPolicePenaltyRequest = new RpcPolicePenaltyRequest();
			Inject(rpcPolicePenaltyRequest, resolver, parameters);
			return rpcPolicePenaltyRequest;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
