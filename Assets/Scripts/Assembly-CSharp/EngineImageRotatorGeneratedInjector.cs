using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;

[Preserve]
internal class EngineImageRotatorGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:EngineImageRotator cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		EngineImageRotator obj = (EngineImageRotator)instance;
		StageRender stageRender = (StageRender)resolver.ResolveOrParameter(typeof(StageRender), "stageRender", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		obj.Construct(stageRender, eventRegistry);
	}
}
