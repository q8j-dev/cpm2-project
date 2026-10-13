using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;

[Preserve]
internal class WheelUIChangerPanelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:WheelUIChangerPanel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		WheelUIChangerPanel obj = (WheelUIChangerPanel)instance;
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		IWheelsDataService wheelsDataService = (IWheelsDataService)resolver.ResolveOrParameter(typeof(IWheelsDataService), "wheelsDataService", parameters);
		obj.Construct(prefabFactory, wheelsDataService);
	}
}
