using System.Collections.Generic;
using CPM.Account;
using CPM.Balance;
using CPM.Connection;
using CPM.Database;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.Database.Account
{
	[Preserve]
	internal class RegistrationControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RegistrationController registrationController = new RegistrationController();
			Inject(registrationController, resolver, parameters);
			return registrationController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RegistrationController obj = (RegistrationController)instance;
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			DataBase dataBase = (DataBase)resolver.ResolveOrParameter(typeof(DataBase), "dataBase", parameters);
			IPlayerStorageService playerStorageService = (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters);
			IAccountDataProvider accountDataProvider = (IAccountDataProvider)resolver.ResolveOrParameter(typeof(IAccountDataProvider), "accountDataProvider", parameters);
			ProgressContainer progressContainer = (ProgressContainer)resolver.ResolveOrParameter(typeof(ProgressContainer), "progressContainer", parameters);
			IConnectionManager connectionManager = (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters);
			PlayerPrefsTimer playerPrefsTimer = (PlayerPrefsTimer)resolver.ResolveOrParameter(typeof(PlayerPrefsTimer), "playerPrefsTimer", parameters);
			ProfileAnalytic profileAnalytic = (ProfileAnalytic)resolver.ResolveOrParameter(typeof(ProfileAnalytic), "profileAnalytic", parameters);
			ServerTimeService serverTimeService = (ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters);
			UserPreferenceAnalytic userPreferenceAnalytic = (UserPreferenceAnalytic)resolver.ResolveOrParameter(typeof(UserPreferenceAnalytic), "userPreferenceAnalytic", parameters);
			BootStartData bootStartData = (BootStartData)resolver.ResolveOrParameter(typeof(BootStartData), "bootStartData", parameters);
			IGameDataRepository gameDataRepository = (IGameDataRepository)resolver.ResolveOrParameter(typeof(IGameDataRepository), "gameDataRepository", parameters);
			IConnectionDataHandler connectionDataHandler = (IConnectionDataHandler)resolver.ResolveOrParameter(typeof(IConnectionDataHandler), "connectionDataHandler", parameters);
			OptionalDependency<GarageController> garageControllerDependency = (OptionalDependency<GarageController>)resolver.ResolveOrParameter(typeof(OptionalDependency<GarageController>), "garageControllerDependency", parameters);
			OptionalDependency<IAutoSaveService> autoSaveService = (OptionalDependency<IAutoSaveService>)resolver.ResolveOrParameter(typeof(OptionalDependency<IAutoSaveService>), "autoSaveService", parameters);
			IBalanceDocumentSynchronizer balanceDocumentSynchronizer = (IBalanceDocumentSynchronizer)resolver.ResolveOrParameter(typeof(IBalanceDocumentSynchronizer), "balanceDocumentSynchronizer", parameters);
			ISteamAccountService steamAccountService = (ISteamAccountService)resolver.ResolveOrParameter(typeof(ISteamAccountService), "steamAccountService", parameters);
			obj.Construct(accountController, dataBase, playerStorageService, accountDataProvider, progressContainer, connectionManager, playerPrefsTimer, profileAnalytic, serverTimeService, userPreferenceAnalytic, bootStartData, gameDataRepository, connectionDataHandler, garageControllerDependency, autoSaveService, balanceDocumentSynchronizer, steamAccountService);
		}
	}
}
