using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.PlayerData;
using CPM.Scripts.Garage;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.ToRoad;
using _Car_Parking.Scripts.UIElement;

[Preserve]
internal class FreeDriveControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:FreeDriveControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		FreeDriveControl obj = (FreeDriveControl)instance;
		PlayerDatas.FriendsUtility friendsUtility = (PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters);
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		PlayerDatas.CarLoadingUtility carLoadingUtility = (PlayerDatas.CarLoadingUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.CarLoadingUtility), "carLoadingUtility", parameters);
		GlobalCollectionsProvider collectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "collectionsProvider", parameters);
		OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
		GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		HomesConfig homesConfig = (HomesConfig)resolver.ResolveOrParameter(typeof(HomesConfig), "homesConfig", parameters);
		IEarnMoneyService earnMoneyService = (IEarnMoneyService)resolver.ResolveOrParameter(typeof(IEarnMoneyService), "earnMoneyService", parameters);
		IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
		PoliceSystemConfig policeConfig = (PoliceSystemConfig)resolver.ResolveOrParameter(typeof(PoliceSystemConfig), "policeConfig", parameters);
		Sort2 sort = (Sort2)resolver.ResolveOrParameter(typeof(Sort2), "sort2", parameters);
		IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		VirtualPurchaseAnalytic virtualPurchaseAnalytic = (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters);
		IBusinessService businessService = (IBusinessService)resolver.ResolveOrParameter(typeof(IBusinessService), "businessService", parameters);
		OpenWorldAnalytic openWorldAnalytic = (OpenWorldAnalytic)resolver.ResolveOrParameter(typeof(OpenWorldAnalytic), "openWorldAnalytic", parameters);
		OptionalDependency<ParkingMenuUI> parkingMenuUI = (OptionalDependency<ParkingMenuUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ParkingMenuUI>), "parkingMenuUI", parameters);
		OptionalDependency<FreeTimeAttackController> timeAttackController = (OptionalDependency<FreeTimeAttackController>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeTimeAttackController>), "timeAttackController", parameters);
		IGameFactManager gameFactManager = (IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "gameFactManager", parameters);
		ToRoadRouter toRoadRouter = (ToRoadRouter)resolver.ResolveOrParameter(typeof(ToRoadRouter), "toRoadRouter", parameters);
		obj.Construct(friendsUtility, playerDataStorage, menuLevelParent, carLoadingUtility, collectionsProvider, onlineCollectionsProvider, garageController, managerUI, prefabFactory, currencyManagerController, homesConfig, earnMoneyService, pricesService, policeConfig, sort, accountController, controlItemsProvider, freeDriveProvider, steamAchievementService, eventRegistry, inputMapSetEvent, levelsProvider, garageSelectorService, carInputManager, virtualPurchaseAnalytic, businessService, openWorldAnalytic, parkingMenuUI, timeAttackController, gameFactManager, toRoadRouter);
	}
}
