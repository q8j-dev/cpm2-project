using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class ArrowsControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ArrowsController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArrowsController obj = (ArrowsController)instance;
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			obj.Construct(carInputManager);
		}
	}
}
