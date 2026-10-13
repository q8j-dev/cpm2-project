using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class ClutchSliderControlGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ClutchSliderControl cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ClutchSliderControl obj = (ClutchSliderControl)instance;
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			InputItemsProvider inputItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "inputItemsProvider", parameters);
			obj.Construct(freeDriveProvider, carInputManager, inputItemsProvider);
		}
	}
}
