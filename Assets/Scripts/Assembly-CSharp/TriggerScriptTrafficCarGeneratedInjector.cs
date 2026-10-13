using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class TriggerScriptTrafficCarGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TriggerScriptTrafficCar cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TriggerScriptTrafficCar obj = (TriggerScriptTrafficCar)instance;
		Sort2 sort = (Sort2)resolver.ResolveOrParameter(typeof(Sort2), "sort2", parameters);
		OptionalDependency<TrafficCarControl> trafficCar = (OptionalDependency<TrafficCarControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<TrafficCarControl>), "trafficCar", parameters);
		obj.Construct(sort, trafficCar);
	}
}
