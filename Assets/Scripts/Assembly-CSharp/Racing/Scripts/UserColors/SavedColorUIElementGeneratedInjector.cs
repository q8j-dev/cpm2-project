using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace Racing.Scripts.UserColors
{
	[Preserve]
	internal class SavedColorUIElementGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SavedColorUIElement cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SavedColorUIElement obj = (SavedColorUIElement)instance;
			SavedColorCollection savedColorCollection = (SavedColorCollection)resolver.ResolveOrParameter(typeof(SavedColorCollection), "savedColorCollection", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(savedColorCollection, managerUI);
		}
	}
}
