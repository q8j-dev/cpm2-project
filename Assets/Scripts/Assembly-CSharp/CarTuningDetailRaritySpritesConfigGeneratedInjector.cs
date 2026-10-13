using System.Collections.Generic;
using VContainer;

[Preserve]
internal class CarTuningDetailRaritySpritesConfigGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig = new CarTuningDetailRaritySpritesConfig();
		Inject(carTuningDetailRaritySpritesConfig, resolver, parameters);
		return carTuningDetailRaritySpritesConfig;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
