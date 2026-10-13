using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

[Preserve]
internal class RacingCarControllerUIGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RacingCarControllerUI cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RacingCarControllerUI obj = (RacingCarControllerUI)instance;
		QuickCarSettings quickCarSettings = (QuickCarSettings)resolver.ResolveOrParameter(typeof(QuickCarSettings), "quickCarSettings", parameters);
		RacingCameraController cameraController = (RacingCameraController)resolver.ResolveOrParameter(typeof(RacingCameraController), "cameraController", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputItemsProvider inputItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "inputItemsProvider", parameters);
		obj.Construct(quickCarSettings, cameraController, eventRegistry, inputItemsProvider);
	}
}
