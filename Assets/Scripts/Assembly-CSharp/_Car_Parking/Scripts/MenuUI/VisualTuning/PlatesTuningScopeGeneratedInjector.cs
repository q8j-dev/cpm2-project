using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class PlatesTuningScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlatesTuningScope platesTuningScope = new PlatesTuningScope();
			Inject(platesTuningScope, resolver, parameters);
			return platesTuningScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlatesTuningScope obj = (PlatesTuningScope)instance;
			VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			obj.Construct(vinylDrawer, carInfoService);
		}
	}
}
