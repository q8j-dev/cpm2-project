using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class BrakeSliderControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:BrakeSliderControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		BrakeSliderControl obj = (BrakeSliderControl)instance;
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		obj.Construct(controlItemsProvider, carInputManager);
	}
}
