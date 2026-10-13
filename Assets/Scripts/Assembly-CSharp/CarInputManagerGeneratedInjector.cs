using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.CPMEngine;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

[Preserve]
internal class CarInputManagerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CarInputManager cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarInputManager obj = (CarInputManager)instance;
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		CarControlValuesSO carControlValuesSo = (CarControlValuesSO)resolver.ResolveOrParameter(typeof(CarControlValuesSO), "carControlValuesSo", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		DesktopControlsSettingsSO controlsSettingsAsset = (DesktopControlsSettingsSO)resolver.ResolveOrParameter(typeof(DesktopControlsSettingsSO), "controlsSettingsAsset", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		GamepadHapticService gamepadHaptics = (GamepadHapticService)resolver.ResolveOrParameter(typeof(GamepadHapticService), "gamepadHaptics", parameters);
		HapticConfigSO hapticConfig = (HapticConfigSO)resolver.ResolveOrParameter(typeof(HapticConfigSO), "hapticConfig", parameters);
		ForceFeedback forceFeedback = (ForceFeedback)resolver.ResolveOrParameter(typeof(ForceFeedback), "forceFeedback", parameters);
		obj.Construct(controlItemsProvider, carControlValuesSo, levelsProvider, eventRegistry, controlsSettingsAsset, inputMapSetEvent, gamepadHaptics, hapticConfig, forceFeedback);
	}
}
