using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

[Preserve]
internal class CarTuningUIElementGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CarTuningUIElement cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarTuningUIElement obj = (CarTuningUIElement)instance;
		AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		DetailVendorsConfigs detailVendorsConfigs = (DetailVendorsConfigs)resolver.ResolveOrParameter(typeof(DetailVendorsConfigs), "detailVendorsConfigs", parameters);
		CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig = (CarTuningDetailRaritySpritesConfig)resolver.ResolveOrParameter(typeof(CarTuningDetailRaritySpritesConfig), "carTuningDetailRaritySpritesConfig", parameters);
		obj.Construct(allParts, managerUI, detailVendorsConfigs, carTuningDetailRaritySpritesConfig);
	}
}
