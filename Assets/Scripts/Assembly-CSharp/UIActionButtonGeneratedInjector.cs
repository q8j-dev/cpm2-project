using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;

[Preserve]
internal class UIActionButtonGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:UIActionButton cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		UIActionButton obj = (UIActionButton)instance;
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		obj.Construct(eventRegistry);
	}
}
