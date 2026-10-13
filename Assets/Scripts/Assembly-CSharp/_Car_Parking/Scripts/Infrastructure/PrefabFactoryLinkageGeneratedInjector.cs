using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	internal class PrefabFactoryLinkageGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PrefabFactoryLinkage prefabFactoryLinkage = new PrefabFactoryLinkage((PlayerDatas)resolver.ResolveOrParameter(typeof(PlayerDatas), "playerDatas", parameters), (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters));
			Inject(prefabFactoryLinkage, resolver, parameters);
			return prefabFactoryLinkage;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
