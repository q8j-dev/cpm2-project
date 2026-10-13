using System.Collections.Generic;
using CPM.Scripts.VinylDrawer;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class WindowVinylsTuningScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WindowVinylsTuningScope windowVinylsTuningScope = new WindowVinylsTuningScope();
			Inject(windowVinylsTuningScope, resolver, parameters);
			return windowVinylsTuningScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WindowVinylsTuningScope obj = (WindowVinylsTuningScope)instance;
			VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
			obj.Construct(vinylDrawer);
		}
	}
}
