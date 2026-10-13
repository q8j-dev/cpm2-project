using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class EngineTemperatureViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EngineTemperatureView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EngineTemperatureView obj = (EngineTemperatureView)instance;
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			obj.Construct(carInputManager);
		}
	}
}
