using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	[Preserve]
	internal class VisualTuningFavouriteUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VisualTuningFavouriteUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualTuningFavouriteUI obj = (VisualTuningFavouriteUI)instance;
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			obj.Construct(eventRegistry);
		}
	}
}
