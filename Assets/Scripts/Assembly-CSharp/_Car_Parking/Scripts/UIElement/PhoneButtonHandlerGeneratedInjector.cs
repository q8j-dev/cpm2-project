using System;
using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class PhoneButtonHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PhoneButtonHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhoneButtonHandler obj = (PhoneButtonHandler)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			OptionalDependency<LevelScript> levelScript = (OptionalDependency<LevelScript>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelScript>), "levelScript", parameters);
			OptionalDependency<ChatControl> chatControl = (OptionalDependency<ChatControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<ChatControl>), "chatControl", parameters);
			OptionalDependency<InputItemsProvider> inputItemsProvider = (OptionalDependency<InputItemsProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<InputItemsProvider>), "inputItemsProvider", parameters);
			OptionalDependency<FreeTimeAttackController> freeTimeAttackController = (OptionalDependency<FreeTimeAttackController>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeTimeAttackController>), "freeTimeAttackController", parameters);
			obj.Construct(managerUI, eventRegistry, levelScript, chatControl, inputItemsProvider, freeTimeAttackController);
		}
	}
}
