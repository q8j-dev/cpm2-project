using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	internal class RpcRrServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RpcRrService cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcRrService obj = (RpcRrService)instance;
			IEnumerable<RpcRrRequestObjectBase> rpcObjectBase = (IEnumerable<RpcRrRequestObjectBase>)resolver.ResolveOrParameter(typeof(IEnumerable<RpcRrRequestObjectBase>), "rpcObjectBase", parameters);
			obj.Construct(rpcObjectBase);
		}
	}
}
