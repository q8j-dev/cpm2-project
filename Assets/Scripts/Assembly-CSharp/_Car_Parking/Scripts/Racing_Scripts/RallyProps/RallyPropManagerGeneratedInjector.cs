using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[Preserve]
	internal class RallyPropManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyPropManager rallyPropManager = new RallyPropManager((RallyPropCatalogSO)resolver.ResolveOrParameter(typeof(RallyPropCatalogSO), "catalog", parameters), (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters));
			Inject(rallyPropManager, resolver, parameters);
			return rallyPropManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
