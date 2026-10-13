using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PlayerProperty.Business;

[Preserve]
internal class ProfitControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ProfitControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ProfitControl obj = (ProfitControl)instance;
		IBusinessProfitControl businessProfitControl = (IBusinessProfitControl)resolver.ResolveOrParameter(typeof(IBusinessProfitControl), "businessProfitControl", parameters);
		obj.Construct(businessProfitControl);
	}
}
