using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	internal class RpcRrRequestShortCarTuningDataGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcRrRequestShortCarTuningData rpcRrRequestShortCarTuningData = new RpcRrRequestShortCarTuningData((FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters));
			Inject(rpcRrRequestShortCarTuningData, resolver, parameters);
			return rpcRrRequestShortCarTuningData;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
