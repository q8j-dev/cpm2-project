using System.Collections.Generic;
using VContainer;

[Preserve]
internal class GaragePositionsConfigGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		GaragePositionsConfig garagePositionsConfig = new GaragePositionsConfig();
		Inject(garagePositionsConfig, resolver, parameters);
		return garagePositionsConfig;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
