using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Analytics
{
	[Preserve]
	internal class RallySessionTrackerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallySessionTracker rallySessionTracker = new RallySessionTracker();
			Inject(rallySessionTracker, resolver, parameters);
			return rallySessionTracker;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
