using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement.HUD;

[Preserve]
internal class QuickCarSettingsGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:QuickCarSettings cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		QuickCarSettings obj = (QuickCarSettings)instance;
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		InputItemsProvider inputItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "inputItemsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		LockCarButtonController lockCarButtonController = (LockCarButtonController)resolver.ResolveOrParameter(typeof(LockCarButtonController), "lockCarButtonController", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		OptionalDependency<MultiPlayer> multiPlayer = (OptionalDependency<MultiPlayer>)resolver.ResolveOrParameter(typeof(OptionalDependency<MultiPlayer>), "multiPlayer", parameters);
		OptionalDependency<HudColorChanger> hudColorChanger = (OptionalDependency<HudColorChanger>)resolver.ResolveOrParameter(typeof(OptionalDependency<HudColorChanger>), "hudColorChanger", parameters);
		obj.Construct(menuLevelParent, inputItemsProvider, freeDriveProvider, coreGameManager, lockCarButtonController, eventRegistry, inputMapSetEvent, managerUI, allCarsInfo, carInputManager, multiPlayer, hudColorChanger);
	}
}
