using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class AccountLinkAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AccountLinkAnalytic accountLinkAnalytic = new AccountLinkAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters));
			Inject(accountLinkAnalytic, resolver, parameters);
			return accountLinkAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
