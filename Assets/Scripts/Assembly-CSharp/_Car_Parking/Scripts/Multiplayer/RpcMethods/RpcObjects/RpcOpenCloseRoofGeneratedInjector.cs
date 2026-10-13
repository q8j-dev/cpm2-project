using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcOpenCloseRoofGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcOpenCloseRoof rpcOpenCloseRoof = new RpcOpenCloseRoof();
			Inject(rpcOpenCloseRoof, resolver, parameters);
			return rpcOpenCloseRoof;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
