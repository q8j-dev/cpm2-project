using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	internal class RpcRrRequestTestGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcRrRequestTest rpcRrRequestTest = new RpcRrRequestTest((ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters));
			Inject(rpcRrRequestTest, resolver, parameters);
			return rpcRrRequestTest;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
