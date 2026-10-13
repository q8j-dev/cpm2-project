using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[Preserve]
	internal class CPMCrowdNpcCatalogSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CPMCrowdNpcCatalogSO cPMCrowdNpcCatalogSO = new CPMCrowdNpcCatalogSO();
			Inject(cPMCrowdNpcCatalogSO, resolver, parameters);
			return cPMCrowdNpcCatalogSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
