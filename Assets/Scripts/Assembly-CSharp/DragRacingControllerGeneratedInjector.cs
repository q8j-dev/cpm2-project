using System;
using System.Collections.Generic;
using CPM.Connection;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

[Preserve]
internal class DragRacingControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DragRacingController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DragRacingController obj = (DragRacingController)instance;
		RaceFinishUI raceFinishUI = (RaceFinishUI)resolver.ResolveOrParameter(typeof(RaceFinishUI), "raceFinishUI", parameters);
		PlayerDatas.CarLoadingUtility carLoadingUtility = (PlayerDatas.CarLoadingUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.CarLoadingUtility), "carLoadingUtility", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		ScreenFader screenFader = (ScreenFader)resolver.ResolveOrParameter(typeof(ScreenFader), "screenFader", parameters);
		IPlayerStorageService playerStorageService = (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters);
		SoundEventChannel soundEventChannel = (SoundEventChannel)resolver.ResolveOrParameter(typeof(SoundEventChannel), "soundEventChannel", parameters);
		IDragRacingRewardService dragRacingRewardService = (IDragRacingRewardService)resolver.ResolveOrParameter(typeof(IDragRacingRewardService), "dragRacingRewardService", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		IConnectionManager connectionManager = (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
		IPlayerStatusSystem playerStatusSystem = (IPlayerStatusSystem)resolver.ResolveOrParameter(typeof(IPlayerStatusSystem), "playerStatusSystem", parameters);
		RacingAnalytic racingAnalytic = (RacingAnalytic)resolver.ResolveOrParameter(typeof(RacingAnalytic), "racingAnalytic", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		IGuestAccountCreator guestAccountCreator = (IGuestAccountCreator)resolver.ResolveOrParameter(typeof(IGuestAccountCreator), "guestAccountCreator", parameters);
		AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
		obj.Construct(raceFinishUI, carLoadingUtility, prefabFactory, screenFader, playerStorageService, soundEventChannel, dragRacingRewardService, managerUI, connectionManager, controlItemsProvider, coreGameManager, menuLevelParent, freeDriveProvider, racingProvider, playerStatusSystem, racingAnalytic, carInputManager, guestAccountCreator, allCarsInfo, inputMapSetEvent, eventRegistry, steamAchievementService);
	}
}
