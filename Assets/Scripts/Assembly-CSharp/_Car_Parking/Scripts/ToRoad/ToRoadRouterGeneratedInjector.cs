using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.ToRoad
{
	[Preserve]
	internal class ToRoadRouterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ToRoadRouter toRoadRouter = new ToRoadRouter((IEnumerable<IToRoadHandler>)resolver.ResolveOrParameter(typeof(IEnumerable<IToRoadHandler>), "handlers", parameters));
			Inject(toRoadRouter, resolver, parameters);
			return toRoadRouter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
