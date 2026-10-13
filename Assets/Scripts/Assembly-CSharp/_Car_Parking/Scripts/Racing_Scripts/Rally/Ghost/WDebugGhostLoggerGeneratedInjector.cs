using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	[Preserve]
	internal class WDebugGhostLoggerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WDebugGhostLogger wDebugGhostLogger = new WDebugGhostLogger();
			Inject(wDebugGhostLogger, resolver, parameters);
			return wDebugGhostLogger;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
