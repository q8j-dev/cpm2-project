using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class MarketInventoryShowCasePanelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MarketInventoryShowCasePanel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MarketInventoryShowCasePanel obj = (MarketInventoryShowCasePanel)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig = (CarTuningDetailRaritySpritesConfig)resolver.ResolveOrParameter(typeof(CarTuningDetailRaritySpritesConfig), "carTuningDetailRaritySpritesConfig", parameters);
		obj.Construct(managerUI, carTuningDetailRaritySpritesConfig);
	}
}
