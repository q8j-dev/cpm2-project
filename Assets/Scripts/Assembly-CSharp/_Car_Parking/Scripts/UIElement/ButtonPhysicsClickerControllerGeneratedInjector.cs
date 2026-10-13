using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class ButtonPhysicsClickerControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ButtonPhysicsClickerController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ButtonPhysicsClickerController obj = (ButtonPhysicsClickerController)instance;
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			UiNavigationValueEvent uiNav = (UiNavigationValueEvent)resolver.ResolveOrParameter(typeof(UiNavigationValueEvent), "uiNav", parameters);
			UITouchHelper uiTouchHelper = (UITouchHelper)resolver.ResolveOrParameter(typeof(UITouchHelper), "uiTouchHelper", parameters);
			obj.Construct(eventRegistry, uiNav, uiTouchHelper);
		}
	}
}
