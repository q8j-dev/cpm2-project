using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcSendDataOpenDoorsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcSendDataOpenDoors rpcSendDataOpenDoors = new RpcSendDataOpenDoors();
			Inject(rpcSendDataOpenDoors, resolver, parameters);
			return rpcSendDataOpenDoors;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
