using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class ThrottleSliderControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ThrottleSliderControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ThrottleSliderControl obj = (ThrottleSliderControl)instance;
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		obj.Construct(controlItemsProvider, freeDriveProvider, carInputManager);
	}
}
