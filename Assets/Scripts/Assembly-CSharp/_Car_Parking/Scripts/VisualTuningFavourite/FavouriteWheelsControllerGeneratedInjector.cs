using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	[Preserve]
	internal class FavouriteWheelsControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FavouriteWheelsController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FavouriteWheelsController obj = (FavouriteWheelsController)instance;
			AllWheelsCounterConfig allWheelsCounterConfig = (AllWheelsCounterConfig)resolver.ResolveOrParameter(typeof(AllWheelsCounterConfig), "allWheelsCounterConfig", parameters);
			obj.Construct(allWheelsCounterConfig);
		}
	}
}
