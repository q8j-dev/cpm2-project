using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

[Preserve]
internal class ManualHShifterControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ManualHShifterController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ManualHShifterController obj = (ManualHShifterController)instance;
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		SteeringWheelValueEvent steeringWheelValueEvent = (SteeringWheelValueEvent)resolver.ResolveOrParameter(typeof(SteeringWheelValueEvent), "steeringWheelValueEvent", parameters);
		obj.Construct(carInputManager, eventRegistry, steeringWheelValueEvent);
	}
}
