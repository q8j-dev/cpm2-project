using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

[Preserve]
internal class CarTuningExchangeUIElementGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CarTuningExchangeUIElement cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarTuningExchangeUIElement obj = (CarTuningExchangeUIElement)instance;
		AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
		DetailVendorsConfigs detailVendorsConfigs = (DetailVendorsConfigs)resolver.ResolveOrParameter(typeof(DetailVendorsConfigs), "detailVendorsConfigs", parameters);
		CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig = (CarTuningDetailRaritySpritesConfig)resolver.ResolveOrParameter(typeof(CarTuningDetailRaritySpritesConfig), "carTuningDetailRaritySpritesConfig", parameters);
		obj.Construct(allParts, detailVendorsConfigs, carTuningDetailRaritySpritesConfig);
	}
}
