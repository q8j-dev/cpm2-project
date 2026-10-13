using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	[Preserve]
	internal class ColorsServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ColorsService colorsService = new ColorsService();
			Inject(colorsService, resolver, parameters);
			return colorsService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ColorsService obj = (ColorsService)instance;
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			DefaultMaterialsConfig defaultMaterialsConfig = (DefaultMaterialsConfig)resolver.ResolveOrParameter(typeof(DefaultMaterialsConfig), "defaultMaterialsConfig", parameters);
			CarDirtConfig carDirtConfig = (CarDirtConfig)resolver.ResolveOrParameter(typeof(CarDirtConfig), "carDirtConfig", parameters);
			obj.Construct(carInfoService, defaultMaterialsConfig, carDirtConfig);
		}
	}
}
