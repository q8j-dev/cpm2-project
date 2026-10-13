using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput
{
	[Preserve]
	internal class GameInputManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameInputManager gameInputManager = new GameInputManager();
			Inject(gameInputManager, resolver, parameters);
			return gameInputManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameInputManager obj = (GameInputManager)instance;
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
			UiNavigationValueEvent uiNavigationValueEvent = (UiNavigationValueEvent)resolver.ResolveOrParameter(typeof(UiNavigationValueEvent), "uiNavigationValueEvent", parameters);
			CurrentDeviceValueEvent currentDeviceValueEvent = (CurrentDeviceValueEvent)resolver.ResolveOrParameter(typeof(CurrentDeviceValueEvent), "currentDeviceValueEvent", parameters);
			SteeringWheelValueEvent steeringWheelValueEvent = (SteeringWheelValueEvent)resolver.ResolveOrParameter(typeof(SteeringWheelValueEvent), "steeringWheelValueEvent", parameters);
			DesktopControlsSettingsSO controlsSettingsAsset = (DesktopControlsSettingsSO)resolver.ResolveOrParameter(typeof(DesktopControlsSettingsSO), "controlsSettingsAsset", parameters);
			obj.Construct(eventRegistry, inputMapSetEvent, uiNavigationValueEvent, currentDeviceValueEvent, steeringWheelValueEvent, controlsSettingsAsset);
		}
	}
}
