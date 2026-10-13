using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Realty;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class RealtyPlaceUIItemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RealtyPlaceUIItem cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RealtyPlaceUIItem obj = (RealtyPlaceUIItem)instance;
			RealtyConfig config = (RealtyConfig)resolver.ResolveOrParameter(typeof(RealtyConfig), "config", parameters);
			obj.Construct(config);
		}
	}
}
