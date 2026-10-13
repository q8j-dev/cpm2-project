using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class AllDataInventoryElementGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:AllDataInventoryElement cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		AllDataInventoryElement obj = (AllDataInventoryElement)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig = (CarTuningDetailRaritySpritesConfig)resolver.ResolveOrParameter(typeof(CarTuningDetailRaritySpritesConfig), "carTuningDetailRaritySpritesConfig", parameters);
		DetailVendorsConfigs detailVendorsConfigs = (DetailVendorsConfigs)resolver.ResolveOrParameter(typeof(DetailVendorsConfigs), "detailVendorsConfigs", parameters);
		obj.Construct(managerUI, carTuningDetailRaritySpritesConfig, detailVendorsConfigs);
	}
}
