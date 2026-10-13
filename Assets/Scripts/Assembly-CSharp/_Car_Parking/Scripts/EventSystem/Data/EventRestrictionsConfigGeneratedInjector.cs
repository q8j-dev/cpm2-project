using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Preserve]
	internal class EventRestrictionsConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventRestrictionsConfig eventRestrictionsConfig = new EventRestrictionsConfig();
			Inject(eventRestrictionsConfig, resolver, parameters);
			return eventRestrictionsConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
