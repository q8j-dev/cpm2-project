using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	internal class RpcRrRequestCarDetailsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcRrRequestCarDetails rpcRrRequestCarDetails = new RpcRrRequestCarDetails((FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters));
			Inject(rpcRrRequestCarDetails, resolver, parameters);
			return rpcRrRequestCarDetails;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
