using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;

namespace Racing.CameraManagement.SpecificTypes
{
	[Preserve]
	internal class ForwardLookCameraHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ForwardLookCameraHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ForwardLookCameraHandler obj = (ForwardLookCameraHandler)instance;
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
			SteeringWheelValueEvent steeringWheelValueEvent = (SteeringWheelValueEvent)resolver.ResolveOrParameter(typeof(SteeringWheelValueEvent), "steeringWheelValueEvent", parameters);
			OptionalDependency<InputDeviceDetector> inputDeviceDetectorDep = (OptionalDependency<InputDeviceDetector>)resolver.ResolveOrParameter(typeof(OptionalDependency<InputDeviceDetector>), "inputDeviceDetectorDep", parameters);
			OptionalDependency<DesktopControlsSettingsSO> desktopControlsSettingsSOdep = (OptionalDependency<DesktopControlsSettingsSO>)resolver.ResolveOrParameter(typeof(OptionalDependency<DesktopControlsSettingsSO>), "desktopControlsSettingsSOdep", parameters);
			obj.ConstructControlsSettings(cameraManager, eventRegistry, inputMapSetEvent, steeringWheelValueEvent, inputDeviceDetectorDep, desktopControlsSettingsSOdep);
		}
	}
}
