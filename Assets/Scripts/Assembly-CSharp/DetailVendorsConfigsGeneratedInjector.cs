using System.Collections.Generic;
using VContainer;

[Preserve]
internal class DetailVendorsConfigsGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DetailVendorsConfigs detailVendorsConfigs = new DetailVendorsConfigs();
		Inject(detailVendorsConfigs, resolver, parameters);
		return detailVendorsConfigs;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
