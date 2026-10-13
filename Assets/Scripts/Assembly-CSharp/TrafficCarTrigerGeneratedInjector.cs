using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class TrafficCarTrigerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TrafficCarTriger cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TrafficCarTriger obj = (TrafficCarTriger)instance;
		Sort2 sort = (Sort2)resolver.ResolveOrParameter(typeof(Sort2), "sort2", parameters);
		OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
		obj.Construct(sort, onlineCollectionsProvider);
	}
}
