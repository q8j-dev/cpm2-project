using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods
{
	[Preserve]
	internal class RpcCollectionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcCollection rpcCollection = new RpcCollection((IEnumerable<RpcObjectBase>)resolver.ResolveOrParameter(typeof(IEnumerable<RpcObjectBase>), "rpcObjectBase", parameters));
			Inject(rpcCollection, resolver, parameters);
			return rpcCollection;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
