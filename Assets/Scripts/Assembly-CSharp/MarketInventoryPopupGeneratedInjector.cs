using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class MarketInventoryPopupGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MarketInventoryPopup cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MarketInventoryPopup obj = (MarketInventoryPopup)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig = (CarTuningDetailRaritySpritesConfig)resolver.ResolveOrParameter(typeof(CarTuningDetailRaritySpritesConfig), "carTuningDetailRaritySpritesConfig", parameters);
		DetailVendorsConfigs detailVendorsConfigs = (DetailVendorsConfigs)resolver.ResolveOrParameter(typeof(DetailVendorsConfigs), "detailVendorsConfigs", parameters);
		VirtualPurchaseAnalytic virtualPurchaseAnalytic = (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters);
		obj.Construct(managerUI, carTuningDetailRaritySpritesConfig, detailVendorsConfigs, virtualPurchaseAnalytic);
	}
}
