using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class CircuitRaceMapControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CircuitRaceMapController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CircuitRaceMapController obj = (CircuitRaceMapController)instance;
		CircuitRaceControlller circuitRaceControlller = (CircuitRaceControlller)resolver.ResolveOrParameter(typeof(CircuitRaceControlller), "circuitRaceControlller", parameters);
		obj.Construct(circuitRaceControlller);
	}
}
