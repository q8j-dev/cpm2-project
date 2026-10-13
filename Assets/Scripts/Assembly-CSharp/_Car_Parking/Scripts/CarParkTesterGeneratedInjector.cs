using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class CarParkTesterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CarParkTester cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarParkTester obj = (CarParkTester)instance;
			CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
			ExteriorTuning exteriorTuning = (ExteriorTuning)resolver.ResolveOrParameter(typeof(ExteriorTuning), "exteriorTuning", parameters);
			GaragePositionsConfig garagePositionsConfig = (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters);
			IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
			obj.Construct(coreGameManager, managerUI, garageController, exteriorTuning, garagePositionsConfig, garageSelectorService);
		}
	}
}
