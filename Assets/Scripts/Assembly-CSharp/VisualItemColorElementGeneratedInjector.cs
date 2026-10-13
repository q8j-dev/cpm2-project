using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

[Preserve]
internal class VisualItemColorElementGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:VisualItemColorElement cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		VisualItemColorElement obj = (VisualItemColorElement)instance;
		IPaintDataService paintDataService = (IPaintDataService)resolver.ResolveOrParameter(typeof(IPaintDataService), "paintDataService", parameters);
		obj.Construct(paintDataService);
	}
}
