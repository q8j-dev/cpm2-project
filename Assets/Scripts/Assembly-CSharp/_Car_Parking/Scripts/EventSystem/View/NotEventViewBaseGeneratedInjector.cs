using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.EventSystem.View
{
	[Preserve]
	internal class NotEventViewBaseGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:NotEventViewBase cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			NotEventViewBase obj = (NotEventViewBase)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(managerUI);
		}
	}
}
