using System.Collections.Generic;
using VContainer;

[Preserve]
internal class CircuitRacingConfigGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CircuitRacingConfig circuitRacingConfig = new CircuitRacingConfig();
		Inject(circuitRacingConfig, resolver, parameters);
		return circuitRacingConfig;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
