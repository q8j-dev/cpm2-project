using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drone;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement;

[Preserve]
internal class LevelScriptGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:LevelScript cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		LevelScript obj = (LevelScript)instance;
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		GlobalCollectionsProvider collectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "collectionsProvider", parameters);
		LockCarButtonController lockCarButtonController = (LockCarButtonController)resolver.ResolveOrParameter(typeof(LockCarButtonController), "lockCarButtonController", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		LevelsConfig levelsConfig = (LevelsConfig)resolver.ResolveOrParameter(typeof(LevelsConfig), "levelsConfig", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		IPlayerStatusSystem playerStatusSystem = (IPlayerStatusSystem)resolver.ResolveOrParameter(typeof(IPlayerStatusSystem), "playerStatusSystem", parameters);
		LevelsAnalytic levelsAnalytic = (LevelsAnalytic)resolver.ResolveOrParameter(typeof(LevelsAnalytic), "levelsAnalytic", parameters);
		AssetBundleManager assetBundleManager = (AssetBundleManager)resolver.ResolveOrParameter(typeof(AssetBundleManager), "assetBundleManager", parameters);
		AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		OptionalDependency<PlayerDatas.CarLoadingUtility> carLoadingUtility = (OptionalDependency<PlayerDatas.CarLoadingUtility>)resolver.ResolveOrParameter(typeof(OptionalDependency<PlayerDatas.CarLoadingUtility>), "carLoadingUtility", parameters);
		OptionalDependency<QuickCarSettings> quickCarSettings = (OptionalDependency<QuickCarSettings>)resolver.ResolveOrParameter(typeof(OptionalDependency<QuickCarSettings>), "quickCarSettings", parameters);
		OptionalDependency<ICarInfoService> carInfoService = (OptionalDependency<ICarInfoService>)resolver.ResolveOrParameter(typeof(OptionalDependency<ICarInfoService>), "carInfoService", parameters);
		OptionalDependency<ParkingMenuUI> parkingMenuUI = (OptionalDependency<ParkingMenuUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ParkingMenuUI>), "parkingMenuUI", parameters);
		OptionalDependency<ManagerUI> managerUI = (OptionalDependency<ManagerUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ManagerUI>), "managerUI", parameters);
		OptionalDependency<DroneProvider> droneProvider = (OptionalDependency<DroneProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<DroneProvider>), "droneProvider", parameters);
		OptionalDependency<TrafficCarControl> trafficCarControl = (OptionalDependency<TrafficCarControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<TrafficCarControl>), "trafficCarControl", parameters);
		InputItemsProvider inputItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "inputItemsProvider", parameters);
		ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
		obj.Construct(playerDataStorage, menuLevelParent, currencyManagerController, collectionsProvider, lockCarButtonController, controlItemsProvider, freeDriveProvider, levelsConfig, levelsProvider, slotService, carInputManager, playerStatusSystem, levelsAnalytic, assetBundleManager, allCarsInfo, eventRegistry, inputMapSetEvent, carLoadingUtility, quickCarSettings, carInfoService, parkingMenuUI, managerUI, droneProvider, trafficCarControl, inputItemsProvider, steamAchievementService);
	}
}
