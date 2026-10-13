using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class TouchMovePersonGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TouchMovePerson cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TouchMovePerson obj = (TouchMovePerson)instance;
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		OptionalDependency<DesktopControlsSettingsSO> settingsDep = (OptionalDependency<DesktopControlsSettingsSO>)resolver.ResolveOrParameter(typeof(OptionalDependency<DesktopControlsSettingsSO>), "settingsDep", parameters);
		obj.Construct(menuLevelParent, controlItemsProvider, freeDriveProvider, carInputManager, eventRegistry, settingsDep);
	}
}
