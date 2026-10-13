using System;
using System.Collections.Generic;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

[Preserve]
internal class TempCameraRotationDisablerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TempCameraRotationDisabler cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TempCameraRotationDisabler obj = (TempCameraRotationDisabler)instance;
		CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		obj.Construct(cameraManager, freeDriveProvider, eventRegistry);
	}
}
