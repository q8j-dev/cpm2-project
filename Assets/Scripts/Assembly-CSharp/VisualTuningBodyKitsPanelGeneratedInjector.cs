using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Economics;

[Preserve]
internal class VisualTuningBodyKitsPanelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:VisualTuningBodyKitsPanel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		VisualTuningBodyKitsPanel obj = (VisualTuningBodyKitsPanel)instance;
		IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
		obj.Construct(pricesService);
	}
}
