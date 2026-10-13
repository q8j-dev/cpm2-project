using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[Preserve]
	internal class RallyPropCatalogSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyPropCatalogSO rallyPropCatalogSO = new RallyPropCatalogSO();
			Inject(rallyPropCatalogSO, resolver, parameters);
			return rallyPropCatalogSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
