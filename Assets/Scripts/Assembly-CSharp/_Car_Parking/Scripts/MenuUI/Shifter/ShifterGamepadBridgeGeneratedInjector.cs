using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.MenuUI.Shifter
{
	[Preserve]
	internal class ShifterGamepadBridgeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ShifterGamepadBridge cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ShifterGamepadBridge obj = (ShifterGamepadBridge)instance;
			InputItemsProvider inputItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "inputItemsProvider", parameters);
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			SteeringWheelValueEvent steeringWheelValueEvent = (SteeringWheelValueEvent)resolver.ResolveOrParameter(typeof(SteeringWheelValueEvent), "steeringWheelValueEvent", parameters);
			obj.Construct(inputItemsProvider, carInputManager, eventRegistry, steeringWheelValueEvent);
		}
	}
}
