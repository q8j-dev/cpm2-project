using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace CPM.Scripts.Garage
{
	[Preserve]
	internal class ExitGarageDetectorGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ExitGarageDetector cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ExitGarageDetector obj = (ExitGarageDetector)instance;
			GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			OptionalDependency<MultiPlayerMenu> multiPlayerMenu = (OptionalDependency<MultiPlayerMenu>)resolver.ResolveOrParameter(typeof(OptionalDependency<MultiPlayerMenu>), "multiPlayerMenu", parameters);
			obj.Construct(garageController, freeDriveProvider, multiPlayerMenu);
		}
	}
}
