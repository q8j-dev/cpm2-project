using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[Preserve]
	internal class CPMCrowdManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CPMCrowdManager cPMCrowdManager = new CPMCrowdManager((CPMCrowdNpcCatalogSO)resolver.ResolveOrParameter(typeof(CPMCrowdNpcCatalogSO), "npcCatalog", parameters), (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters));
			Inject(cPMCrowdManager, resolver, parameters);
			return cPMCrowdManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
