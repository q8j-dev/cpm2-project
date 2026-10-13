using System;
using System.Collections.Generic;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class NewGarageMarkViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:NewGarageMarkView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			NewGarageMarkView obj = (NewGarageMarkView)instance;
			IGarageBuyService garageBuyService = (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters);
			obj.Construct(garageBuyService);
		}
	}
}
