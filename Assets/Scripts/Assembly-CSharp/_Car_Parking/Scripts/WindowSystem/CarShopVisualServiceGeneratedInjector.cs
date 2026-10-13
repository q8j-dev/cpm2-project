using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class CarShopVisualServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarShopVisualService carShopVisualService = new CarShopVisualService();
			Inject(carShopVisualService, resolver, parameters);
			return carShopVisualService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
