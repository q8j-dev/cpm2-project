using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Preserve]
	internal class DesktopControlsSettingsSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DesktopControlsSettingsSO desktopControlsSettingsSO = new DesktopControlsSettingsSO();
			Inject(desktopControlsSettingsSO, resolver, parameters);
			return desktopControlsSettingsSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
