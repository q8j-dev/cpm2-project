using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Preserve]
	internal class BindingOverridesServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BindingOverridesService bindingOverridesService = new BindingOverridesService((GameInputManager)resolver.ResolveOrParameter(typeof(GameInputManager), "inputManager", parameters), (BindingsPrefsStore)resolver.ResolveOrParameter(typeof(BindingsPrefsStore), "store", parameters), (RebindableActionsConfig)resolver.ResolveOrParameter(typeof(RebindableActionsConfig), "config", parameters), (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters), (InputDeviceDetector)resolver.ResolveOrParameter(typeof(InputDeviceDetector), "deviceDetector", parameters), (ActiveWheelService)resolver.ResolveOrParameter(typeof(ActiveWheelService), "activeWheels", parameters));
			Inject(bindingOverridesService, resolver, parameters);
			return bindingOverridesService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
