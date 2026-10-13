using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;

namespace _Car_Parking.Scripts.GameInput
{
	[Preserve]
	internal class GamepadHapticServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GamepadHapticService gamepadHapticService = new GamepadHapticService((DesktopControlsSettingsSO)resolver.ResolveOrParameter(typeof(DesktopControlsSettingsSO), "settings", parameters), (HapticConfigSO)resolver.ResolveOrParameter(typeof(HapticConfigSO), "config", parameters), (GamepadMirrorTracker)resolver.ResolveOrParameter(typeof(GamepadMirrorTracker), "mirrors", parameters));
			Inject(gamepadHapticService, resolver, parameters);
			return gamepadHapticService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
