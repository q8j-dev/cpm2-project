using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcOpenCloseDoorAnimateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcOpenCloseDoorAnimate rpcOpenCloseDoorAnimate = new RpcOpenCloseDoorAnimate();
			Inject(rpcOpenCloseDoorAnimate, resolver, parameters);
			return rpcOpenCloseDoorAnimate;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
