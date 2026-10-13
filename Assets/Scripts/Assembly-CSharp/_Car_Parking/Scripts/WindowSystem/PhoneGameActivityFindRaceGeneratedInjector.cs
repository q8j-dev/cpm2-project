using System;
using System.Collections.Generic;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class PhoneGameActivityFindRaceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PhoneGameActivityFindRace cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhoneGameActivityFindRace obj = (PhoneGameActivityFindRace)instance;
			ArenaFlowController arenaFlowController = (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arenaFlowController", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			OptionalDependency<MenuLevelParent> menuLevelParent = (OptionalDependency<MenuLevelParent>)resolver.ResolveOrParameter(typeof(OptionalDependency<MenuLevelParent>), "menuLevelParent", parameters);
			ExitToMainMenuService exitToMainMenu = (ExitToMainMenuService)resolver.ResolveOrParameter(typeof(ExitToMainMenuService), "exitToMainMenu", parameters);
			PlayerDatas.FriendsUtility friendsUtility = (PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters);
			PhotonReconnectionHandler reconnect = (PhotonReconnectionHandler)resolver.ResolveOrParameter(typeof(PhotonReconnectionHandler), "reconnect", parameters);
			obj.Construct(arenaFlowController, managerUI, menuLevelParent, exitToMainMenu, friendsUtility, reconnect);
		}
	}
}
