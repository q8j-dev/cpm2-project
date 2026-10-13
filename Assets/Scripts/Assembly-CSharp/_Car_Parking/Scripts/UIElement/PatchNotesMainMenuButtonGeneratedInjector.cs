using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class PatchNotesMainMenuButtonGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PatchNotesMainMenuButton cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PatchNotesMainMenuButton obj = (PatchNotesMainMenuButton)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.ConstructBase(managerUI);
		}
	}
}
