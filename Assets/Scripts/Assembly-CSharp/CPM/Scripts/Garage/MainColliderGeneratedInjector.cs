using System;
using System.Collections.Generic;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;

namespace CPM.Scripts.Garage
{
	[Preserve]
	internal class MainColliderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MainCollider cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MainCollider obj = (MainCollider)instance;
			IGarageBuyService garageBuyService = (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters);
			obj.Construct(garageBuyService);
		}
	}
}
