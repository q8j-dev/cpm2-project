using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.WorldInteraction
{
	[Preserve]
	internal class WorldInteractionControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:WorldInteractionController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WorldInteractionController obj = (WorldInteractionController)instance;
			OptionalDependency<ManagerUI> managerUi = (OptionalDependency<ManagerUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ManagerUI>), "managerUi", parameters);
			obj.Construct(managerUi);
		}
	}
}
