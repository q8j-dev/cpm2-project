using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class qualitySetGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:qualitySet cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		qualitySet obj = (qualitySet)instance;
		GlobalCollectionsProvider provider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "provider", parameters);
		obj.Construct(provider);
	}
}
