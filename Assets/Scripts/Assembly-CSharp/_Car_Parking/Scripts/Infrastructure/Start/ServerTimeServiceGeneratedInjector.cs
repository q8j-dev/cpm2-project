using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	internal class ServerTimeServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ServerTimeService serverTimeService = new ServerTimeService();
			Inject(serverTimeService, resolver, parameters);
			return serverTimeService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
