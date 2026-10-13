using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

[Preserve]
internal class ActionButtonGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ActionButton cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ActionButton obj = (ActionButton)instance;
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		obj.Construct(eventRegistry, freeDriveProvider);
	}
}
