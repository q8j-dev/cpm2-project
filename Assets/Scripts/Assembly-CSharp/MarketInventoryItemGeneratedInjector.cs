using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

[Preserve]
internal class MarketInventoryItemGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MarketInventoryItem cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MarketInventoryItem obj = (MarketInventoryItem)instance;
		DetailVendorsConfigs detailVendorsConfigs = (DetailVendorsConfigs)resolver.ResolveOrParameter(typeof(DetailVendorsConfigs), "detailVendorsConfigs", parameters);
		AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig = (CarTuningDetailRaritySpritesConfig)resolver.ResolveOrParameter(typeof(CarTuningDetailRaritySpritesConfig), "carTuningDetailRaritySpritesConfig", parameters);
		obj.Construct(detailVendorsConfigs, allParts, managerUI, carTuningDetailRaritySpritesConfig);
	}
}
