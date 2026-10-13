using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameInput;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	[Preserve]
	internal class InputDeviceDetectorGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InputDeviceDetector inputDeviceDetector = new InputDeviceDetector();
			Inject(inputDeviceDetector, resolver, parameters);
			return inputDeviceDetector;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InputDeviceDetector obj = (InputDeviceDetector)instance;
			GamepadMirrorTracker mirrors = (GamepadMirrorTracker)resolver.ResolveOrParameter(typeof(GamepadMirrorTracker), "mirrors", parameters);
			obj.ConstructMirrors(mirrors);
		}
	}
}
