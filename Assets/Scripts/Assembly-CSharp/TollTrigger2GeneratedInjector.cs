using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class TollTrigger2GeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TollTrigger2 cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TollTrigger2 obj = (TollTrigger2)instance;
		IEarnMoneyService earnMoneyService = (IEarnMoneyService)resolver.ResolveOrParameter(typeof(IEarnMoneyService), "earnMoneyService", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		obj.Construct(earnMoneyService, freeDriveProvider);
	}
}
