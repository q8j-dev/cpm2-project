using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameInput
{
	[Preserve]
	internal class GamepadMirrorTrackerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GamepadMirrorTracker gamepadMirrorTracker = new GamepadMirrorTracker();
			Inject(gamepadMirrorTracker, resolver, parameters);
			return gamepadMirrorTracker;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
