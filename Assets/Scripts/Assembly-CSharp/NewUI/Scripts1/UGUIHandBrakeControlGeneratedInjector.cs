using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;

namespace NewUI.Scripts1
{
	[Preserve]
	internal class UGUIHandBrakeControlGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:UGUIHandBrakeControl cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UGUIHandBrakeControl obj = (UGUIHandBrakeControl)instance;
			CarControlValuesSO carControlValuesSO = (CarControlValuesSO)resolver.ResolveOrParameter(typeof(CarControlValuesSO), "carControlValuesSO", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			obj.Construct(carControlValuesSO, eventRegistry, carInputManager);
		}
	}
}
