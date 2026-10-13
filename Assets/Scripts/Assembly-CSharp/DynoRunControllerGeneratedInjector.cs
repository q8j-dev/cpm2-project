using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using VContainer;

[Preserve]
internal class DynoRunControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DynoRunController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DynoRunController obj = (DynoRunController)instance;
		GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
		GaragePositionsConfig garageConfig = (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garageConfig", parameters);
		IGarageSelectorService selectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "selectorService", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		obj.Construct(garageController, garageConfig, selectorService, carInputManager);
	}
}
