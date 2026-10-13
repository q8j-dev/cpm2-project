using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.BannerSystem.Rules
{
	[Preserve]
	internal class EventsBannerRuleGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventsBannerRule eventsBannerRule = new EventsBannerRule((IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters));
			Inject(eventsBannerRule, resolver, parameters);
			return eventsBannerRule;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
