using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

[Preserve]
internal class UISelectionCursorGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:UISelectionCursor cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		UISelectionCursor obj = (UISelectionCursor)instance;
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		GamepadHapticService gamepadHaptics = (GamepadHapticService)resolver.ResolveOrParameter(typeof(GamepadHapticService), "gamepadHaptics", parameters);
		UiNavigationValueEvent uiNavigationValueEvent = (UiNavigationValueEvent)resolver.ResolveOrParameter(typeof(UiNavigationValueEvent), "uiNavigationValueEvent", parameters);
		obj.Construct(eventRegistry, inputMapSetEvent, gamepadHaptics, uiNavigationValueEvent);
	}
}
