using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	internal class CarClassFilterVisibleControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CarClassFilterVisibleController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarClassFilterVisibleController obj = (CarClassFilterVisibleController)instance;
			NewCarWindowConfig config = (NewCarWindowConfig)resolver.ResolveOrParameter(typeof(NewCarWindowConfig), "config", parameters);
			obj.Construct(config);
		}
	}
}
