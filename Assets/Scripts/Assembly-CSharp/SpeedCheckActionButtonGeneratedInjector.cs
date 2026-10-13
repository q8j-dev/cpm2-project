using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;

[Preserve]
internal class SpeedCheckActionButtonGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:SpeedCheckActionButton cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		SpeedCheckActionButton obj = (SpeedCheckActionButton)instance;
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		obj.Construct(eventRegistry);
	}
}
