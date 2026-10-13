using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using CPM.RemoteConfig;
using VContainer;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.Multiplayer
{
	[Preserve]
	internal class MultiplayerLobbyHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MultiplayerLobbyHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MultiplayerLobbyHandler obj = (MultiplayerLobbyHandler)instance;
			PlayerDatas.FriendsUtility friendsUtility = (PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters);
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ITimerTuningService timerTuningService = (ITimerTuningService)resolver.ResolveOrParameter(typeof(ITimerTuningService), "timerTuningService", parameters);
			CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
			PhotonReconnectionHandler photonReconnectionHandler = (PhotonReconnectionHandler)resolver.ResolveOrParameter(typeof(PhotonReconnectionHandler), "photonReconnectionHandler", parameters);
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			IAutoSaveService autoSaveService = (IAutoSaveService)resolver.ResolveOrParameter(typeof(IAutoSaveService), "autoSaveService", parameters);
			IRemoteConfigManager remoteConfigManager = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters);
			ArenaFlowController arenaFlowController = (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arenaFlowController", parameters);
			obj.Construct(friendsUtility, playerDataStorage, managerUI, timerTuningService, coreGameManager, photonReconnectionHandler, accountController, autoSaveService, remoteConfigManager, arenaFlowController);
		}
	}
}
