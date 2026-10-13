using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcOpenCloseTrunkGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcOpenCloseTrunk rpcOpenCloseTrunk = new RpcOpenCloseTrunk();
			Inject(rpcOpenCloseTrunk, resolver, parameters);
			return rpcOpenCloseTrunk;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
