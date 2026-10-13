using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class RacingUIMapGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RacingUIMap cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RacingUIMap obj = (RacingUIMap)instance;
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		Camera camera = (Camera)resolver.ResolveOrParameter(typeof(Camera), "camera", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		OptionalDependency<DesktopControlsSettingsSO> settingsDep = (OptionalDependency<DesktopControlsSettingsSO>)resolver.ResolveOrParameter(typeof(OptionalDependency<DesktopControlsSettingsSO>), "settingsDep", parameters);
		obj.Construct(prefabFactory, camera, eventRegistry, settingsDep);
	}
}
