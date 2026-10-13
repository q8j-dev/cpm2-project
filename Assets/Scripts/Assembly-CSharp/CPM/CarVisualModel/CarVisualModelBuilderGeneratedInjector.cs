using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;

namespace CPM.CarVisualModel
{
	[Preserve]
	internal class CarVisualModelBuilderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarVisualModelBuilder carVisualModelBuilder = new CarVisualModelBuilder((ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters), (DefaultMaterialsConfig)resolver.ResolveOrParameter(typeof(DefaultMaterialsConfig), "materialsConfig", parameters), (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters), (PlayerDatas.VinylsDataUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.VinylsDataUtility), "vinylsDataUtility", parameters), (DrawQueue)resolver.ResolveOrParameter(typeof(DrawQueue), "drawQueue", parameters));
			Inject(carVisualModelBuilder, resolver, parameters);
			return carVisualModelBuilder;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
