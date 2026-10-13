using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.Car.CruiseControl;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.CircularMenu
{
	[Preserve]
	internal class CircularMenuControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CircularMenuController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CircularMenuController obj = (CircularMenuController)instance;
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
			OptionalDependency<CruiseControl> cruiseControlDep = (OptionalDependency<CruiseControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<CruiseControl>), "cruiseControlDep", parameters);
			OptionalDependency<QuickCarSettings> quickCarSettingsDep = (OptionalDependency<QuickCarSettings>)resolver.ResolveOrParameter(typeof(OptionalDependency<QuickCarSettings>), "quickCarSettingsDep", parameters);
			OptionalDependency<FreeDriveProvider> freeDriveProviderDep = (OptionalDependency<FreeDriveProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeDriveProvider>), "freeDriveProviderDep", parameters);
			OptionalDependency<MenuLevelParent> menuLevelParentDep = (OptionalDependency<MenuLevelParent>)resolver.ResolveOrParameter(typeof(OptionalDependency<MenuLevelParent>), "menuLevelParentDep", parameters);
			OptionalDependency<ParkingMenuUI> parkingMenuUIDep = (OptionalDependency<ParkingMenuUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ParkingMenuUI>), "parkingMenuUIDep", parameters);
			OptionalDependency<LockCarButtonController> lockCarButtonControllerDep = (OptionalDependency<LockCarButtonController>)resolver.ResolveOrParameter(typeof(OptionalDependency<LockCarButtonController>), "lockCarButtonControllerDep", parameters);
			OptionalDependency<PoliceButtonsControllerRegistry> policeRegistryDep = (OptionalDependency<PoliceButtonsControllerRegistry>)resolver.ResolveOrParameter(typeof(OptionalDependency<PoliceButtonsControllerRegistry>), "policeRegistryDep", parameters);
			OptionalDependency<ChatControl> chatControlDep = (OptionalDependency<ChatControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<ChatControl>), "chatControlDep", parameters);
			OptionalDependency<GarageController> garageControllerDep = (OptionalDependency<GarageController>)resolver.ResolveOrParameter(typeof(OptionalDependency<GarageController>), "garageControllerDep", parameters);
			OptionalDependency<LevelsProvider> levelsProviderDep = (OptionalDependency<LevelsProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelsProvider>), "levelsProviderDep", parameters);
			OptionalDependency<RacingProvider> racingProviderDep = (OptionalDependency<RacingProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<RacingProvider>), "racingProviderDep", parameters);
			OptionalDependency<ManagerUI> managerUiDep = (OptionalDependency<ManagerUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ManagerUI>), "managerUiDep", parameters);
			GamepadHapticService gamepadHaptics = (GamepadHapticService)resolver.ResolveOrParameter(typeof(GamepadHapticService), "gamepadHaptics", parameters);
			OptionalDependency<CarInputManager> carInputManagerDep = (OptionalDependency<CarInputManager>)resolver.ResolveOrParameter(typeof(OptionalDependency<CarInputManager>), "carInputManagerDep", parameters);
			obj.Construct(eventRegistry, inputMapSetEvent, cruiseControlDep, quickCarSettingsDep, freeDriveProviderDep, menuLevelParentDep, parkingMenuUIDep, lockCarButtonControllerDep, policeRegistryDep, chatControlDep, garageControllerDep, levelsProviderDep, racingProviderDep, managerUiDep, gamepadHaptics, carInputManagerDep);
		}
	}
}
