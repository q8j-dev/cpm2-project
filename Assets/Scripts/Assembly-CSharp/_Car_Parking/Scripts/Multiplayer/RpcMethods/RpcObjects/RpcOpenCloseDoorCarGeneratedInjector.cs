using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcOpenCloseDoorCarGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcOpenCloseDoorCar rpcOpenCloseDoorCar = new RpcOpenCloseDoorCar();
			Inject(rpcOpenCloseDoorCar, resolver, parameters);
			return rpcOpenCloseDoorCar;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
