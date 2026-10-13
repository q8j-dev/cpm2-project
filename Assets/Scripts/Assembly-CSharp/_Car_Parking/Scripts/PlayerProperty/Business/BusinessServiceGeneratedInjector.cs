using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Preserve]
	internal class BusinessServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BusinessService businessService = new BusinessService((CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (BusinessStorage)resolver.ResolveOrParameter(typeof(BusinessStorage), "businessStorage", parameters), (IEarnMoneyService)resolver.ResolveOrParameter(typeof(IEarnMoneyService), "buyMoneyService", parameters));
			Inject(businessService, resolver, parameters);
			return businessService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
