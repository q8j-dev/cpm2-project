using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class SummaryOfEventsViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SummaryOfEventsView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SummaryOfEventsView obj = (SummaryOfEventsView)instance;
			IOfferSystem offerSystem = (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters);
			obj.Construct(offerSystem);
		}
	}
}
