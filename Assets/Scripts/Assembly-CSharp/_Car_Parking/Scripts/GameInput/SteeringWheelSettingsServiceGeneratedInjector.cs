using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput
{
	[Preserve]
	internal class SteeringWheelSettingsServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteeringWheelSettingsService steeringWheelSettingsService = new SteeringWheelSettingsService();
			Inject(steeringWheelSettingsService, resolver, parameters);
			return steeringWheelSettingsService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteeringWheelSettingsService obj = (SteeringWheelSettingsService)instance;
			DesktopControlsSettingsSO settings = (DesktopControlsSettingsSO)resolver.ResolveOrParameter(typeof(DesktopControlsSettingsSO), "settings", parameters);
			SteeringWheelValueEvent steeringWheelValueEvent = (SteeringWheelValueEvent)resolver.ResolveOrParameter(typeof(SteeringWheelValueEvent), "steeringWheelValueEvent", parameters);
			obj.Construct(settings, steeringWheelValueEvent);
		}
	}
}
