using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.CarLegitService
{
	[Preserve]
	internal class CarLegitServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarLegitService carLegitService = new CarLegitService((IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfig", parameters), (IProgressBackend)resolver.ResolveOrParameter(typeof(IProgressBackend), "progressBackend", parameters));
			Inject(carLegitService, resolver, parameters);
			return carLegitService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
