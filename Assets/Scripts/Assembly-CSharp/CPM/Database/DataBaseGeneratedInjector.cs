using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.VisualTuningFavourite;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace CPM.Database
{
	[Preserve]
	internal class DataBaseGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DataBase dataBase = new DataBase((PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (ExternalSaveableDataAdapter)resolver.ResolveOrParameter(typeof(ExternalSaveableDataAdapter), "externalSaveableDataAdapter", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters), (PersonKitConfigs)resolver.ResolveOrParameter(typeof(PersonKitConfigs), "personKitConfigs", parameters), (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters), (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters), (ProgressContainer)resolver.ResolveOrParameter(typeof(ProgressContainer), "progressContainer", parameters), (PartsInventorySaveLoadService)resolver.ResolveOrParameter(typeof(PartsInventorySaveLoadService), "partsInventorySaveLoadService", parameters), (EngineSaveLoadService)resolver.ResolveOrParameter(typeof(EngineSaveLoadService), "engineSaveLoadService", parameters), (PlatesSaveLoadService)resolver.ResolveOrParameter(typeof(PlatesSaveLoadService), "platesSaveLoadService", parameters), (IInventoryPartsService)resolver.ResolveOrParameter(typeof(IInventoryPartsService), "inventoryPartsService", parameters), (IEngineKitsInventoryService)resolver.ResolveOrParameter(typeof(IEngineKitsInventoryService), "engineKitsInventoryService", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (FriendsSaveLoadService)resolver.ResolveOrParameter(typeof(FriendsSaveLoadService), "friendsSaveLoadService", parameters), (ITimerTuningService)resolver.ResolveOrParameter(typeof(ITimerTuningService), "timerTuningService", parameters), (IEngineTuningTimerService)resolver.ResolveOrParameter(typeof(IEngineTuningTimerService), "engineTuningTimerService", parameters), (SlotsOnDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(SlotsOnDatabaseSaveLoadService), "slotsOnDatabaseSaveLoadService", parameters), (AllWheelsCounterConfig)resolver.ResolveOrParameter(typeof(AllWheelsCounterConfig), "allWheelsCounterConfig", parameters), (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters), (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters), (IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "objectResolver", parameters), (PresetGrantSession)resolver.ResolveOrParameter(typeof(PresetGrantSession), "presetGrantSession", parameters), (OptionalDependency<FreeDriveDB>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeDriveDB>), "freeDriveDB", parameters), (OptionalDependency<ManagerUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ManagerUI>), "managerUI", parameters));
			Inject(dataBase, resolver, parameters);
			return dataBase;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
