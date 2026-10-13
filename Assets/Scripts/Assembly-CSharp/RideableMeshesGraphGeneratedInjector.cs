using System.Collections.Generic;
using VContainer;

[Preserve]
internal class RideableMeshesGraphGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RideableMeshesGraph rideableMeshesGraph = new RideableMeshesGraph();
		Inject(rideableMeshesGraph, resolver, parameters);
		return rideableMeshesGraph;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
