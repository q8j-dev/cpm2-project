using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

[Preserve]
internal class CarExchangeLongInfoPanelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CarExchangeLongInfoPanel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarExchangeLongInfoPanel obj = (CarExchangeLongInfoPanel)instance;
		AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
		obj.Construct(allParts);
	}
}
