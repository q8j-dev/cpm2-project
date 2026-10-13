using System.Collections.Generic;
using VContainer;

[Preserve]
internal class BrakeTuningConfigGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		BrakeTuningConfig brakeTuningConfig = new BrakeTuningConfig();
		Inject(brakeTuningConfig, resolver, parameters);
		return brakeTuningConfig;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
