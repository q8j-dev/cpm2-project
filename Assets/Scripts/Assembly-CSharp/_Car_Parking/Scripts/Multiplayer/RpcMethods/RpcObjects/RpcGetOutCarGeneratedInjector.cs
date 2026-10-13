using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcGetOutCarGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcGetOutCar rpcGetOutCar = new RpcGetOutCar();
			Inject(rpcGetOutCar, resolver, parameters);
			return rpcGetOutCar;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
