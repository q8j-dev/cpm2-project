using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class SpeedometerTireViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SpeedometerTireView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SpeedometerTireView obj = (SpeedometerTireView)instance;
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			obj.Construct(carInputManager);
		}
	}
}
