using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using CPM.PlayerData;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.FreeDriverService;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Wallet.Cars;

[Preserve]
internal class CoreGameManagerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CoreGameManager cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CoreGameManager obj = (CoreGameManager)instance;
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		IDatabaseSaveLoadService databaseSaveLoadService = (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters);
		OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
		IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
		GlobalCollectionsProvider collectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "collectionsProvider", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
		ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		IPlayerStorageService playerStorageService = (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters);
		IDatabaseCarsService databaseCarsService = (IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters);
		GaragePositionsConfig garagePositionsConfig = (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		LockCarButtonController lockCarButtonController = (LockCarButtonController)resolver.ResolveOrParameter(typeof(LockCarButtonController), "lockCarButtonController", parameters);
		AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
		SlotsOnDatabaseSaveLoadService slotsOnDatabaseSaveLoadService = (SlotsOnDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(SlotsOnDatabaseSaveLoadService), "slotsOnDatabaseSaveLoadService", parameters);
		IBuyCarBackendService buyCarBackendService = (IBuyCarBackendService)resolver.ResolveOrParameter(typeof(IBuyCarBackendService), "buyCarBackendService", parameters);
		CarBuyRequestFactory carBuyRequestFactory = (CarBuyRequestFactory)resolver.ResolveOrParameter(typeof(CarBuyRequestFactory), "carBuyRequestFactory", parameters);
		IPlayerStatusSystem playerStatusSystem = (IPlayerStatusSystem)resolver.ResolveOrParameter(typeof(IPlayerStatusSystem), "playerStatusSystem", parameters);
		AnalyticEventFactory analyticEventFactory = (AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters);
		InterestGroupManager interestGroupManager = (InterestGroupManager)resolver.ResolveOrParameter(typeof(InterestGroupManager), "interestGroupManager", parameters);
		ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
		CharactersService charactersService = (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters);
		IOfflineModeSystem offlineModeSystem = (IOfflineModeSystem)resolver.ResolveOrParameter(typeof(IOfflineModeSystem), "offlineModeSystem", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		ICheckCarDataBase checkCarDataBase = (ICheckCarDataBase)resolver.ResolveOrParameter(typeof(ICheckCarDataBase), "checkCarDataBase", parameters);
		AssetBundleManager assetBundleManager = (AssetBundleManager)resolver.ResolveOrParameter(typeof(AssetBundleManager), "assetBundleManager", parameters);
		OptionalDependency<MapIconsModel> mapIconsModel = (OptionalDependency<MapIconsModel>)resolver.ResolveOrParameter(typeof(OptionalDependency<MapIconsModel>), "mapIconsModel", parameters);
		OptionalDependency<MapIconsDataSo> mapIconsDataSo = (OptionalDependency<MapIconsDataSo>)resolver.ResolveOrParameter(typeof(OptionalDependency<MapIconsDataSo>), "mapIconsDataSo", parameters);
		OptionalDependency<GarageController> garageController = (OptionalDependency<GarageController>)resolver.ResolveOrParameter(typeof(OptionalDependency<GarageController>), "garageController", parameters);
		OptionalDependency<CameraManager> cameraManager = (OptionalDependency<CameraManager>)resolver.ResolveOrParameter(typeof(OptionalDependency<CameraManager>), "cameraManager", parameters);
		OptionalDependency<IGarageSelectorService> garageSelectorService = (OptionalDependency<IGarageSelectorService>)resolver.ResolveOrParameter(typeof(OptionalDependency<IGarageSelectorService>), "garageSelectorService", parameters);
		OptionalDependency<IFreeDriverService> freeDriverService = (OptionalDependency<IFreeDriverService>)resolver.ResolveOrParameter(typeof(OptionalDependency<IFreeDriverService>), "freeDriverService", parameters);
		SoundBankLoadService soundBankLoadService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankLoadService", parameters);
		RainSoundState rainSoundState = (RainSoundState)resolver.ResolveOrParameter(typeof(RainSoundState), "rainSoundState", parameters);
		RainSound rainSound = (RainSound)resolver.ResolveOrParameter(typeof(RainSound), "rainSound", parameters);
		obj.Construct(playerDataStorage, menuLevelParent, databaseSaveLoadService, onlineCollectionsProvider, accountController, collectionsProvider, prefabFactory, freeDriveProvider, levelsProvider, slotService, carInfoService, managerUI, playerStorageService, databaseCarsService, garagePositionsConfig, controlItemsProvider, currencyManagerController, lockCarButtonController, allCarsInfo, slotsOnDatabaseSaveLoadService, buyCarBackendService, carBuyRequestFactory, playerStatusSystem, analyticEventFactory, interestGroupManager, characterEquipmentDataService, charactersService, offlineModeSystem, carInputManager, checkCarDataBase, assetBundleManager, mapIconsModel, mapIconsDataSo, garageController, cameraManager, garageSelectorService, freeDriverService, soundBankLoadService, rainSoundState, rainSound);
	}
}
