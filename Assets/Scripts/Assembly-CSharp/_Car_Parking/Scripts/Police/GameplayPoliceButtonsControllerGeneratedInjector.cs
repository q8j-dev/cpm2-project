using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class GameplayPoliceButtonsControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GameplayPoliceButtonsController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameplayPoliceButtonsController obj = (GameplayPoliceButtonsController)instance;
			PoliceButtonsControllerRegistry registry = (PoliceButtonsControllerRegistry)resolver.ResolveOrParameter(typeof(PoliceButtonsControllerRegistry), "registry", parameters);
			obj.Construct(registry);
		}
	}
}
