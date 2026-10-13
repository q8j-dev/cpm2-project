using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class CustomSliderValueHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CustomSliderValueHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CustomSliderValueHandler obj = (CustomSliderValueHandler)instance;
			UiNavigationValueEvent uiNavigationValueEvent = (UiNavigationValueEvent)resolver.ResolveOrParameter(typeof(UiNavigationValueEvent), "uiNavigationValueEvent", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			obj.Construct(uiNavigationValueEvent, eventRegistry);
		}
	}
}
