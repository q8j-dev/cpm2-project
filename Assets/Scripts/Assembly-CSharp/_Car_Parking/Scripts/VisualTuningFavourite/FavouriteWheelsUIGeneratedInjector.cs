using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	[Preserve]
	internal class FavouriteWheelsUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FavouriteWheelsUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FavouriteWheelsUI obj = (FavouriteWheelsUI)instance;
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			obj.Construct(eventRegistry);
		}
	}
}
