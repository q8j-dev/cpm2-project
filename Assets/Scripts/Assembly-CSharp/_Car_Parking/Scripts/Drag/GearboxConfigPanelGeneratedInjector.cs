using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class GearboxConfigPanelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GearboxConfigPanel cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GearboxConfigPanel obj = (GearboxConfigPanel)instance;
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
			ICarTuningFactory carTuningFactory = (ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters);
			obj.Construct(prefabFactory, managerUI, carInfoService, allParts, carTuningFactory);
		}
	}
}
