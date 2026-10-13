using System.Collections.Generic;
using VContainer;

[Preserve]
internal class CarControlValuesSOGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarControlValuesSO carControlValuesSO = new CarControlValuesSO();
		Inject(carControlValuesSO, resolver, parameters);
		return carControlValuesSO;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
