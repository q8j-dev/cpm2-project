using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.OfflineMode
{
	[Preserve]
	internal class OfflineModeSystemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OfflineModeSystem offlineModeSystem = new OfflineModeSystem();
			Inject(offlineModeSystem, resolver, parameters);
			return offlineModeSystem;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
