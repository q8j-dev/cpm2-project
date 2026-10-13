using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	internal class PrefabFactoryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PrefabFactory prefabFactory = new PrefabFactory((IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "resolver", parameters));
			Inject(prefabFactory, resolver, parameters);
			return prefabFactory;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
