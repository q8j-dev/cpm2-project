using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using CPM.PlayerData;
using CPM.Rating;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;

namespace CPM.Database
{
	[Preserve]
	internal class FreeDriveDBGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FreeDriveDB cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeDriveDB obj = (FreeDriveDB)instance;
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			RatingSender ratingController = (RatingSender)resolver.ResolveOrParameter(typeof(RatingSender), "ratingController", parameters);
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			DailyTasksManager dailyTasksManager = (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters);
			IConnectionManager connectionManager = (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters);
			IDatabaseCarsService databaseCarsService = (IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			IDatabaseSaveLoadService databaseSaveLoadService = (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters);
			obj.Construct(accountController, onlineCollectionsProvider, playerDataStorage, freeDriveProvider, ratingController, menuLevelParent, dailyTasksManager, connectionManager, databaseCarsService, managerUI, slotService, steamAchievementService, databaseSaveLoadService);
		}
	}
}
