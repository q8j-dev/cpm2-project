using System;
using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class ParkingMenuUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ParkingMenuUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ParkingMenuUI obj = (ParkingMenuUI)instance;
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
			OptionalDependency<LevelScript> levelScript = (OptionalDependency<LevelScript>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelScript>), "levelScript", parameters);
			OptionalDependency<ChatControl> chatControl = (OptionalDependency<ChatControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<ChatControl>), "chatControl", parameters);
			OptionalDependency<FreeDriveControl> freeDriveControl = (OptionalDependency<FreeDriveControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeDriveControl>), "freeDriveControl", parameters);
			OptionalDependency<FreeTimeAttackController> freeTimeAttackController = (OptionalDependency<FreeTimeAttackController>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeTimeAttackController>), "freeTimeAttackController", parameters);
			obj.Construct(menuLevelParent, freeDriveProvider, coreGameManager, managerUI, eventRegistry, inputMapSetEvent, levelScript, chatControl, freeDriveControl, freeTimeAttackController);
		}
	}
}
