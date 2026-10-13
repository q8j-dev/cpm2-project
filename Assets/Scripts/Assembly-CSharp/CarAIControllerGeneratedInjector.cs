using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class CarAIControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CarAIController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarAIController obj = (CarAIController)instance;
		OptionalDependency<Sort2> sort = (OptionalDependency<Sort2>)resolver.ResolveOrParameter(typeof(OptionalDependency<Sort2>), "sort2", parameters);
		OptionalDependency<TrafficCarControl> trafficCarControl = (OptionalDependency<TrafficCarControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<TrafficCarControl>), "trafficCarControl", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		LevelsProvider levelProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelProvider", parameters);
		obj.Construct(sort, trafficCarControl, carInputManager, levelProvider);
	}
}
