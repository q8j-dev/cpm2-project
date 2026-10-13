using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	[Preserve]
	internal class ButtonOpenSlotManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ButtonOpenSlotManager cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ButtonOpenSlotManager obj = (ButtonOpenSlotManager)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(managerUI);
		}
	}
}
