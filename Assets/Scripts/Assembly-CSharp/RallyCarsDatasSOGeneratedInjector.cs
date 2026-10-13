using System.Collections.Generic;
using VContainer;

[Preserve]
internal class RallyCarsDatasSOGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RallyCarsDatasSO rallyCarsDatasSO = new RallyCarsDatasSO();
		Inject(rallyCarsDatasSO, resolver, parameters);
		return rallyCarsDatasSO;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
