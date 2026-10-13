using System.Collections.Generic;
using CPM.Account;
using VContainer;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	[Preserve]
	internal class CarBuyRequestFactoryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarBuyRequestFactory carBuyRequestFactory = new CarBuyRequestFactory((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(carBuyRequestFactory, resolver, parameters);
			return carBuyRequestFactory;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
