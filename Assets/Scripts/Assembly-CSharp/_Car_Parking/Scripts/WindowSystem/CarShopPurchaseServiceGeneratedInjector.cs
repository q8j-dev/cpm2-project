using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class CarShopPurchaseServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarShopPurchaseService carShopPurchaseService = new CarShopPurchaseService();
			Inject(carShopPurchaseService, resolver, parameters);
			return carShopPurchaseService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
