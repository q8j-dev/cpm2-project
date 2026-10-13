using System;
using System.Collections.Generic;
using VContainer;

namespace CPM.Fallout
{
	[Preserve]
	internal class FalloutLimitsCacheGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FalloutLimitsCache falloutLimitsCache = new FalloutLimitsCache((IFalloutBackend)resolver.ResolveOrParameter(typeof(IFalloutBackend), "backend", parameters), (Func<double>)resolver.ResolveOrParameter(typeof(Func<double>), "now", parameters));
			Inject(falloutLimitsCache, resolver, parameters);
			return falloutLimitsCache;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
