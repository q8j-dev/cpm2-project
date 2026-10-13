using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Preserve]
	internal class BusinessProfitControlGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BusinessProfitControl businessProfitControl = new BusinessProfitControl((IBusinessService)resolver.ResolveOrParameter(typeof(IBusinessService), "businessService", parameters), (IEarnMoneyService)resolver.ResolveOrParameter(typeof(IEarnMoneyService), "earnMoneyService", parameters), (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters));
			Inject(businessProfitControl, resolver, parameters);
			return businessProfitControl;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
