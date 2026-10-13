using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class SpectatorWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:SpectatorWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		SpectatorWindow obj = (SpectatorWindow)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
		SceneControl sceneControl = (SceneControl)resolver.ResolveOrParameter(typeof(SceneControl), "sceneControl", parameters);
		ChatControl chatControl = (ChatControl)resolver.ResolveOrParameter(typeof(ChatControl), "chatControl", parameters);
		LevelScript levelScript = (LevelScript)resolver.ResolveOrParameter(typeof(LevelScript), "levelScript", parameters);
		OptionalDependency<MenuLevelParent> menuLevelParent = (OptionalDependency<MenuLevelParent>)resolver.ResolveOrParameter(typeof(OptionalDependency<MenuLevelParent>), "menuLevelParent", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		obj.Construct(managerUI, freeDriveProvider, onlineCollectionsProvider, sceneControl, chatControl, levelScript, menuLevelParent, eventRegistry, inputMapSetEvent);
	}
}
