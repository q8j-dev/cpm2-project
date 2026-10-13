using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.ToRoad.Handlers
{
	[Preserve]
	internal class OpenWorldToRoadHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OpenWorldToRoadHandler openWorldToRoadHandler = new OpenWorldToRoadHandler((FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters));
			Inject(openWorldToRoadHandler, resolver, parameters);
			return openWorldToRoadHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
