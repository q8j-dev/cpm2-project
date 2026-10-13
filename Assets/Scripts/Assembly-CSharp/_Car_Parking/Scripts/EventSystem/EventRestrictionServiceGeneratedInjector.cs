using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class EventRestrictionServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventRestrictionService eventRestrictionService = new EventRestrictionService((EventRestrictionsConfig)resolver.ResolveOrParameter(typeof(EventRestrictionsConfig), "config", parameters));
			Inject(eventRestrictionService, resolver, parameters);
			return eventRestrictionService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
