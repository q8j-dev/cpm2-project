using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class TrafficWaypointControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TrafficWaypointControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TrafficWaypointControl obj = (TrafficWaypointControl)instance;
		Sort2 sort = (Sort2)resolver.ResolveOrParameter(typeof(Sort2), "sort2", parameters);
		obj.Construct(sort);
	}
}
